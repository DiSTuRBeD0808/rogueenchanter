using Godot;
using System;
using System.Collections.Generic;

public enum UpgradeType
{
	Damage,
	AttackSpeed,
	AutoAttack
}

public partial class ShopUpgrade : Resource
{
	[Export] public UpgradeType Type { get; set; }
	[Export] public string Name { get; set; }
	[Export] public string Description { get; set; }
	[Export] public int Level { get; set; } = 0;
	[Export] public int BasePrice { get; set; }
	[Export] public float PriceMultiplier { get; set; } = 1.5f;
	
	public int CurrentPrice => Mathf.RoundToInt(BasePrice * Mathf.Pow(PriceMultiplier, Level));
	
	public ShopUpgrade()
	{
		// Default constructor for Godot
	}
	
	public ShopUpgrade(UpgradeType type, string name, string description, int basePrice)
	{
		Type = type;
		Name = name;
		Description = description;
		BasePrice = basePrice;
	}
	
	public int GetCurrentCost()
	{
		return Mathf.RoundToInt(BasePrice * Mathf.Pow(1.5f, Level));
	}
	
	public string GetDisplayText()
	{
		switch (Type)
		{
			case UpgradeType.Damage:
				return $"🗡️ Weapon Mastery Lv.{Level} (+{Level} dmg) - {GetCurrentCost()}g";
			case UpgradeType.AttackSpeed:
				return $"⚡ Combat Training Lv.{Level} (-{Level * 0.1f}s cooldown) - {GetCurrentCost()}g";
			case UpgradeType.AutoAttack:
				if (Level == 0)
					return $"🤖 Auto-Attack System - {GetCurrentCost()}g";
				else
					return $"🤖 Auto-Attack System - PURCHASED";
			default:
				return $"Unknown Upgrade Lv.{Level} - {GetCurrentCost()}g";
		}
	}
	
	public string GetTooltipText()
	{
		switch (Type)
		{
			case UpgradeType.Damage:
				return $"Increases all weapon damage by 1 per level.\nNext level: +{Level + 1} total damage\nCost increases by 50% each level.";
			case UpgradeType.AttackSpeed:
				return $"Reduces attack cooldown by 0.1 seconds per level.\nNext level: -{(Level + 1) * 0.1f}s total reduction\nCost increases by 50% each level.";
			case UpgradeType.AutoAttack:
				if (Level == 0)
					return $"Enables automatic attacking when off cooldown.\nOne-time purchase that adds a checkbox next to attack button.";
				else
					return $"Auto-attack system already purchased!\nUse the checkbox next to the attack button.";
			default:
				return "Unknown upgrade effect.";
		}
	}
}

public partial class Shop : Resource
{
	[Export] public Godot.Collections.Array<ShopUpgrade> Upgrades { get; set; } = new Godot.Collections.Array<ShopUpgrade>();
	
	public Shop()
	{
		InitializeUpgrades();
	}
	
	private void InitializeUpgrades()
	{
		// Damage upgrade
		var damageUpgrade = new ShopUpgrade(
			UpgradeType.Damage,
			"Weapon Mastery",
			"Increases base damage by 1 per level",
			15
		);
		Upgrades.Add(damageUpgrade);
		
		// Attack Speed upgrade
		var speedUpgrade = new ShopUpgrade(
			UpgradeType.AttackSpeed,
			"Combat Training",
			"Reduces attack cooldown by 0.1s per level",
			20
		);
		Upgrades.Add(speedUpgrade);
		
		// Auto Attack upgrade (one-time purchase)
		var autoAttackUpgrade = new ShopUpgrade(
			UpgradeType.AutoAttack,
			"Auto-Attack System",
			"Automatically attacks when off cooldown",
			100
		);
		Upgrades.Add(autoAttackUpgrade);
	}
	
	public bool CanAffordUpgrade(ShopUpgrade upgrade, int playerGold)
	{
		return playerGold >= upgrade.GetCurrentCost();
	}
	
	public bool PurchaseUpgrade(ShopUpgrade upgrade, Inventory playerInventory)
	{
		if (CanAffordUpgrade(upgrade, playerInventory.Gold))
		{
			playerInventory.SpendGold(upgrade.CurrentPrice);
			upgrade.Level++;
			GD.Print($"Purchased {upgrade.Name} level {upgrade.Level} for {upgrade.CurrentPrice - Mathf.RoundToInt(upgrade.BasePrice * Mathf.Pow(upgrade.PriceMultiplier, upgrade.Level - 2))} gold");
			return true;
		}
		return false;
	}
}
