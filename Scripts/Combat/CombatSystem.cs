using Godot;
using RogueEnchanter.Systems;
using RogueEnchanter.Models.Enums;

/// <summary>
/// Combat System - Handles all combat logic
/// Minimal starting point - features will be implemented step by step
/// </summary>
public partial class CombatSystem : Node
{
    public override void _Ready()
    {
        DebugManager.Log(DebugCategory.Combat, "CombatSystem: Ready for development", DebugLevel.Info);
    }
    
    // TODO: Implement combat functionality
    // public void EnableCombat() { }
    // public void DisableCombat() { }
    // public void ProcessPlayerAttack() { }
    // etc.
}
