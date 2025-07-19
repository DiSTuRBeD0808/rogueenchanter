/// <summary>
/// Item data structure with all item-related information
/// Pure C# class with no Godot dependencies
/// </summary>
public class ItemData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ItemType Type { get; set; }
    public ItemRarity Rarity { get; set; }
    public EquipmentSlot Slot { get; set; }
    
    // Stats
    public int AttackBonus { get; set; } = 0;
    public int DefenseBonus { get; set; } = 0;
    public int HealthBonus { get; set; } = 0;
    public float CritChanceBonus { get; set; } = 0.0f;
    
    // Economic
    public int Value { get; set; }
    public int StackSize { get; set; } = 1;
    
    // Flags
    public bool IsEquippable => Type == ItemType.Weapon || Type == ItemType.Armor || Type == ItemType.Accessory;
    public bool IsConsumable => Type == ItemType.Consumable;
    public bool IsStackable => StackSize > 1;
    
    public ItemData() { }
    
    public ItemData(string name, ItemType type, ItemRarity rarity = ItemRarity.Common)
    {
        Name = name;
        Type = type;
        Rarity = rarity;
        
        // Basic stat generation for now
        int rarityMultiplier = (int)rarity + 1;
        AttackBonus = type == ItemType.Weapon ? 5 * rarityMultiplier : 0;
        DefenseBonus = type == ItemType.Armor ? 3 * rarityMultiplier : 0;
        Value = 10 * rarityMultiplier;
    }
    
    // TODO: Implement full stat generation system
    // TODO: Add rarity color methods
    // TODO: Add stat text generation
}
