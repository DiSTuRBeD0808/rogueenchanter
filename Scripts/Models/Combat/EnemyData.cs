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
        
        // Energy System
        public float Energy { get; set; } = 0.0f;
        public float MaxEnergy { get; set; } = 100.0f;
        public float EnergyGainRate { get; set; } = 1.0f; // Energy per second
        
        public EnemyData() { }
        
        public EnemyData(int level, EnemyType type = EnemyType.Melee)
        {
            Level = level;
            Type = type;
            Name = GenerateEnemyName(level, type);
            
            // Scaled stats with exponential growth for challenge
            MaxHealth = (int)(50 + (level * 25) + (level * level * 2.5f));
            Health = MaxHealth;
            Attack = (int)(5 + (level * 3) + (level * level * 0.5f));
            Defense = (int)(2 + (level * 1) + (level * level * 0.2f));
            
            // Energy system initialization
            Energy = 0.0f;
            MaxEnergy = 100.0f;
            EnergyGainRate = Mathf.Min(1.0f, 0.3f + (level * 0.1f)); // Caps at 1.0/second
            
            // Attack speed correlates with energy gain rate
            AttackSpeed = EnergyGainRate;
            
            // Scaled rewards
            ExperienceReward = (int)(25 + (level * 10) + (level * level * 1.5f));
            GoldReward = (int)(10 + (level * 5) + (level * level * 0.8f));
            
            // Elite chance increases with level
            if (level >= 5 && GD.Randf() < (level * 0.02f))
            {
                IsElite = true;
                ApplyEliteModifiers();
            }
        }
        
        /// <summary>
        /// Generate enemy name based on level and type
        /// </summary>
        private string GenerateEnemyName(int level, EnemyType type)
        {
            string[] prefixes = { "", "Angry ", "Fierce ", "Savage ", "Ancient ", "Corrupted " };
            string[] names = type switch
            {
                EnemyType.Melee => new[] { "Goblin", "Orc", "Troll", "Warrior", "Berserker" },
                EnemyType.Ranged => new[] { "Archer", "Hunter", "Sniper", "Marksman", "Scout" },
                EnemyType.Magic => new[] { "Mage", "Wizard", "Sorcerer", "Warlock", "Necromancer" },
                _ => new[] { "Enemy", "Foe", "Monster", "Creature", "Beast" }
            };
            
            string prefix = level > 10 ? prefixes[GD.RandRange(1, prefixes.Length - 1)] : prefixes[0];
            string name = names[GD.RandRange(0, names.Length - 1)];
            
            return $"{prefix}{name}";
        }
        
        /// <summary>
        /// Apply elite enemy modifiers
        /// </summary>
        private void ApplyEliteModifiers()
        {
            Name = "Elite " + Name;
            MaxHealth = (int)(MaxHealth * 1.5f);
            Health = MaxHealth;
            Attack = (int)(Attack * 1.3f);
            Defense = (int)(Defense * 1.2f);
            EnergyGainRate = Mathf.Min(1.0f, EnergyGainRate * 1.2f);
            AttackSpeed = EnergyGainRate;
            
            // Better rewards
            ExperienceReward = (int)(ExperienceReward * 1.5f);
            GoldReward = (int)(GoldReward * 1.4f);
            ItemDropChance = Mathf.Min(0.8f, ItemDropChance * 1.5f);
        }
        
        /// <summary>
        /// Take damage and return true if enemy dies
        /// </summary>
        public bool TakeDamage(int damage)
        {
            Health = Mathf.Max(0, Health - damage);
            return Health <= 0;
        }
        
        /// <summary>
        /// Reset energy to 0 (after attacking)
        /// </summary>
        public void ResetEnergy()
        {
            Energy = 0.0f;
        }
        
        /// <summary>
        /// Add energy based on time delta and energy gain rate
        /// </summary>
        public void AddEnergy(float deltaTime)
        {
            Energy = Mathf.Min(MaxEnergy, Energy + (EnergyGainRate * deltaTime * 60.0f)); // 60 for per-second rate
        }
        
        /// <summary>
        /// Check if enemy has enough energy to attack
        /// </summary>
        public bool CanAttack()
        {
            return Energy >= MaxEnergy;
        }
    }
}
