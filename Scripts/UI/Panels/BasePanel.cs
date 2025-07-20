using Godot;

/// <summary>
/// Base class for all UI panels in the game
/// Provides common functionality and signal patterns for panel management
/// </summary>
public abstract partial class BasePanel : Control
{
    /// <summary>
    /// Emitted when a panel wants to request an action from the GameManager
    /// action: The action type (e.g., "buy_item", "attack_enemy", "close_panel")
    /// data: Additional data for the action (item ID, damage amount, etc.)
    /// </summary>
    [Signal] public delegate void PanelActionRequestedEventHandler(string action, Variant data);
    
    /// <summary>
    /// Emitted when a panel wants to change its visibility or request panel switching
    /// panelAction: "show", "hide", "replace"
    /// targetPanel: The panel type to switch to (for replace actions)
    /// </summary>
    [Signal] public delegate void PanelVisibilityRequestedEventHandler(string panelAction, string targetPanel = "");
    
    // Protected fields for derived classes
    protected bool _isInitialized = false;
    
    public override void _Ready()
    {
        GD.Print($"🎯 {GetType().Name}: Initializing panel...");
        
        // Initialize the panel
        Initialize();
        
        // Connect internal signals
        ConnectSignals();
        
        _isInitialized = true;
        GD.Print($"🎯 {GetType().Name}: Panel ready!");
    }
    
    /// <summary>
    /// Initialize panel-specific UI elements and data
    /// Called during _Ready() - override in derived classes
    /// </summary>
    public abstract void Initialize();
    
    /// <summary>
    /// Update the panel display with current data
    /// Called by GameUI when data changes - override in derived classes
    /// </summary>
    public abstract void UpdateDisplay();
    
    /// <summary>
    /// Connect panel-specific signals
    /// Called during _Ready() - override in derived classes if needed
    /// </summary>
    protected virtual void ConnectSignals()
    {
        // Base implementation - can be overridden
        GD.Print($"🎯 {GetType().Name}: Base signal connections (none)");
    }
    
    /// <summary>
    /// Show this panel with custom panel logic
    /// Uses Godot's base Show() method + custom panel behavior
    /// </summary>
    public virtual void ShowPanel()
    {
        Show(); // Use Godot's base method
        OnPanelShown();
    }
    
    /// <summary>
    /// Hide this panel with custom panel logic
    /// Uses Godot's base Hide() method + custom panel behavior
    /// </summary>
    public virtual void HidePanel()
    {
        Hide(); // Use Godot's base method
        OnPanelHidden();
    }
    
    /// <summary>
    /// Called when panel is shown - override for custom show behavior
    /// </summary>
    protected virtual void OnPanelShown()
    {
        GD.Print($"🎯 {GetType().Name}: Panel shown");
    }
    
    /// <summary>
    /// Called when panel is hidden - override for custom hide behavior
    /// </summary>
    protected virtual void OnPanelHidden()
    {
        GD.Print($"🎯 {GetType().Name}: Panel hidden");
    }
    
    /// <summary>
    /// Request an action from the GameManager
    /// </summary>
    protected void RequestAction(string action, Variant data = default)
    {
        GD.Print($"🎯 {GetType().Name}: Requesting action '{action}' with data: {data}");
        EmitSignal(SignalName.PanelActionRequested, action, data);
    }
    
    /// <summary>
    /// Request a visibility change for this or another panel
    /// </summary>
    protected void RequestVisibilityChange(string panelAction, string targetPanel = "")
    {
        GD.Print($"🎯 {GetType().Name}: Requesting visibility change '{panelAction}' -> '{targetPanel}'");
        EmitSignal(SignalName.PanelVisibilityRequested, panelAction, targetPanel);
    }
    
    /// <summary>
    /// Safely get a child node with error checking
    /// </summary>
    protected T GetChildNode<T>(string path) where T : Node
    {
        var node = GetNode<T>(path);
        if (node == null)
        {
            GD.PrintErr($"❌ {GetType().Name}: Failed to find node at path '{path}'");
        }
        return node;
    }
    
    /// <summary>
    /// Check if the panel is properly initialized
    /// </summary>
    public bool IsInitialized => _isInitialized;
}
