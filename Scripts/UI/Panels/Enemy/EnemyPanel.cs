using Godot;
using RogueEnchanter.Systems;
using RogueEnchanter.Models.Enums;
using RogueEnchanter.Models;

/// <summary>
/// Enemy display panel for combat mode
/// Shows enemy stats, health, energy bar, and visual representation
/// </summary>
public partial class EnemyPanel : BasePanel
{
    // UI Element References
    private Label _enemyNameLabel;
    private Label _enemyLevelLabel;
    private Label _enemyHealthLabel;
    private ProgressBar _enemyHealthBar;
    private ProgressBar _enemyEnergyBar;
    private ColorRect _enemyVisual;
    private Label _enemyStatsLabel;
    
    // Data Reference
    private EnemyData _currentEnemyData;
    
    // Energy tracking
    private float _currentEnergy = 0.0f;
    private bool _isInCombat = false;

    public override void Initialize()
    {
        DebugManager.Log(DebugCategory.UI_Enemy, "Initializing enemy display panel...", DebugLevel.Info);
        
        // Get all UI element references
        GetUIReferences();
        
        // Set initial state (no enemy)
        SetNoEnemyState();
        
        DebugManager.Log(DebugCategory.UI_Enemy, "Enemy panel initialization complete", DebugLevel.Info);
    }
    
    private void GetUIReferences()
    {
        // Try to find existing label first (for compatibility with current layout)
        var mainLabel = GetNodeOrNull<Label>("EnemyStatsLabel");
        
        if (mainLabel == null)
        {
            // No existing label found, create our own layout
            DebugManager.Log(DebugCategory.UI_Enemy, "No existing label found, creating new enemy panel layout...", DebugLevel.Verbose);
            CreateEnemyPanelLayout();
        }
        else
        {
            // Use existing label for now, enhance later
            _enemyStatsLabel = mainLabel;
            DebugManager.Log(DebugCategory.UI_Enemy, "Using existing EnemyStatsLabel for now", DebugLevel.Verbose);
        }
        
        DebugManager.Log(DebugCategory.UI_Enemy, "Enemy panel UI references set up", DebugLevel.Verbose);
    }
    
    private void CreateEnemyPanelLayout()
    {
        // Create main container
        var mainContainer = new VBoxContainer();
        mainContainer.Name = "EnemyPanelContent";
        mainContainer.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        
        // Enemy name and level header
        var headerContainer = new HBoxContainer();
        
        _enemyNameLabel = new Label();
        _enemyNameLabel.Name = "EnemyNameLabel";
        _enemyNameLabel.Text = "No Enemy";
        _enemyNameLabel.AddThemeStyleboxOverride("normal", new StyleBoxFlat());
        
        _enemyLevelLabel = new Label();
        _enemyLevelLabel.Name = "EnemyLevelLabel";
        _enemyLevelLabel.Text = "Lv. 0";
        _enemyLevelLabel.HorizontalAlignment = HorizontalAlignment.Right;
        
        headerContainer.AddChild(_enemyNameLabel);
        headerContainer.AddChild(_enemyLevelLabel);
        
        // Enemy visual
        _enemyVisual = new ColorRect();
        _enemyVisual.Name = "EnemyVisual";
        _enemyVisual.Color = Colors.Red;
        _enemyVisual.CustomMinimumSize = new Vector2(60, 60);
        _enemyVisual.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
        
        // Health bar with label
        var healthContainer = new VBoxContainer();
        
        _enemyHealthLabel = new Label();
        _enemyHealthLabel.Name = "EnemyHealthLabel";
        _enemyHealthLabel.Text = "HP: 0/0";
        _enemyHealthLabel.HorizontalAlignment = HorizontalAlignment.Center;
        
        _enemyHealthBar = new ProgressBar();
        _enemyHealthBar.Name = "EnemyHealthBar";
        _enemyHealthBar.ShowPercentage = false;
        _enemyHealthBar.CustomMinimumSize = new Vector2(100, 20);
        
        healthContainer.AddChild(_enemyHealthLabel);
        healthContainer.AddChild(_enemyHealthBar);
        
        // Energy bar with label
        var energyContainer = new VBoxContainer();
        
        var energyLabel = new Label();
        energyLabel.Text = "Energy";
        energyLabel.HorizontalAlignment = HorizontalAlignment.Center;
        
        _enemyEnergyBar = new ProgressBar();
        _enemyEnergyBar.Name = "EnemyEnergyBar";
        _enemyEnergyBar.ShowPercentage = false;
        _enemyEnergyBar.CustomMinimumSize = new Vector2(100, 15);
        _enemyEnergyBar.MaxValue = 100.0;
        _enemyEnergyBar.Value = 0.0;
        
        energyContainer.AddChild(energyLabel);
        energyContainer.AddChild(_enemyEnergyBar);
        
        // Fallback stats label for compatibility
        _enemyStatsLabel = new Label();
        _enemyStatsLabel.Name = "EnemyStatsLabel";
        _enemyStatsLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _enemyStatsLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _enemyStatsLabel.VerticalAlignment = VerticalAlignment.Center;
        
        // Add all components to main container
        mainContainer.AddChild(headerContainer);
        mainContainer.AddChild(_enemyVisual);
        mainContainer.AddChild(healthContainer);
        mainContainer.AddChild(energyContainer);
        mainContainer.AddChild(_enemyStatsLabel);
        
        // Add main container to panel
        AddChild(mainContainer);
        
        DebugManager.Log(DebugCategory.UI_Enemy, "Enemy panel layout created", DebugLevel.Verbose);
    }
    
    private void SetNoEnemyState()
    {
        // Set default "no enemy" state
        if (_enemyNameLabel != null) _enemyNameLabel.Text = "No Enemy";
        if (_enemyLevelLabel != null) _enemyLevelLabel.Text = "";
        if (_enemyHealthLabel != null) _enemyHealthLabel.Text = "HP: -/-";
        if (_enemyHealthBar != null) _enemyHealthBar.Value = 0;
        if (_enemyEnergyBar != null) _enemyEnergyBar.Value = 0;
        if (_enemyVisual != null) _enemyVisual.Visible = false;
        
        // Fallback label for current UI
        if (_enemyStatsLabel != null)
        {
            _enemyStatsLabel.Text = "ENEMY STATS\nNo enemy present\n(Enter combat to spawn enemy)";
        }
        
        _currentEnergy = 0.0f;
        _isInCombat = false;
    }
    
    public override void UpdateDisplay()
    {
        if (_currentEnemyData == null)
        {
            SetNoEnemyState();
            return;
        }
        
        // Update individual UI elements if they exist
        if (_enemyNameLabel != null) _enemyNameLabel.Text = _currentEnemyData.Name;
        if (_enemyLevelLabel != null) _enemyLevelLabel.Text = $"Lv. {_currentEnemyData.Level}";
        if (_enemyHealthLabel != null) _enemyHealthLabel.Text = $"HP: {_currentEnemyData.Health}/{_currentEnemyData.MaxHealth}";
        
        if (_enemyHealthBar != null)
        {
            _enemyHealthBar.MaxValue = _currentEnemyData.MaxHealth;
            _enemyHealthBar.Value = _currentEnemyData.Health;
        }
        
        if (_enemyVisual != null) _enemyVisual.Visible = true;
        
        // Update fallback stats label for compatibility
        if (_enemyStatsLabel != null)
        {
            _enemyStatsLabel.Text = BuildEnemyStatsText(_currentEnemyData);
        }
        
        DebugManager.Log(DebugCategory.UI_Enemy, "Enemy display updated", DebugLevel.Verbose);
    }
    
    /// <summary>
    /// Update the panel with new enemy data
    /// </summary>
    public void UpdateEnemyData(EnemyData enemyData)
    {
        _currentEnemyData = enemyData;
        
        if (_isInitialized)
        {
            UpdateDisplay();
        }
        else
        {
            DebugManager.Log(DebugCategory.UI_Enemy, "Panel not yet initialized, enemy data stored for later display", DebugLevel.Verbose);
        }
        
        DebugManager.Log(DebugCategory.UI_Enemy, $"Enemy data updated - {enemyData?.Name ?? "None"} Level {enemyData?.Level ?? 0}", DebugLevel.Verbose);
    }
    
    /// <summary>
    /// Update the enemy energy bar
    /// </summary>
    public void UpdateEnemyEnergy(float energy)
    {
        _currentEnergy = Mathf.Clamp(energy, 0.0f, 100.0f);
        
        if (_enemyEnergyBar != null)
        {
            _enemyEnergyBar.Value = _currentEnergy;
        }
        
        // Check if energy is full (ready to attack)
        if (_currentEnergy >= 100.0f && _isInCombat)
        {
            DebugManager.Log(DebugCategory.UI_Enemy, "Enemy energy full - ready to attack!", DebugLevel.Verbose);
        }
    }
    
    /// <summary>
    /// Set combat state for energy generation
    /// </summary>
    public void SetCombatState(bool inCombat)
    {
        _isInCombat = inCombat;
        
        if (!inCombat)
        {
            _currentEnergy = 0.0f;
            if (_enemyEnergyBar != null) _enemyEnergyBar.Value = 0.0f;
        }
        
        DebugManager.Log(DebugCategory.UI_Enemy, $"Enemy combat state set to: {inCombat}", DebugLevel.Verbose);
    }
    
    private string BuildEnemyStatsText(EnemyData enemyData)
    {
        return $"ENEMY STATS\n" +
               $"Name: {enemyData.Name}\n" +
               $"Level: {enemyData.Level}\n" +
               $"Health: {enemyData.Health}/{enemyData.MaxHealth}\n" +
               $"Attack: {enemyData.Attack}\n" +
               $"Energy: {_currentEnergy:F0}/100";
    }
    
    protected override void ConnectSignals()
    {
        base.ConnectSignals();
        
        // Enemy panel typically doesn't have interactive elements
        // but if we add buttons later (like "Flee Combat"), we'd connect them here
        
        DebugManager.Log(DebugCategory.UI_Enemy, "No signals to connect (display only)", DebugLevel.Verbose);
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
        
        // Reset energy when panel is hidden
        _currentEnergy = 0.0f;
        if (_enemyEnergyBar != null) _enemyEnergyBar.Value = 0.0f;
    }
    
    /// <summary>
    /// Get current enemy data (useful for other systems)
    /// </summary>
    public EnemyData GetCurrentEnemyData()
    {
        return _currentEnemyData;
    }
    
    /// <summary>
    /// Check if enemy data is loaded
    /// </summary>
    public bool HasEnemyData()
    {
        return _currentEnemyData != null;
    }
    
    /// <summary>
    /// Get current enemy energy level
    /// </summary>
    public float GetEnemyEnergy()
    {
        return _currentEnergy;
    }
}
