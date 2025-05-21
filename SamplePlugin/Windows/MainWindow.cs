using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using SamplePlugin;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin Plugin;
    private List<PlayerInfo> _partyMembers = new();
    private List<PlayerInfo> _nearbyPlayers = new();

    public MainWindow(Plugin plugin) : base(
        "Sprout Tracker",
        ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
    {
        Plugin = plugin;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(200, 100),
            MaximumSize = new Vector2(1000, 1000)
        };
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Apply saved window size if available
        if (Plugin.Configuration.MainWindowSize != Vector2.Zero)
        {
            Size = Plugin.Configuration.MainWindowSize;
            SizeCondition = ImGuiCond.FirstUseEver;
        }

        // Apply saved position if available
        if (Plugin.Configuration.MainWindowPosition != Vector2.Zero)
        {
            Position = Plugin.Configuration.MainWindowPosition;
            PositionCondition = ImGuiCond.FirstUseEver;
        }

        // Lock window position if configured
        if (Plugin.Configuration.LockMainWindowPosition)
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
    }

    public override void OnClose()
    {
        // Save window size and position when closed
        Plugin.Configuration.MainWindowSize = Size ?? new Vector2(400, 300);
        Plugin.Configuration.MainWindowPosition = Position ?? new Vector2(100, 100);
        Plugin.Configuration.Save();
    }

    public override void Draw()
    {
        // Update player lists
        _partyMembers = Plugin.GetPartySpRouters();
        _nearbyPlayers = Plugin.GetNearbySpRouters();

        // Welcome message
        if (Plugin.Configuration.ShowWelcomeMessage)
        {
            ImGui.TextColored(new Vector4(1, 1, 0, 1), "Welcome to Sprout Tracker!");
            ImGui.Text("This window displays sprouts and returners in your current party and area.");
            ImGui.Separator();
        }

        // Party Member Section
        if (Plugin.Configuration.ShowPartyMembers)
        {
            ImGui.PushStyleColor(ImGuiCol.Header, Plugin.Configuration.HeaderColor);
            if (ImGui.CollapsingHeader("Party Members", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.PopStyleColor();

                if (_partyMembers.Count == 0)
                {
                    ImGui.Text("No sprouts or returners in party.");
                }
                else
                {
                    DrawPlayerTable(_partyMembers, "party");
                }
            }
            else
            {
                ImGui.PopStyleColor();
            }
        }

        // Nearby Players Section
        if (Plugin.Configuration.ShowNearbyPlayers)
        {
            ImGui.PushStyleColor(ImGuiCol.Header, Plugin.Configuration.HeaderColor);
            if (ImGui.CollapsingHeader("Nearby Players", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.PopStyleColor();

                if (_nearbyPlayers.Count == 0)
                {
                    ImGui.Text("No sprouts or returners nearby.");
                }
                else
                {
                    DrawPlayerTable(_nearbyPlayers, "nearby");
                }
            }
            else
            {
                ImGui.PopStyleColor();
            }
        }

        ImGui.Separator();

        // Footer with buttons
        if (ImGui.Button("Settings"))
        {
            Plugin.ToggleConfigUI();
        }

        ImGui.SameLine();

        if (ImGui.Button("Refresh"))
        {
            // Force data refresh
        }
    }

    private void DrawPlayerTable(List<PlayerInfo> players, string id)
    {
        if (ImGui.BeginTable($"playersTable_{id}", Plugin.Configuration.ShowJobIcons ? 4 : 3, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Table headers
            ImGui.TableSetupColumn(Plugin.Configuration.ShowJobIcons ? "Job" : "Name", ImGuiTableColumnFlags.WidthFixed, 50);
            ImGui.TableSetupColumn(Plugin.Configuration.ShowJobIcons ? "Name" : "World", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn(Plugin.Configuration.ShowJobIcons ? "World" : "Status", ImGuiTableColumnFlags.WidthFixed, 80);
            if (Plugin.Configuration.ShowJobIcons)
            {
                ImGui.TableSetupColumn("Status", ImGuiTableColumnFlags.WidthFixed, 80);
            }
            ImGui.TableHeadersRow();

            // Table rows
            foreach (var player in players)
            {
                ImGui.TableNextRow();
                  // First column - Job icon or Name
                ImGui.TableNextColumn();
                if (Plugin.Configuration.ShowJobIcons)
                {
                // Just display job ID as text for now
                ImGui.Text($"Job {player.JobId}");
                }
                else
                {
                    ImGui.Text(player.Name);
                }

                // Second column - Name or World
                ImGui.TableNextColumn();
                if (Plugin.Configuration.ShowJobIcons)
                {
                    ImGui.Text(player.Name);
                }
                else
                {
                    ImGui.Text(player.WorldName);
                }

                // Third column - World or Status
                ImGui.TableNextColumn();
                if (Plugin.Configuration.ShowJobIcons)
                {
                    ImGui.Text(player.WorldName);
                }
                else
                {
                    if (player.IsSprout)
                    {
                        ImGui.TextColored(Plugin.Configuration.SproutColor, "Sprout");
                    }
                    else if (player.IsReturner)
                    {
                        ImGui.TextColored(Plugin.Configuration.ReturnerColor, "Returner");
                    }
                }

                // Fourth column - Status (only if job icons are shown)
                if (Plugin.Configuration.ShowJobIcons)
                {
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
            }
              ImGui.EndTable();
        }
    }

    private uint GetJobIconId(uint jobId)
    {
        // These icon IDs are based on the game's job icons
        // Values may need to be adjusted based on the actual game data

        // Tanks
        if (jobId == 1 || jobId == 19) return 62101; // Gladiator/Paladin
        if (jobId == 3 || jobId == 21) return 62119; // Marauder/Warrior
        if (jobId == 32) return 62132; // Dark Knight
        if (jobId == 37) return 62137; // Gunbreaker

        // Healers
        if (jobId == 6 || jobId == 24) return 62106; // Conjurer/White Mage
        if (jobId == 26) return 62126; // Scholar
        if (jobId == 33) return 62133; // Astrologian
        if (jobId == 40) return 62140; // Sage

        // Melee DPS
        if (jobId == 2 || jobId == 20) return 62102; // Pugilist/Monk
        if (jobId == 4 || jobId == 22) return 62104; // Lancer/Dragoon
        if (jobId == 29) return 62129; // Rogue/Ninja
        if (jobId == 34) return 62134; // Samurai
        if (jobId == 39) return 62139; // Reaper

        // Ranged Physical DPS
        if (jobId == 5 || jobId == 23) return 62105; // Archer/Bard
        if (jobId == 31) return 62131; // Machinist
        if (jobId == 38) return 62138; // Dancer

        // Magical DPS
        if (jobId == 7 || jobId == 25) return 62107; // Thaumaturge/Black Mage
        if (jobId == 26 || jobId == 27) return 62127; // Arcanist/Summoner
        if (jobId == 35) return 62135; // Red Mage
        if (jobId == 36) return 62136; // Blue Mage

        // Crafters
        if (jobId == 8) return 62108; // Carpenter
        if (jobId == 9) return 62109; // Blacksmith
        if (jobId == 10) return 62110; // Armorer
        if (jobId == 11) return 62111; // Goldsmith
        if (jobId == 12) return 62112; // Leatherworker
        if (jobId == 13) return 62113; // Weaver
        if (jobId == 14) return 62114; // Alchemist
        if (jobId == 15) return 62115; // Culinarian

        // Gatherers
        if (jobId == 16) return 62116; // Miner
        if (jobId == 17) return 62117; // Botanist
        if (jobId == 18) return 62118; // Fisher

        // Default icon for unknown jobs
        return 62101;
    }
}