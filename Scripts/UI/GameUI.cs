using Godot;

/// <summary>
/// Main game UI controller for the 3x2 grid layout
/// Handles UI updates from GameManager - no game logic here
/// Now uses panel-based architecture for modular UI management
/// </summary>
public partial class GameUI : Control
{
    // Panel References
    private PlayerStatsPanel _playerStatsPanel;
    
    // UI References (for non-panel elements)
    private Label _enemyStatsLabel;
    private Label _combatStatsLabel;
    private Label _inventoryLabel;
    private Label _upgradeShopLabel;
    private Label _runeUpgradesLabel;
    
    private Button _attackButton;
    private Button _restButton;
    
    // Test buttons for PlayerStatsPanel
    private Button _levelUpButton;
    private Button _levelDownButton;
    
    private ColorRect _playerVisual;
    private ColorRect _enemyVisual;
    
    // Signals for test buttons
    [Signal] public delegate void LevelUpRequestedEventHandler();
    [Signal] public delegate void LevelDownRequestedEventHandler();
    
    public override void _Ready()
    {
        GD.Print("🖥️ GameUI: Initializing 3x2 grid layout with panel system...");
        
        // Initialize panels
        InitializePanels();
        
        // Create test buttons
        CreateTestButtons();
        
        // Get references to non-panel UI elements
        GetUIReferences();
        
        // Connect button signals
        ConnectButtonSignals();
        
        GD.Print("🖥️ GameUI: Layout ready with panel system!");
    }
    
    private void InitializePanels()
    {
        GD.Print("🖥️ GameUI: Initializing panels...");
        
        // Create and setup PlayerStatsPanel
        _playerStatsPanel = new PlayerStatsPanel();
        
        // Get the existing PlayerStats panel node from the scene
        var playerStatsContainer = GetNode<Panel>("GridContainer/PlayerStats");
        if (playerStatsContainer != null)
        {
            // Replace the existing label with our PlayerStatsPanel
            var existingLabel = playerStatsContainer.GetNodeOrNull<Label>("PlayerStatsLabel");
            if (existingLabel != null)
            {
                // Remove the old label and add our panel
                existingLabel.QueueFree();
            }
            
            // Add our PlayerStatsPanel to the container
            playerStatsContainer.AddChild(_playerStatsPanel);
            _playerStatsPanel.Name = "PlayerStatsPanel";
            
            // Set layout properties to fill the parent panel (like the original label)
            _playerStatsPanel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            _playerStatsPanel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            _playerStatsPanel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            
            // Force the layout mode to container mode
            _playerStatsPanel.Set("layout_mode", 1); // 1 = anchors mode
            
            GD.Print("🖥️ GameUI: PlayerStatsPanel added to container");
        }
        else
        {
            GD.PrintErr("❌ GameUI: Could not find PlayerStats container!");
        }
    }
    
    private void CreateTestButtons()
    {
        GD.Print("🖥️ GameUI: Creating test buttons for PlayerStatsPanel...");
        
        // Get the existing GameButtons container
        var gameButtonsContainer = GetNodeOrNull<HBoxContainer>("GridContainer/GameDisplay/GameDisplayContent/GameButtons");
        if (gameButtonsContainer != null)
        {
            // Get references to existing buttons BEFORE removing container
            var attackButton = gameButtonsContainer.GetNode<Button>("AttackButton");
            var restButton = gameButtonsContainer.GetNode<Button>("RestButton");
            
            // Store references to the original buttons for later use
            _attackButton = attackButton;
            _restButton = restButton;
            
            // Remove buttons from old container (but don't destroy them)
            gameButtonsContainer.RemoveChild(attackButton);
            gameButtonsContainer.RemoveChild(restButton);
            
            // Get parent and index for replacement
            var parent = gameButtonsContainer.GetParent();
            var index = gameButtonsContainer.GetIndex();
            
            // Remove the old container
            gameButtonsContainer.QueueFree();
            
            // Create a new GridContainer to replace the HBoxContainer
            var buttonGridContainer = new GridContainer();
            buttonGridContainer.Name = "GameButtons";
            buttonGridContainer.Columns = 2; // 2x2 grid
            
            // Center align the grid container
            buttonGridContainer.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
            buttonGridContainer.SizeFlagsVertical = Control.SizeFlags.ShrinkEnd; // Align to bottom
            
            // Create Level Up button
            _levelUpButton = new Button();
            _levelUpButton.Text = "Level Up (+1)";
            _levelUpButton.Name = "LevelUpButton";
            _levelUpButton.CustomMinimumSize = new Vector2(120, 40);
            _levelUpButton.Disabled = false;
            _levelUpButton.Visible = true;
            
            // Create Level Down button  
            _levelDownButton = new Button();
            _levelDownButton.Text = "Level Down (-1)";
            _levelDownButton.Name = "LevelDownButton";
            _levelDownButton.CustomMinimumSize = new Vector2(120, 40);
            _levelDownButton.Disabled = false;
            _levelDownButton.Visible = true;
            
            // Add all buttons to the grid in order:
            // Row 1: Attack, Rest
            // Row 2: Level Up, Level Down
            buttonGridContainer.AddChild(attackButton);
            buttonGridContainer.AddChild(restButton);
            buttonGridContainer.AddChild(_levelUpButton);
            buttonGridContainer.AddChild(_levelDownButton);
            
            // Add the new grid container back to the parent
            parent.AddChild(buttonGridContainer);
            parent.MoveChild(buttonGridContainer, index);
            
            GD.Print($"🖥️ GameUI: Button grid created with all 4 buttons");
            GD.Print($"🖥️ GameUI: Attack button preserved: {attackButton != null}");
            GD.Print($"🖥️ GameUI: Rest button preserved: {restButton != null}");
            GD.Print($"�️ GameUI: Level Up button created: {_levelUpButton != null}");
            GD.Print($"�️ GameUI: Level Down button created: {_levelDownButton != null}");
            
            // Debug: Check button properties
            GD.Print($"🔍 DEBUG: Button grid children count: {buttonGridContainer.GetChildCount()}");
        }
        else
        {
            GD.PrintErr("❌ GameUI: Could not find GameButtons container!");
        }
    }
    
    private void GetUIReferences()
    {
        // Get all non-panel label references
        _enemyStatsLabel = GetNode<Label>("GridContainer/EnemyAndCombatStats/EnemyAndCombatContent/EnemyStatsLabel");
        _combatStatsLabel = GetNode<Label>("GridContainer/EnemyAndCombatStats/EnemyAndCombatContent/CombatStatsLabel");
        _inventoryLabel = GetNode<Label>("GridContainer/Inventory/InventoryLabel");
        _upgradeShopLabel = GetNode<Label>("GridContainer/UpgradeShop/UpgradeShopLabel");
        _runeUpgradesLabel = GetNode<Label>("GridContainer/RuneUpgrades/RuneUpgradesLabel");
        
        // Button references are now set in CreateTestButtons() method
        // No need to get them here since they've been moved to GridContainer
        
        // Get visual references
        _playerVisual = GetNode<ColorRect>("GridContainer/GameDisplay/GameDisplayContent/GameVisuals/PlayerVisual");
        _enemyVisual = GetNode<ColorRect>("GridContainer/GameDisplay/GameDisplayContent/GameVisuals/EnemyVisual");
        
        // Hide the old player visual since we're using PlayerStatsPanel now
        if (_playerVisual != null)
        {
            _playerVisual.Visible = false;
            GD.Print("🖥️ GameUI: Old PlayerVisual hidden");
        }
        
        // Add a new player visual to GameDisplay area
        CreateGameDisplayPlayerVisual();
        
        GD.Print("🖥️ GameUI: All UI references obtained");
    }
    
    private void ConnectButtonSignals()
    {
        _attackButton.Pressed += OnAttackButtonPressed;
        _restButton.Pressed += OnRestButtonPressed;
        
        // Connect test buttons
        if (_levelUpButton != null)
        {
            _levelUpButton.Pressed += OnLevelUpButtonPressed;
            GD.Print("🖥️ GameUI: Level Up button signal connected");
        }
        else
        {
            GD.PrintErr("❌ GameUI: Level Up button is null, cannot connect signal");
        }
        
        if (_levelDownButton != null)
        {
            _levelDownButton.Pressed += OnLevelDownButtonPressed;
            GD.Print("🖥️ GameUI: Level Down button signal connected");
        }
        else
        {
            GD.PrintErr("❌ GameUI: Level Down button is null, cannot connect signal");
        }
        
        GD.Print("🖥️ GameUI: Button signals connected (including test buttons)");
    }
    
    private void CreateGameDisplayPlayerVisual()
    {
        GD.Print("🖥️ GameUI: Creating player visual in GameDisplay...");
        
        // Get the GameDisplay content area
        var gameDisplayContent = GetNodeOrNull<VBoxContainer>("GridContainer/GameDisplay/GameDisplayContent");
        if (gameDisplayContent != null)
        {
            // Create a container for the player visual
            var playerVisualContainer = new HBoxContainer();
            playerVisualContainer.Name = "PlayerVisualContainer";
            
            // Create the player visual
            var playerRect = new ColorRect();
            playerRect.Name = "GameDisplayPlayerVisual";
            playerRect.Color = Colors.Blue;
            playerRect.CustomMinimumSize = new Vector2(40, 40);
            
            // Create a label for the player
            var playerLabel = new Label();
            playerLabel.Text = "Player";
            playerLabel.VerticalAlignment = VerticalAlignment.Center;
            
            // Add them to the container
            playerVisualContainer.AddChild(playerRect);
            playerVisualContainer.AddChild(playerLabel);
            
            // Add the container to GameDisplay (insert before test buttons)
            var testButtonContainer = gameDisplayContent.GetNodeOrNull("TestButtonContainer");
            if (testButtonContainer != null)
            {
                // Insert before the test button container
                var index = testButtonContainer.GetIndex();
                gameDisplayContent.AddChild(playerVisualContainer);
                gameDisplayContent.MoveChild(playerVisualContainer, index);
            }
            else
            {
                // Just add it at the end
                gameDisplayContent.AddChild(playerVisualContainer);
            }
            
            GD.Print("🖥️ GameUI: Player visual created in GameDisplay");
        }
        else
        {
            GD.PrintErr("❌ GameUI: Could not find GameDisplay content area for player visual!");
        }
    }
    
    // Placeholder signal handlers - GameManager will handle the actual logic
    private void OnAttackButtonPressed()
    {
        GD.Print("🖥️ GameUI: Attack button pressed (placeholder)");
        // TODO: Send signal to GameManager
    }
    
    private void OnRestButtonPressed()
    {
        GD.Print("🖥️ GameUI: Rest button pressed (placeholder)");
        // TODO: Send signal to GameManager  
    }
    
    // Test button signal handlers
    private void OnLevelUpButtonPressed()
    {
        GD.Print("� GameUI: Level Up button ACTUALLY PRESSED!");
        GD.Print("�🖥️ GameUI: Level Up button pressed - emitting signal");
        EmitSignal(SignalName.LevelUpRequested);
    }
    
    private void OnLevelDownButtonPressed()
    {
        GD.Print("🔥 GameUI: Level Down button ACTUALLY PRESSED!");
        GD.Print("🖥️ GameUI: Level Down button pressed - emitting signal");
        EmitSignal(SignalName.LevelDownRequested);
    }
    
    // Public methods for GameManager to update UI
    public void UpdatePlayerStats(PlayerData playerData)
    {
        if (_playerStatsPanel != null)
        {
            _playerStatsPanel.UpdatePlayerData(playerData);
            GD.Print("🖥️ GameUI: PlayerStatsPanel updated with new data");
        }
        else
        {
            GD.PrintErr("❌ GameUI: PlayerStatsPanel is null!");
        }
    }
    
    public void UpdateEnemyStats(string statsText)
    {
        if (_enemyStatsLabel != null)
            _enemyStatsLabel.Text = statsText;
    }
    
    public void UpdateCombatStats(string statsText)
    {
        if (_combatStatsLabel != null)
            _combatStatsLabel.Text = statsText;
    }
    
    public void UpdateInventory(string inventoryText)
    {
        if (_inventoryLabel != null)
            _inventoryLabel.Text = inventoryText;
    }
    
    public void UpdateUpgradeShop(string shopText)
    {
        if (_upgradeShopLabel != null)
            _upgradeShopLabel.Text = shopText;
    }
    
    public void UpdateRuneUpgrades(string runesText)
    {
        if (_runeUpgradesLabel != null)
            _runeUpgradesLabel.Text = runesText;
    }
    
    public void UpdateGameDisplay(string displayText)
    {
        // For now, we can add this text to the GameDisplay area
        // In the future, this might update specific game display elements
        GD.Print($"🖥️ GameUI: Game Display updated: {displayText}");
        // TODO: Find appropriate place to display this text in the GameDisplay panel
    }
    
    public void SetEnemyVisible(bool visible)
    {
        if (_enemyVisual != null)
            _enemyVisual.Visible = visible;
    }
    
    public void SetAttackButtonEnabled(bool enabled)
    {
        if (_attackButton != null)
            _attackButton.Disabled = !enabled;
    }
}
