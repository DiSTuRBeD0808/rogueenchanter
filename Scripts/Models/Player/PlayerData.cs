using System;

/// <summary>
/// Core player statistics and progression data
/// Pure C# class with no Godot dependencies
/// </summary>
public class PlayerData
{
    // Core Stats
    public int Level { get; set; } = 1;
    public int Health { get; set; } = 100;
    public int MaxHealth { get; set; } = 100;
    public int Experience { get; set; } = 0;
    public int ExperienceToNext { get; set; } = 100;
    public int Gold { get; set; } = 0;
    
    // Combat Stats
    public int Attack { get; set; } = 10;
    public int Defense { get; set; } = 5;
    public float CritChance { get; set; } = 0.1f;
    public float CritMultiplier { get; set; } = 2.0f;
    
    // Equipment bonuses (applied from items)
    public int BonusAttack { get; set; } = 0;
    public int BonusDefense { get; set; } = 0;
    public float BonusCritChance { get; set; } = 0.0f;
    
    // Calculated properties
    public int TotalAttack => Attack + BonusAttack;
    public int TotalDefense => Defense + BonusDefense;
    public float TotalCritChance => Math.Min(Math.Max(CritChance + BonusCritChance, 0.0f), 1.0f);
    
    // TODO: Add signal support when implementing reactive UI
    // TODO: Add save/load functionality
    // TODO: Add validation methods
}
