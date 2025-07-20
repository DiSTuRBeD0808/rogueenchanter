using Godot;
using RogueEnchanter.Systems;
using RogueEnchanter.Models.Enums;

/// <summary>
/// UI System - Manages all user interface elements
/// Minimal starting point - features will be implemented step by step
/// </summary>
public partial class UISystem : Node
{
    public override void _Ready()
    {
        DebugManager.Log(DebugCategory.UI_General, "UISystem: Ready for development", DebugLevel.Info);
    }
    
    // TODO: Implement UI functionality
    // public void ShowGameUI() { }
    // public void ShowPauseMenu() { }
    // public void RefreshPlayerDisplay(PlayerData playerData) { }
    // etc.
}
