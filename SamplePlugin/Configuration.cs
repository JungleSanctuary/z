using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Numerics;

namespace SamplePlugin;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    // Window settings
    public bool IsConfigWindowMovable { get; set; } = true;
    public bool ShowWelcomeMessage { get; set; } = true;
    public Vector2 MainWindowPosition { get; set; } = new Vector2(100, 100);
    public Vector2 MainWindowSize { get; set; } = new Vector2(400, 300);
    public bool LockMainWindowPosition { get; set; } = false;
    
    // Display settings
    public bool ShowPartyMembers { get; set; } = true;
    public bool ShowNearbyPlayers { get; set; } = true;
    public bool ShowSprouts { get; set; } = true;
    public bool ShowReturners { get; set; } = true;
    public float NearbyPlayerRadius { get; set; } = 30.0f;
    public int MaxNearbyPlayers { get; set; } = 20;
    
    // Color settings - stored as RGBA vectors
    public Vector4 SproutColor { get; set; } = new Vector4(0.0f, 0.8f, 0.0f, 1.0f); // Green
    public Vector4 ReturnerColor { get; set; } = new Vector4(0.0f, 0.5f, 1.0f, 1.0f); // Blue
    public Vector4 HeaderColor { get; set; } = new Vector4(1.0f, 0.8f, 0.0f, 1.0f); // Gold
    
    // Misc settings
    public bool AutoHideInCombat { get; set; } = false;
    public bool ShowJobIcons { get; set; } = true;

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
