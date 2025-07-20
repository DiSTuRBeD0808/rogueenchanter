using Godot;
using RogueEnchanter.Systems;
using RogueEnchanter.Models.Enums;

/// <summary>
/// Inventory System - Handles items, equipment, and gold
/// Minimal starting point - features will be implemented step by step
/// </summary>
public partial class InventorySystem : Node
{
    public override void _Ready()
    {
        DebugManager.Log(DebugCategory.Inventory, "InventorySystem: Ready for development", DebugLevel.Info);
    }
    
    // TODO: Implement inventory functionality
    // public void AddItem(ItemData item) { }
    // public void EquipItem(ItemData item) { }
    // public void ProcessEnemyLoot(EnemyData enemy) { }
    // etc.
}
