using Godot;
using System.Collections.Generic;
using RogueEnchanter.Models.Enums;

namespace RogueEnchanter.Systems;

/// <summary>
/// Centralized debug message management system
/// Controls which debug messages are shown based on category and level
/// </summary>
public static class DebugManager
{
    // Configuration: Which categories to show (true = enabled, false = disabled)
    private static readonly Dictionary<DebugCategory, bool> _categoryEnabled = new()
    {
        // Core Systems
        { DebugCategory.GameManager, true },
        { DebugCategory.GameData, false },
        
        // UI Systems
        { DebugCategory.UI_General, true },
        { DebugCategory.UI_Panels, true },
        { DebugCategory.UI_PlayerStats, true },
        { DebugCategory.UI_Buttons, false },      // Turn off button spam
        { DebugCategory.UI_Layout, false },       // Turn off layout spam
        
        // Game Systems
        { DebugCategory.Combat, true },
        { DebugCategory.Inventory, true },
        { DebugCategory.Player, true },
        
        // Technical
        { DebugCategory.Performance, true },
        { DebugCategory.Errors, true },
        { DebugCategory.Initialization, true },
        
        // Testing
        { DebugCategory.Testing, true },
        
        // Catch-all
        { DebugCategory.General, true }
    };
    
    // Minimum level to show (messages below this level are filtered out)
    private static DebugLevel _minimumLevel = DebugLevel.Info;
    
    // Master switch - turn off ALL debug messages
    private static bool _debugEnabled = true;
    
    /// <summary>
    /// Main debug print method - replaces GD.Print calls
    /// </summary>
    public static void Log(DebugCategory category, string message, DebugLevel level = DebugLevel.Info)
    {
        // Check master switch
        if (!_debugEnabled) return;
        
        // Check if category is enabled
        if (!_categoryEnabled.TryGetValue(category, out bool categoryEnabled) || !categoryEnabled)
            return;
        
        // Check if level meets minimum
        if (level < _minimumLevel) return;
        
        // Format and print the message
        string prefix = GetPrefix(category, level);
        string formattedMessage = $"{prefix} {message}";
        
        // Use appropriate Godot print method based on level
        switch (level)
        {
            case DebugLevel.Error:
            case DebugLevel.Critical:
                GD.PrintErr(formattedMessage);
                break;
            case DebugLevel.Warning:
                GD.Print($"⚠️ {formattedMessage}");
                break;
            default:
                GD.Print(formattedMessage);
                break;
        }
    }
    
    /// <summary>
    /// Convenience methods for different levels
    /// </summary>
    public static void LogInfo(DebugCategory category, string message) => 
        Log(category, message, DebugLevel.Info);
    
    public static void LogWarning(DebugCategory category, string message) => 
        Log(category, message, DebugLevel.Warning);
    
    public static void LogError(DebugCategory category, string message) => 
        Log(category, message, DebugLevel.Error);
    
    public static void LogVerbose(DebugCategory category, string message) => 
        Log(category, message, DebugLevel.Verbose);
    
    /// <summary>
    /// Configuration methods
    /// </summary>
    public static void EnableCategory(DebugCategory category) => 
        _categoryEnabled[category] = true;
    
    public static void DisableCategory(DebugCategory category) => 
        _categoryEnabled[category] = false;
    
    public static void SetMinimumLevel(DebugLevel level) => 
        _minimumLevel = level;
    
    public static void EnableAllDebug() => _debugEnabled = true;
    public static void DisableAllDebug() => _debugEnabled = false;
    
    /// <summary>
    /// Get appropriate emoji/prefix for category and level
    /// </summary>
    private static string GetPrefix(DebugCategory category, DebugLevel level)
    {
        string emoji = category switch
        {
            DebugCategory.GameManager => "🎮",
            DebugCategory.GameData => "📊",
            DebugCategory.UI_General => "🖥️",
            DebugCategory.UI_Panels => "🎯",
            DebugCategory.UI_PlayerStats => "📊",
            DebugCategory.UI_Buttons => "🔘",
            DebugCategory.UI_Layout => "📐",
            DebugCategory.Combat => "⚔️",
            DebugCategory.Inventory => "🎒",
            DebugCategory.Player => "👤",
            DebugCategory.Performance => "⚡",
            DebugCategory.Errors => "❌",
            DebugCategory.Initialization => "🔧",
            DebugCategory.Testing => "🧪",
            DebugCategory.General => "ℹ️",
            _ => "❓"
        };
        
        string levelIndicator = level switch
        {
            DebugLevel.Critical => "🚨",
            DebugLevel.Error => "❌",
            DebugLevel.Warning => "⚠️",
            DebugLevel.Verbose => "🔍",
            _ => ""
        };
        
        return $"{emoji}{levelIndicator}";
    }
}
