/// <summary>
/// Core game state enumeration
/// </summary>
public enum GameState
{
    Menu,
    Combat,
    Rest
}

/// <summary>
/// Combat states for the combat system
/// </summary>
public enum CombatState
{
    Idle,
    Victory,
    Defeat
}

/// <summary>
/// State transition types for the state management system
/// </summary>
public enum StateTransition
{
    StartCombat,    // Rest → Combat
    EndCombat,      // Combat → Rest
    EnterMenu,      // Any → Menu
    ExitMenu        // Menu → Rest
}
