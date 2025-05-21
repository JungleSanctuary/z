using System;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin Plugin;

    public MainWindow(Plugin plugin, string imagePath)
        : base("Simple Plugin Window", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 200),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        if (Plugin.Configuration.ShowWelcomeMessage)
        {
            ImGui.TextUnformatted("Welcome to my Simple Dalamud Plugin!");
        }
        
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        
        ImGui.TextUnformatted("This is a basic window that displays on the game screen.");
        ImGui.Spacing();
        
        if (ImGui.Button("Show Settings"))
        {
            Plugin.ToggleConfigUI();
        }
        
        ImGui.Spacing();
        
        if (ImGui.Button("Close Window"))
        {
            IsOpen = false;
        }
    }
}
