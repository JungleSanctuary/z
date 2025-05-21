using System;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using Dalamud.Interface;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin Plugin;

    public MainWindow(Plugin plugin)
        : base("Sprout Tracker")
    {
        Plugin = plugin;        // Use configuration for window sizing
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(250, 200),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        // Fix nullable Vector2 issue - use default values if null
        if (Plugin.Configuration.MainWindowSize != Vector2.Zero)
            Size = (Vector2)Plugin.Configuration.MainWindowSize;
        else
            Size = new Vector2(400, 300);

        if (Plugin.Configuration.MainWindowPosition != Vector2.Zero)
            Position = (Vector2)Plugin.Configuration.MainWindowPosition;
        else
            Position = new Vector2(100, 100);

        // Configure window flags
        Flags = ImGuiWindowFlags.NoScrollbar;
          if (Plugin.Configuration.LockMainWindowPosition)
            Flags |= ImGuiWindowFlags.NoMove;
    }

    public void Dispose() { }

    // Save window position and size when closing
    public override void OnClose()
    {
        // Cast Size and Position to Vector2 to ensure non-nullable
        Vector2 currentSize = Size ?? new Vector2(400, 300);
        Vector2 currentPosition = Position ?? new Vector2(100, 100);

        Plugin.Configuration.MainWindowSize = currentSize;
        Plugin.Configuration.MainWindowPosition = currentPosition;
        Plugin.Configuration.Save();
    }

    public override void Draw()
    {
        // Get players of interest
        var partyPlayers = Plugin.GetPartySpRouters();
        var nearbyPlayers = Plugin.GetNearbySpRouters();

        bool hasPartyPlayers = partyPlayers.Count > 0;
        bool hasNearbyPlayers = nearbyPlayers.Count > 0;

        // No sprouts or returners found
        if (!hasPartyPlayers && !hasNearbyPlayers)
        {
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), "No sprouts or returners found.");
            return;
        }

        // Display party sprouts and returners
        if (Plugin.Configuration.ShowPartyMembers)
        {
            DrawPlayerSection("Party Members", partyPlayers, hasPartyPlayers);
        }

        // Display nearby sprouts and returners
        if (Plugin.Configuration.ShowNearbyPlayers)
        {
            DrawPlayerSection("Nearby Players", nearbyPlayers, hasNearbyPlayers);
        }
    }

    private void DrawPlayerSection(string title, List<PlayerInfo> players, bool hasPlayers)
    {
        // Section header
        ImGui.Separator();
        ImGui.TextColored(Plugin.Configuration.HeaderColor, title);
        ImGui.Separator();

        // No players in this section
        if (!hasPlayers)
        {
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), $"No sprouts or returners found in {title.ToLower()}.");
            return;
        }

        // Begin a child region for scrolling
        using (var childRegion = ImRaii.Child($"##ScrollRegion{title}", new Vector2(-1, 0), true))
        {
            if (childRegion.Success)
            {
                // Draw table
                if (ImGui.BeginTable($"##Table{title}", Plugin.Configuration.ShowJobIcons ? 4 : 3, ImGuiTableFlags.RowBg))
                {
                    // Set up columns
                    if (Plugin.Configuration.ShowJobIcons)
                        ImGui.TableSetupColumn("Job", ImGuiTableColumnFlags.WidthFixed, 25);

                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.None);
                    ImGui.TableSetupColumn("World", ImGuiTableColumnFlags.WidthFixed, 80);
                    ImGui.TableSetupColumn("Status", ImGuiTableColumnFlags.WidthFixed, 70);

                    // Display players
                    foreach (var player in players)
                    {
                        ImGui.TableNextRow();

                        // Job icon
                        if (Plugin.Configuration.ShowJobIcons)
                        {
                            ImGui.TableNextColumn();
                            try
                            {
                                var jobIconId = GetJobIconIdForPlayer(player);
                                DrawJobIcon(jobIconId);
                            }
                            catch (Exception ex)
                            {
                                Plugin.Log.Error(ex, "Failed to load job icon");
                            }
                        }

                        // Name
                        ImGui.TableNextColumn();
                        ImGui.Text(player.Name);

                        // World
                        ImGui.TableNextColumn();
                        ImGui.Text(player.WorldName);

                        // Status
                        ImGui.TableNextColumn();

                        if (player.IsSprout)
                        {
                            ImGui.TextColored(Plugin.Configuration.SproutColor, "Sprout");
                        }
                        else if (player.IsReturner)
                        {
                            ImGui.TextColored(Plugin.Configuration.ReturnerColor, "Returner");
                        }
                    }

                    ImGui.EndTable();
                }
            }
        }
    }

    private string GetPlayerWorldName(PlayerInfo player)
    {
        // Simply return the WorldName property from PlayerInfo
        return player.WorldName;
    }

    private uint GetJobIconIdForPlayer(PlayerInfo player)
    {
        // Get job icon ID based on the JobId in PlayerInfo
        uint jobId = player.JobId;

        // Map job IDs to their respective icons
        // These are the standard job icons in FFXIV
        switch (jobId)
        {
            // Tanks
            case 1: return 62101; // Gladiator
            case 19: return 62119; // Paladin
            case 3: return 62103; // Marauder
            case 21: return 62121; // Warrior
            case 32: return 62132; // Dark Knight
            case 37: return 62137; // Gunbreaker

            // Healers
            case 6: return 62106; // Conjurer
            case 24: return 62124; // White Mage
            case 26: return 62126; // Arcanist (Scholar)
            case 28: return 62128; // Scholar
            case 33: return 62133; // Astrologian
            case 40: return 62140; // Sage

            // Melee DPS
            case 2: return 62102; // Pugilist
            case 20: return 62120; // Monk
            case 4: return 62104; // Lancer
            case 22: return 62122; // Dragoon
            case 29: return 62129; // Rogue
            case 30: return 62130; // Ninja
            case 34: return 62134; // Samurai
            case 39: return 62139; // Reaper

            // Ranged Physical DPS
            case 5: return 62105; // Archer
            case 23: return 62123; // Bard
            case 31: return 62131; // Machinist
            case 38: return 62138; // Dancer

            // Casters
            case 7: return 62107; // Thaumaturge
            case 25: return 62125; // Black Mage
            case 27: return 62127; // Arcanist (Summoner)
            case 36: return 62136; // Blue Mage
            case 35: return 62135; // Red Mage

            // Crafters
            case 8: return 62108; // Carpenter
            case 9: return 62109; // Blacksmith
            case 10: return 62110; // Armorer
            case 11: return 62111; // Goldsmith
            case 12: return 62112; // Leatherworker
            case 13: return 62113; // Weaver
            case 14: return 62114; // Alchemist
            case 15: return 62115; // Culinarian

            // Gatherers
            case 16: return 62116; // Miner
            case 17: return 62117; // Botanist
            case 18: return 62118; // Fisher

            default: return 62101; // Default icon if job not recognized
        }
    }

    private void DrawJobIcon(uint jobIconId)
    {
        try
        {
            if (Plugin.DataManager.GetFile($"ui/icon/{jobIconId / 1000 * 1000:000000}/{jobIconId:000000}.tex") is not null)
                ImGui.Text($"[{jobIconId}]");
            else
                ImGui.Text("?");
        }
        catch (Exception ex)
        {
            ImGui.Text("[ERR]");
            Plugin.Log.Error(ex, "Failed to load job icon");
        }
    }
}
