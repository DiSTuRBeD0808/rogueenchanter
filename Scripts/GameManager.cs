using Godot;

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
        GD.Print("🎮 GameManager: Starting minimal setup...");
        
        // Initialize basic game data
        InitializeGameData();
        
        // Load and setup UI
        InitializeUI();
        
        // Test the models work
        TestModels();
        
        // Update UI with initial data
        UpdateAllUI();
        
        GD.Print("🎮 GameManager: Ready for development!");
    }
    
    private void InitializeGameData()
    {
        GD.Print("📊 InitializeGameData: Creating pure C# models...");
        
        // Create PlayerData from Models/Player/PlayerData.cs
        PlayerData = new PlayerData();
        GD.Print($"📊 PlayerData created - Level: {PlayerData.Level}, Health: {PlayerData.Health}/{PlayerData.MaxHealth}");
        
        // Create GameSettings from Models/Game/GameSettings.cs  
        Settings = new GameSettings();
        GD.Print($"📊 GameSettings created - Volume: {Settings.MasterVolume}, AutoSave: {Settings.AutoSave}");
        
        // Create GameSession from Models/Game/GameSettings.cs
        Session = new GameSession();
        GD.Print($"📊 GameSession created - PlayTime: {Session.PlayTime}");
        
        GD.Print("📊 Basic game data initialized successfully");
    }
    
    private void TestModels()
    {
        GD.Print("🧪 TestModels: Running basic model operations...");
        
        // Test PlayerData operations
        GD.Print($"🧪 Player starting stats - Attack: {PlayerData.TotalAttack}, Defense: {PlayerData.TotalDefense}");
        
        // Test taking damage
        int originalHealth = PlayerData.Health;
        // PlayerData.TakeDamage(10); // Uncomment when TakeDamage method exists
        GD.Print($"🧪 Player health: {originalHealth} -> {PlayerData.Health}");
        
        // Test settings
        GD.Print($"🧪 Settings test - Game speed: {Settings.GameSpeed}");
        
        // Test session
        GD.Print($"🧪 Session test - Current play time: {Session.PlayTime} seconds");
        
        GD.Print("🧪 Model testing completed");
    }
    
    private void InitializeUI()
    {
        GD.Print("🖥️ InitializeUI: Loading GameUI scene...");
        
        // Load the GameUI scene
        var gameUIScene = GD.Load<PackedScene>("res://Scenes/UI/GameUI.tscn");
        if (gameUIScene == null)
        {
            GD.PrintErr("❌ Failed to load GameUI scene!");
            return;
        }
        
        // Instantiate the GameUI
        _gameUI = gameUIScene.Instantiate<GameUI>();
        if (_gameUI == null)
        {
            GD.PrintErr("❌ Failed to instantiate GameUI!");
            return;
        }
        
        // Add GameUI to the UILayer (CanvasLayer) instead of GameManager
        var uiLayer = GetNode("../UILayer");
        if (uiLayer != null)
        {
            uiLayer.AddChild(_gameUI);
            GD.Print("🖥️ GameUI loaded and added to UILayer (CanvasLayer)");
        }
        else
        {
            // Fallback: add to GameManager if UILayer not found
            AddChild(_gameUI);
            GD.PrintErr("⚠️ UILayer not found, added GameUI to GameManager instead");
        }
        
        // Connect GameUI signals to GameManager methods
        ConnectUISignals();
    }
    
    private void ConnectUISignals()
    {
        if (_gameUI == null)
        {
            GD.PrintErr("❌ GameUI is null, cannot connect signals!");
            return;
        }
        
        // Connect test button signals
        _gameUI.LevelUpRequested += OnLevelUpRequested;
        _gameUI.LevelDownRequested += OnLevelDownRequested;
        
        GD.Print("🖥️ GameUI signals connected to GameManager");
    }
    
    // Test button handlers
    private void OnLevelUpRequested()
    {
        GD.Print("🎮 GameManager: Level up requested!");
        
        if (PlayerData != null)
        {
            PlayerData.Level++;
            GD.Print($"🎮 Player level increased to {PlayerData.Level}");
            
            // Update UI with new data
            UpdatePlayerUI();
        }
    }
    
    private void OnLevelDownRequested()
    {
        GD.Print("🎮 GameManager: Level down requested!");
        
        if (PlayerData != null && PlayerData.Level > 1)
        {
            PlayerData.Level--;
            GD.Print($"🎮 Player level decreased to {PlayerData.Level}");
            
            // Update UI with new data
            UpdatePlayerUI();
        }
        else
        {
            GD.Print("🎮 Cannot decrease level below 1");
        }
    }
    
    private void UpdateAllUI()
    {
        if (_gameUI == null)
        {
            GD.Print("⚠️ GameUI is null, skipping UI update");
            return;
        }
        
        GD.Print("🖥️ UpdateAllUI: Refreshing all UI panels...");
        
        // Update Player Stats using new panel system
        UpdatePlayerUI();
        
        // Update other panels (still using old string method for now)
        UpdateOtherUIPanels();
        
        GD.Print("🖥️ All UI panels updated successfully");
    }
    
    private void UpdatePlayerUI()
    {
        if (_gameUI != null && PlayerData != null)
        {
            _gameUI.UpdatePlayerStats(PlayerData);
            GD.Print("🖥️ PlayerStatsPanel updated with real PlayerData");
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
