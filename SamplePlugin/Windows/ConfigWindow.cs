using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace SamplePlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    public ConfigWindow(Plugin plugin) : base("Simple Plugin Configuration")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(300, 120);
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
        var movableWindow = Configuration.IsConfigWindowMovable;
        if (ImGui.Checkbox("Make Window Movable", ref movableWindow))
        {
            Configuration.IsConfigWindowMovable = movableWindow;
            Configuration.Save();
        }

        ImGui.Spacing();

        var showWelcomeMessage = Configuration.ShowWelcomeMessage;
        if (ImGui.Checkbox("Show Welcome Message", ref showWelcomeMessage))
        {
            Configuration.ShowWelcomeMessage = showWelcomeMessage;
            Configuration.Save();
        }

        ImGui.Spacing();
        
        if (ImGui.Button("Save and Close"))
        {
            Configuration.Save();
            IsOpen = false;
        }
    }
}
