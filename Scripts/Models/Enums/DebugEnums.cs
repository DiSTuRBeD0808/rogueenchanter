namespace RogueEnchanter.Models.Enums;

/// <summary>
/// Debug message categories for centralized debug control
/// Each category can be toggled on/off independently
/// </summary>
public enum DebugCategory
{
    // Core Systems
    GameManager,
    GameData,
    
    // UI Systems
    UI_General,
    UI_Panels,
    UI_PlayerStats,
    UI_Buttons,
    UI_Layout,
    
    // Game Systems
    Combat,
    Inventory,
    Player,
    
    // Technical
    Performance,
    Errors,
    Initialization,
    
    // Testing
    Testing,
    
    // Catch-all
    General
}

/// <summary>
/// Debug message priority levels
/// Higher levels are more important and shown more often
/// </summary>
public enum DebugLevel
{
    Verbose = 0,    // Very detailed, only for deep debugging
    Info = 1,       // General information messages
    Warning = 2,    // Potential issues but not breaking
    Error = 3,      // Actual problems that need attention
    Critical = 4    // System-breaking issues
}
