/// <summary>
/// Core game state enumeration
/// </summary>
public enum GameState
{
    Menu,
    Playing,
    Paused,
    Inventory,
    Shop,
    GameOver
}

/// <summary>
/// Combat states for the combat system
/// </summary>
public enum CombatState
{
    Idle,
    PlayerTurn,
    EnemyTurn,
    Processing,
    Victory,
    Defeat
}

/// <summary>
/// UI panel states
/// </summary>
public enum UIState
{
    MainGame,
    Inventory,
    Shop,
    Settings,
    Paused
}
