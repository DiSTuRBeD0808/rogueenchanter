#if !GODOT_SOURCE_GENERATORS
using System;

/// <summary>
/// Game configuration and settings data
/// Pure C# class with no Godot dependencies
/// </summary>
public class GameSettings
{
    // Audio Settings
    public float MasterVolume { get; set; } = 1.0f;
    public float SFXVolume { get; set; } = 1.0f;
    public float MusicVolume { get; set; } = 1.0f;
    
    // Gameplay Settings
    public bool AutoSave { get; set; } = true;
    public float GameSpeed { get; set; } = 1.0f;
    public bool ShowDamageNumbers { get; set; } = true;
    public bool AutoAttackEnabled { get; set; } = false;
    
    // UI Settings
    public bool ShowTooltips { get; set; } = true;
    public float UIScale { get; set; } = 1.0f;
    public bool FullScreen { get; set; } = false;
    
    // Debug Settings
    public bool DebugMode { get; set; } = false;
    public bool ShowFPS { get; set; } = false;
    
    // TODO: Implement save/load functionality
    public void SaveSettings()
    {
        Console.WriteLine("💾 Settings saved (placeholder)");
    }
    
    public void LoadSettings()
    {
        Console.WriteLine("📁 Settings loaded (placeholder)");
    }
}

/// <summary>
/// Game session data - temporary data that doesn't persist
/// Simple C# class for tracking session statistics
/// </summary>
public class GameSession
{
    public float PlayTime { get; set; } = 0.0f;
    public int EnemiesDefeated { get; set; } = 0;
    public int ItemsFound { get; set; } = 0;
    public int GoldEarned { get; set; } = 0;
    public int DamageDealt { get; set; } = 0;
    public int DamageTaken { get; set; } = 0;
    
    public void ResetSession()
    {
        PlayTime = 0.0f;
        EnemiesDefeated = 0;
        ItemsFound = 0;
        GoldEarned = 0;
        DamageDealt = 0;
        DamageTaken = 0;
    }
    
    // TODO: Implement time tracking
    // TODO: Add session statistics methods
}
#endif
