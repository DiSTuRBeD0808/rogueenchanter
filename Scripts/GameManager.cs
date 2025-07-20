using Godot;
using RogueEnchanter.Systems;
using RogueEnchanter.Models.Enums;

/// <summary>
/// Central game orchestrator that coordinates all game systems
/// Minimal starting point - features will be implemented step by step
/// Uses pure C# models from Models folder with GameManager providing Godot integration
/// </summary>
public partial class GameManager : Node
{
    // Game State
    public GameState CurrentState { get; private set; } = GameState.Menu;
    
    // Game Data (pure C# models from Models folder)
    public PlayerData PlayerData { get; private set; }
    public GameSettings Settings { get; private set; }
    public GameSession Session { get; private set; }
    
    // UI Reference
    private GameUI _gameUI;
    
    public override void _Ready()
    {
        DebugManager.Log(DebugCategory.GameManager, "Starting minimal setup...");
        
        // Initialize basic game data
        InitializeGameData();
        
        // Load and setup UI
        InitializeUI();
        
        // Test the models work
        TestModels();
        
        // Update UI with initial data
        UpdateAllUI();
        
        DebugManager.Log(DebugCategory.GameManager, "Ready for development!");
    }
    
    private void InitializeGameData()
    {
        DebugManager.Log(DebugCategory.GameData, "Creating pure C# models...");
        
        // Create PlayerData from Models/Player/PlayerData.cs
        PlayerData = new PlayerData();
        DebugManager.LogVerbose(DebugCategory.GameData, $"PlayerData created - Level: {PlayerData.Level}, Health: {PlayerData.Health}/{PlayerData.MaxHealth}");
        
        // Create GameSettings from Models/Game/GameSettings.cs  
        Settings = new GameSettings();
        DebugManager.LogVerbose(DebugCategory.GameData, $"GameSettings created - Volume: {Settings.MasterVolume}, AutoSave: {Settings.AutoSave}");
        
        // Create GameSession from Models/Game/GameSettings.cs
        Session = new GameSession();
        DebugManager.LogVerbose(DebugCategory.GameData, $"GameSession created - PlayTime: {Session.PlayTime}");
        
        DebugManager.Log(DebugCategory.GameData, "Basic game data initialized successfully");
    }
    
    private void TestModels()
    {
        DebugManager.Log(DebugCategory.Testing, "Running basic model operations...", DebugLevel.Info);
        
        // Test PlayerData operations
        DebugManager.Log(DebugCategory.Player, $"Player starting stats - Attack: {PlayerData.TotalAttack}, Defense: {PlayerData.TotalDefense}", DebugLevel.Info);
        
        // Test taking damage
        int originalHealth = PlayerData.Health;
        // PlayerData.TakeDamage(10); // Uncomment when TakeDamage method exists
        DebugManager.Log(DebugCategory.Player, $"Player health: {originalHealth} -> {PlayerData.Health}", DebugLevel.Info);
        
        // Test settings
        DebugManager.Log(DebugCategory.GameData, $"Settings test - Game speed: {Settings.GameSpeed}", DebugLevel.Info);
        
        // Test session
        DebugManager.Log(DebugCategory.GameData, $"Session test - Current play time: {Session.PlayTime} seconds", DebugLevel.Info);
        
        DebugManager.Log(DebugCategory.Testing, "Model testing completed", DebugLevel.Info);
    }
    
    private void InitializeUI()
    {
        DebugManager.Log(DebugCategory.UI_General, "Loading GameUI scene...", DebugLevel.Info);
        
        // Load the GameUI scene
        var gameUIScene = GD.Load<PackedScene>("res://Scenes/UI/GameUI.tscn");
        if (gameUIScene == null)
        {
            DebugManager.Log(DebugCategory.UI_General, "Failed to load GameUI scene!", DebugLevel.Error);
            return;
        }
        
        // Instantiate the GameUI
        _gameUI = gameUIScene.Instantiate<GameUI>();
        if (_gameUI == null)
        {
            DebugManager.Log(DebugCategory.UI_General, "Failed to instantiate GameUI!", DebugLevel.Error);
            return;
        }
        
        // Add GameUI to the UILayer (CanvasLayer) instead of GameManager
        var uiLayer = GetNode("../UILayer");
        if (uiLayer != null)
        {
            uiLayer.AddChild(_gameUI);
            DebugManager.Log(DebugCategory.UI_General, "GameUI loaded and added to UILayer (CanvasLayer)", DebugLevel.Info);
        }
        else
        {
            // Fallback: add to GameManager if UILayer not found
            AddChild(_gameUI);
            DebugManager.Log(DebugCategory.UI_General, "UILayer not found, added GameUI to GameManager instead", DebugLevel.Warning);
        }
        
        // Connect GameUI signals to GameManager methods
        ConnectUISignals();
    }
    
    private void ConnectUISignals()
    {
        if (_gameUI == null)
        {
            DebugManager.Log(DebugCategory.UI_General, "GameUI is null, cannot connect signals!", DebugLevel.Error);
            return;
        }
        
        // Connect test button signals
        _gameUI.LevelUpRequested += OnLevelUpRequested;
        _gameUI.LevelDownRequested += OnLevelDownRequested;
        
        DebugManager.Log(DebugCategory.UI_General, "GameUI signals connected to GameManager", DebugLevel.Info);
    }
    
    // Test button handlers
    private void OnLevelUpRequested()
    {
        DebugManager.Log(DebugCategory.Testing, "Level up requested!");
        
        if (PlayerData != null)
        {
            PlayerData.Level++;
            DebugManager.Log(DebugCategory.Player, $"Player level increased to {PlayerData.Level}");
            
            // Update UI with new data
            UpdatePlayerUI();
        }
    }
    
    private void OnLevelDownRequested()
    {
        DebugManager.Log(DebugCategory.Testing, "Level down requested!");
        
        if (PlayerData != null && PlayerData.Level > 1)
        {
            PlayerData.Level--;
            DebugManager.Log(DebugCategory.Player, $"Player level decreased to {PlayerData.Level}");
            
            // Update UI with new data
            UpdatePlayerUI();
        }
        else
        {
            DebugManager.LogWarning(DebugCategory.Player, "Cannot decrease level below 1");
        }
    }
    
    private void UpdateAllUI()
    {
        if (_gameUI == null)
        {
            DebugManager.Log(DebugCategory.UI_General, "GameUI is null, skipping UI update", DebugLevel.Warning);
            return;
        }
        
        DebugManager.Log(DebugCategory.UI_General, "Refreshing all UI panels...", DebugLevel.Info);
        
        // Update Player Stats using new panel system
        UpdatePlayerUI();
        
        // Update other panels (still using old string method for now)
        UpdateOtherUIPanels();
        
        DebugManager.Log(DebugCategory.UI_General, "All UI panels updated successfully", DebugLevel.Info);
    }
    
    private void UpdatePlayerUI()
    {
        if (_gameUI != null && PlayerData != null)
        {
            _gameUI.UpdatePlayerStats(PlayerData);
            DebugManager.Log(DebugCategory.UI_PlayerStats, "PlayerStatsPanel updated with real PlayerData", DebugLevel.Verbose);
        }
    }
    
    private void UpdateOtherUIPanels()
    {
        // Update Enemy Stats (placeholder for now)
        string enemyStats = "ENEMY STATS\nNo enemy present\n(Enter combat to spawn enemy)";
        _gameUI.UpdateEnemyStats(enemyStats);
        
        // Update Combat Stats (placeholder)
        string combatStats = "COMBAT STATS\nDamage Dealt: 0\nDamage Taken: 0\nCritical Hits: 0\nCombo: 0x";
        _gameUI.UpdateCombatStats(combatStats);
        
        // Update Inventory (placeholder with real player data)
        string inventory = $"INVENTORY\nItems: 0/10\nGold: {PlayerData.Gold}\n\n(Empty)\n\n[Use] [Drop] [Info]";
        _gameUI.UpdateInventory(inventory);
        
        // Update Game Display (placeholder)
        string gameDisplay = "GAME DISPLAY\nExploring the dungeon...\n\nPress buttons to test level changes.";
        _gameUI.UpdateGameDisplay(gameDisplay);
        
        // Update Shop (placeholder)
        string shop = "UPGRADE SHOP\n- Weapon Upgrades\n- Armor Upgrades\n- Stat Boosts\n- Special Items\n\n[Buy] [Sell] [Info]";
        _gameUI.UpdateUpgradeShop(shop);
        
        // Update Runes (placeholder)
        string runes = "RUNE UPGRADES\nActive Runes: 0/5\n\nAvailable:\n- Fire Rune\n- Ice Rune\n- Lightning Rune\n\n[Upgrade] [Equip] [Info]";
        _gameUI.UpdateRuneUpgrades(runes);
    }
    
    // TODO: Implement system references
    // private void GetSystemReferences() { }
    
    // TODO: Implement signal connections
    // private void ConnectSystemSignals() { }
    
    // TODO: Implement state management
    // public void TransitionToState(GameState newState) { }
    
    // TODO: Implement event handlers
    // private void OnAttackButtonPressed() { }
    // private void OnInventoryToggled(bool isOpen) { }
    // etc.
}
