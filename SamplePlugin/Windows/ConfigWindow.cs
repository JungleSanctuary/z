using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Dalamud.Interface.Colors;
using SamplePlugin;

namespace SamplePlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;
    private Plugin Plugin;

    public ConfigWindow(Plugin plugin) : base("Sprout Tracker Configuration")
    {
        Plugin = plugin;
        
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(400, 500);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (Configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
    }

    public override void Draw()
    {
        // Display Settings Section
        if (ImGui.CollapsingHeader("Display Settings", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Indent(10);
            
            // Player type filters
            var showPartyMembers = Configuration.ShowPartyMembers;
            if (ImGui.Checkbox("Show Party Members", ref showPartyMembers))
            {
                Configuration.ShowPartyMembers = showPartyMembers;
                Configuration.Save();
            }
            
            var showNearbyPlayers = Configuration.ShowNearbyPlayers;
            if (ImGui.Checkbox("Show Nearby Players", ref showNearbyPlayers))
            {
                Configuration.ShowNearbyPlayers = showNearbyPlayers;
                Configuration.Save();
            }
            
            ImGui.Spacing();
            
            // Player status filters
            var showSprouts = Configuration.ShowSprouts;
            if (ImGui.Checkbox("Show Sprouts", ref showSprouts))
            {
                Configuration.ShowSprouts = showSprouts;
                Configuration.Save();
            }
            
            var showReturners = Configuration.ShowReturners;
            if (ImGui.Checkbox("Show Returners", ref showReturners))
            {
                Configuration.ShowReturners = showReturners;
                Configuration.Save();
            }
            
            ImGui.Spacing();
            
            // Nearby player radius
            var nearbyPlayerRadius = Configuration.NearbyPlayerRadius;
            if (ImGui.SliderFloat("Nearby Player Radius (yalms)", ref nearbyPlayerRadius, 5.0f, 100.0f, "%.1f"))
            {
                Configuration.NearbyPlayerRadius = nearbyPlayerRadius;
                Configuration.Save();
            }
            
            // Max nearby players
            var maxNearbyPlayers = Configuration.MaxNearbyPlayers;
            if (ImGui.SliderInt("Max Nearby Players", ref maxNearbyPlayers, 5, 50))
            {
                Configuration.MaxNearbyPlayers = maxNearbyPlayers;
                Configuration.Save();
            }
            
            ImGui.Spacing();
            
            // Show job icons
            var showJobIcons = Configuration.ShowJobIcons;
            if (ImGui.Checkbox("Show Job Icons", ref showJobIcons))
            {
                Configuration.ShowJobIcons = showJobIcons;
                Configuration.Save();
            }
            
            ImGui.Unindent(10);
        }
        
        // Window Settings Section
        if (ImGui.CollapsingHeader("Window Settings"))
        {
            ImGui.Indent(10);
            
            // Config window movable
            var configMovable = Configuration.IsConfigWindowMovable;
            if (ImGui.Checkbox("Make Config Window Movable", ref configMovable))
            {
                Configuration.IsConfigWindowMovable = configMovable;
                Configuration.Save();
            }
            
            // Lock main window position
            var lockMainWindowPosition = Configuration.LockMainWindowPosition;
            if (ImGui.Checkbox("Lock Main Window Position", ref lockMainWindowPosition))
            {
                Configuration.LockMainWindowPosition = lockMainWindowPosition;
                Configuration.Save();
            }
            
            // Window size
            var windowSize = Configuration.MainWindowSize;
            if (ImGui.DragFloat2("Window Size", ref windowSize, 1.0f, 200.0f, 800.0f))
            {
                Configuration.MainWindowSize = windowSize;
                Configuration.Save();
            }
            
            // Auto hide in combat
            var autoHideInCombat = Configuration.AutoHideInCombat;
            if (ImGui.Checkbox("Auto-hide in Combat", ref autoHideInCombat))
            {
                Configuration.AutoHideInCombat = autoHideInCombat;
                Configuration.Save();
            }
            
            ImGui.Unindent(10);
        }
        
        // Color Settings Section
        if (ImGui.CollapsingHeader("Color Settings"))
        {
            ImGui.Indent(10);
            
            // Sprout color
            var sproutColor = Configuration.SproutColor;
            if (ImGui.ColorEdit4("Sprout Color", ref sproutColor))
            {
                Configuration.SproutColor = sproutColor;
                Configuration.Save();
            }
            
            // Returner color
            var returnerColor = Configuration.ReturnerColor;
            if (ImGui.ColorEdit4("Returner Color", ref returnerColor))
            {
                Configuration.ReturnerColor = returnerColor;
                Configuration.Save();
            }
            
            // Header color
            var headerColor = Configuration.HeaderColor;
            if (ImGui.ColorEdit4("Header Color", ref headerColor))
            {
                Configuration.HeaderColor = headerColor;
                Configuration.Save();
            }
            
            // Reset colors button
            if (ImGui.Button("Reset Colors to Default"))
            {
                Configuration.SproutColor = new Vector4(0.0f, 0.8f, 0.0f, 1.0f);  // Green
                Configuration.ReturnerColor = new Vector4(0.0f, 0.5f, 1.0f, 1.0f); // Blue
                Configuration.HeaderColor = new Vector4(1.0f, 0.8f, 0.0f, 1.0f);   // Gold
                Configuration.Save();
            }
            
            ImGui.Unindent(10);
        }
        
        ImGui.Separator();
        
        // Welcome message toggle (kept from original)
        var showWelcomeMessage = Configuration.ShowWelcomeMessage;
        if (ImGui.Checkbox("Show Welcome Message", ref showWelcomeMessage))
        {
            Configuration.ShowWelcomeMessage = showWelcomeMessage;
            Configuration.Save();
        }
        
        ImGui.Spacing();
        ImGui.Spacing();
        
        // Save and close button
        if (ImGui.Button("Save and Close", new Vector2(120, 30)))
        {
            Configuration.Save();
            IsOpen = false;
        }
        
        ImGui.SameLine();
        
        // Open main window button
        if (ImGui.Button("Open Tracker", new Vector2(120, 30)))
        {
            Plugin.ToggleMainUI();
        }
    }
}
