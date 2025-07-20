using Godot;

/// <summary>
/// Player statistics display panel
/// Manages and displays player level, health, attack, defense, gold, and other stats
/// </summary>
public partial class PlayerStatsPanel : BasePanel
{
    // UI Element References
    private Label _levelLabel;
    private Label _healthLabel;
    private Label _attackLabel;
    private Label _defenseLabel;
    private Label _goldLabel;
    private Label _experienceLabel;
    private Label _critChanceLabel;
    
    // Data Reference
    private PlayerData _currentPlayerData;
    
    public override void Initialize()
    {
        GD.Print("📊 PlayerStatsPanel: Initializing player stats display...");
        
        // Get all UI element references
        GetUIReferences();
        
        // Set initial placeholder text
        SetPlaceholderText();
        
        GD.Print("📊 PlayerStatsPanel: Initialization complete");
    }
    
    private void GetUIReferences()
    {
        // Try to find existing label first
        var mainLabel = GetNodeOrNull<Label>("PlayerStatsLabel");
        
        if (mainLabel == null)
        {
            // No existing label found, create our own
            GD.Print("📊 PlayerStatsPanel: No existing label found, creating new one...");
            
            mainLabel = new Label();
            mainLabel.Name = "PlayerStatsLabel";
            mainLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            mainLabel.VerticalAlignment = VerticalAlignment.Center;
            mainLabel.HorizontalAlignment = HorizontalAlignment.Center;
            
            // Make the label fill the entire panel
            mainLabel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            mainLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            mainLabel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            
            AddChild(mainLabel);
            
            GD.Print("📊 PlayerStatsPanel: Created new PlayerStatsLabel with full layout");
        }
        
        // Use the main label for all stats (simple approach for now)
        _levelLabel = mainLabel;
        _healthLabel = mainLabel;
        _attackLabel = mainLabel;
        _defenseLabel = mainLabel;
        _goldLabel = mainLabel;
        _experienceLabel = mainLabel;
        _critChanceLabel = mainLabel;
        
        GD.Print("📊 PlayerStatsPanel: UI references set up successfully");
    }
    
    private void SetPlaceholderText()
    {
        // Set initial placeholder text while no player data is available
        UpdateDisplayText("PLAYER STATS\n(Loading...)");
    }
    
    public override void UpdateDisplay()
    {
        if (_currentPlayerData == null)
        {
            SetPlaceholderText();
            return;
        }
        
        // Build the stats display string
        string statsText = BuildStatsText(_currentPlayerData);
        
        // Update the display
        UpdateDisplayText(statsText);
        
        GD.Print("📊 PlayerStatsPanel: Display updated");
    }
    
    /// <summary>
    /// Update the panel with new player data
    /// </summary>
    public void UpdatePlayerData(PlayerData playerData)
    {
        _currentPlayerData = playerData;
        
        if (_isInitialized)
        {
            UpdateDisplay();
        }
        
        GD.Print($"📊 PlayerStatsPanel: Player data updated - Level {playerData?.Level ?? 0}");
    }
    
    private string BuildStatsText(PlayerData playerData)
    {
        return $"PLAYER STATS\n" +
               $"Level: {playerData.Level}\n" +
               $"Health: {playerData.Health}/{playerData.MaxHealth}\n" +
               $"Attack: {playerData.TotalAttack}\n" +
               $"Defense: {playerData.TotalDefense}\n" +
               $"Gold: {playerData.Gold}\n" +
               $"Experience: {playerData.Experience}/{playerData.ExperienceToNext}\n" +
               $"Crit Chance: {(playerData.TotalCritChance * 100):F1}%";
    }
    
    private void UpdateDisplayText(string text)
    {
        // For now, update the main label
        // Later we can update individual labels for each stat
        if (_levelLabel != null)
        {
            _levelLabel.Text = text;
        }
    }
    
    protected override void ConnectSignals()
    {
        base.ConnectSignals();
        
        // Player stats panel typically doesn't have interactive elements
        // but if we add buttons later (like "View Details"), we'd connect them here
        
        GD.Print("📊 PlayerStatsPanel: No signals to connect (display only)");
    }
    
    protected override void OnPanelShown()
    {
        base.OnPanelShown();
        
        // Refresh display when panel becomes visible
        UpdateDisplay();
    }
    
    protected override void OnPanelHidden()
    {
        base.OnPanelHidden();
        
        // Could add any cleanup logic here if needed
    }
    
    /// <summary>
    /// Get current player data (useful for other systems)
    /// </summary>
    public PlayerData GetCurrentPlayerData()
    {
        return _currentPlayerData;
    }
    
    /// <summary>
    /// Check if player data is loaded
    /// </summary>
    public bool HasPlayerData()
    {
        return _currentPlayerData != null;
    }
}
