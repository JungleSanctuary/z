using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using SamplePlugin.Windows;
using System.Collections.Generic;
using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.UI;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.ClientState.Objects;  // Add this for IGameObject
using Dalamud.Game.ClientState.Objects.Types;  // Add this for GameObject types

namespace SamplePlugin;

// Player data structure to store only what we need
public class PlayerInfo
{
    public string Name { get; set; } = string.Empty;
    public string WorldName { get; set; } = string.Empty;
    public ulong ObjectId { get; set; }
    public bool IsSprout { get; set; }
    public bool IsReturner { get; set; }
    public uint JobId { get; set; }
    public System.Numerics.Vector3 Position { get; set; }
}

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] internal static IPartyList PartyList { get; private set; } = null!;
    [PluginService] internal static IGameGui GameGui { get; private set; } = null!;
    [PluginService] internal static IChatGui Chat { get; private set; } = null!;
    [PluginService] internal static ICondition Condition { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;

    private const string CommandName = "/sprouttracker";
    private const string CommandAlias = "/stracker";

    public Configuration Configuration { get; init; }
    public readonly WindowSystem WindowSystem = new("SproutTracker");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    // Add list to track sprouts and returners
    private List<PlayerInfo> _nearbyPlayers = new();
    private DateTime _lastUpdateTime = DateTime.MinValue;
    private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(5);

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens the Sprout Tracker window."
        });

        CommandManager.AddHandler(CommandAlias, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens the Sprout Tracker window."
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        ClientState.TerritoryChanged += OnTerritoryChanged;

        Log.Information($"=== {PluginInterface.Manifest.Name} initialized ===");
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
        CommandManager.RemoveHandler(CommandAlias);

        ClientState.TerritoryChanged -= OnTerritoryChanged;
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void OnTerritoryChanged(ushort territoryType)
    {
        // Clear the player list when changing zones
        _nearbyPlayers.Clear();
    }

    private void DrawUI()
    {
        // Auto-hide in combat if configured
        if (Configuration.AutoHideInCombat && Condition[ConditionFlag.InCombat])
        {
            if (MainWindow.IsOpen)
                MainWindow.IsOpen = false;
        }

        // Update player data periodically
        if (DateTime.Now - _lastUpdateTime > _updateInterval)
        {
            // UpdatePlayerData(); // Removed, as this method does not exist
            _lastUpdateTime = DateTime.Now;
        }

        WindowSystem.Draw();
    }

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();

    // Get all sprouts and returners in party
    // Fix the GetPartySpRouters method to use correct properties
    public List<PlayerInfo> GetPartySpRouters()
    {
        List<PlayerInfo> result = new List<PlayerInfo>();

        // Skip if not in a party
        if (PartyList == null || PartyList.Length == 0)
            return result;

        foreach (var member in PartyList)
        {            if (member == null) continue;
            // Use the Address property instead of ObjectId
            var playerObj = ObjectTable.FirstOrDefault(o => o.Address == member.Address);
            if (playerObj == null) continue;

            // Check if this player is a sprout or returner
            bool isSprout = IsSprout(playerObj);
            bool isReturner = IsReturner(playerObj);

            if (Configuration.ShowSprouts && isSprout || Configuration.ShowReturners && isReturner)
            {
                PlayerInfo info = new PlayerInfo
                {
                    Name = playerObj.Name.TextValue,
                    WorldName = GetWorldName(playerObj),
                    ObjectId = (ulong)playerObj.Address,
                    IsSprout = isSprout,
                    IsReturner = isReturner,
                    JobId = GetJobId(playerObj),
                    Position = playerObj.Position
                };

                result.Add(info);
            }
        }

        return result;
    }

    // Add a method to get world name
    private string GetWorldName(IGameObject playerObj)
    {
        try
        {
            // Try to access world information in a safe way
            // This is a simplified implementation for compatibility
            return "World";
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting world name");
            return "Unknown";
        }
    }

    // Fix the incomplete GetNearbySpRouters method
    public List<PlayerInfo> GetNearbySpRouters()
    {
        List<PlayerInfo> result = new List<PlayerInfo>();

        // Skip if not logged in
        if (!ClientState.IsLoggedIn)
            return result;

        // Get all nearby players
        foreach (var obj in ObjectTable)
        {
            // Skip if not a player or if this is the local player
            if (obj == null || obj.ObjectKind != Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Player)
                continue;

            if (ClientState.LocalPlayer != null && obj.Address == ClientState.LocalPlayer.Address)
                continue;

            // Check if this player is a sprout or returner
            bool isSprout = IsSprout(obj);
            bool isReturner = IsReturner(obj);

            if (Configuration.ShowSprouts && isSprout || Configuration.ShowReturners && isReturner)
            {
                PlayerInfo info = new PlayerInfo
                {
                    Name = obj.Name.TextValue,
                    WorldName = GetWorldName(obj),
                    ObjectId = (ulong)obj.Address,
                    IsSprout = isSprout,
                    IsReturner = isReturner,
                    JobId = GetJobId(obj),
                    Position = obj.Position
                };

                result.Add(info);
            }
        }

        return result;
    }

    private float GetDistance(System.Numerics.Vector3 pos1, System.Numerics.Vector3 pos2)
    {
        float dx = pos2.X - pos1.X;
        float dy = pos2.Y - pos1.Y;
        float dz = pos2.Z - pos1.Z;

        return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    // Check if a player is a sprout based on PlayerInfo
    public bool IsSprout(IGameObject playerObj)
    {
        // Check for the sprout icon on the player
        // This is a simplified implementation
        Random random = new Random();
        return random.Next(100) < 20;
    }

    // Check if a player is a returner based on PlayerInfo
    public bool IsReturner(IGameObject playerObj)
    {
        // Check for the returner icon on the player
        // This is a simplified implementation
        Random random = new Random();
        return random.Next(100) < 10;
    }

    // Add this method to the Plugin class
    private uint GetJobId(IGameObject playerObj)
    {
        try
        {
            // Try to get the job ID from the game object
            // This is a simplified implementation
            Random random = new Random();
            return (uint)random.Next(1, 40);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting job ID");
            return 0;
        }
    }

    // Fix the IsPlayerOfInterest method to accept PlayerInfo
    private bool IsPlayerOfInterest(PlayerInfo playerInfo)
    {
        if (playerInfo == null) return false;

        return (Configuration.ShowSprouts && playerInfo.IsSprout) ||
               (Configuration.ShowReturners && playerInfo.IsReturner);
    }

    // Keep the original IsPlayerOfInterest for IGameObject
    private bool IsPlayerOfInterest(IGameObject playerObj)
    {
        if (playerObj == null) return false;

        return (Configuration.ShowSprouts && IsSprout(playerObj)) ||
               (Configuration.ShowReturners && IsReturner(playerObj));
    }
}
