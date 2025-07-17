using Godot;
using System;
using System.Collections.Generic;

public static class LootSystem
{
	private static Random random = new Random();
	
	// Chance for weapon drop (percentage)
	public static int WeaponDropChance = 25; // 25% chance
	
	// Generate a random weapon with scaling based on player level
	public static Weapon GenerateRandomWeapon(int playerLevel)
	{
		var weapon = new Weapon();
		
		// Generate weapon name
		string[] weaponTypes = { "Sword", "Blade", "Dagger", "Axe", "Mace", "Spear" };
		string[] weaponAdjectives = { "Sharp", "Ancient", "Mystical", "Enchanted", "Cursed", "Divine", "Rusted", "Gleaming" };
		
		string adjective = weaponAdjectives[random.Next(weaponAdjectives.Length)];
		string type = weaponTypes[random.Next(weaponTypes.Length)];
		weapon.WeaponName = $"{adjective} {type}";
		
		// Scale weapon stats with player level
		weapon.BaseDamage = 1 + random.Next(0, playerLevel); // 1 to playerLevel damage
		weapon.MaxRuneCapacity = random.Next(1, 4); // 1-3 rune slots
		weapon.BaseCooldown = 1.5f + (random.Next(0, 3) * 0.5f); // 1.5f to 2.5f seconds
		
		return weapon;
	}
	
	// Check if a weapon should drop
	public static bool ShouldDropWeapon()
	{
		return random.Next(0, 100) < WeaponDropChance;
	}
	
	// Generate 4 random runes for drafting
	public static List<Rune> GenerateRuneDraft(int playerLevel)
	{
		var runes = new List<Rune>();
		
		// Define available rune types (excluding Critical and Explosive for now)
		var availableRuneTypes = new RuneType[] 
		{ 
			RuneType.Damage, 
			RuneType.Speed, 
			RuneType.Vampiric, 
			RuneType.ArmorPiercing 
		};
		
		for (int i = 0; i < 4; i++)
		{
			var randomType = availableRuneTypes[random.Next(availableRuneTypes.Length)];
			int value = CalculateRuneValue(randomType, playerLevel);
			runes.Add(new Rune(randomType, value));
		}
		
		return runes;
	}
	
	private static int CalculateRuneValue(RuneType type, int playerLevel)
	{
		// Scale rune power with player level
		int baseValue = 1 + (playerLevel - 1) / 2; // Slow scaling
		
		switch (type)
		{
			case RuneType.Damage:
				return baseValue + random.Next(0, 2); // +1 to +3 damage at level 1
			case RuneType.Speed:
				return Math.Max(1, baseValue); // +1 to +2 speed levels
			case RuneType.Critical:
				return (baseValue * 5) + random.Next(0, 6); // 5-10% crit chance at level 1
			case RuneType.Vampiric:
				return baseValue; // 1-2 heal per attack
			case RuneType.Explosive:
				return baseValue; // 1-2 area damage
			case RuneType.ArmorPiercing:
				return baseValue; // 1-2 armor piercing
			default:
				return baseValue;
		}
	}
}
