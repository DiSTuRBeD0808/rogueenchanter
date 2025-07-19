using Godot;

namespace RogueEnchanter.Models
{
    /// <summary>
    /// Enemy data structure with all combat-related information
    /// Simple C# class for now - Godot integration will be added later
    /// </summary>
    public class EnemyData
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public EnemyType Type { get; set; }
        
        // Combat Stats
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public float CritChance { get; set; } = 0.05f;
        
        // Rewards
        public int ExperienceReward { get; set; }
        public int GoldReward { get; set; }
        public float ItemDropChance { get; set; } = 0.3f;
        
        // AI Behavior
        public float AttackSpeed { get; set; } = 1.0f; // Attacks per second
        public bool IsElite { get; set; } = false;
        
        public EnemyData() { }
        
        public EnemyData(int level, EnemyType type = EnemyType.Melee)
        {
            Level = level;
            Type = type;
            Name = $"Level {level} Enemy"; // Simple name for now
            
            // Basic stats
            MaxHealth = 50 + (level * 25);
            Health = MaxHealth;
            Attack = 5 + (level * 3);
            Defense = 2 + (level * 1);
            
            // Basic rewards
            ExperienceReward = 25 + (level * 10);
            GoldReward = 10 + (level * 5);
        }
        
        // TODO: Implement type-specific stat generation
        // TODO: Implement name generation system
        // TODO: Add combat methods
    }
}
