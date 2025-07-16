using Godot;
using System;
using System.Collections.Generic;

public partial class Inventory : Resource
{
	[Export] public Godot.Collections.Array<Weapon> Weapons { get; set; } = new Godot.Collections.Array<Weapon>();
	[Export] public int EquippedWeaponIndex { get; set; } = 0;
	[Export] public int SpiritPoints { get; set; } = 0;
	[Export] public int Gold { get; set; } = 10; // Start with some gold
	
	public Weapon EquippedWeapon 
	{
		get 
		{
			if (Weapons.Count == 0)
			{
				// Create a default starter weapon
				var starterWeapon = new Weapon();
				starterWeapon.WeaponName = "Rusty Blade";
				starterWeapon.BaseDamage = 1;
				starterWeapon.MaxRuneCapacity = 1;
				starterWeapon.BaseCooldown = 2.0f;
				Weapons.Add(starterWeapon);
			}
			
			if (EquippedWeaponIndex >= 0 && EquippedWeaponIndex < Weapons.Count)
				return Weapons[EquippedWeaponIndex];
			
			return Weapons[0];
		}
	}
	
	public void AddWeapon(Weapon weapon)
	{
		Weapons.Add(weapon);
		GD.Print($"Added weapon to inventory: {weapon}");
	}
	
	public void EquipWeapon(int index)
	{
		if (index >= 0 && index < Weapons.Count)
		{
			EquippedWeaponIndex = index;
			GD.Print($"Equipped weapon: {EquippedWeapon}");
		}
	}
	
	public void AddSpiritPoints(int amount)
	{
		SpiritPoints += amount;
		GD.Print($"Gained {amount} Spirit Points! Total: {SpiritPoints}");
	}
	
	public bool SpendSpiritPoints(int amount)
	{
		if (SpiritPoints >= amount)
		{
			SpiritPoints -= amount;
			return true;
		}
		return false;
	}
	
	public bool CanAffordRuneDraft()
	{
		return SpiritPoints >= GetRuneDraftCost();
	}
	
	public int GetRuneDraftCost()
	{
		return 5; // Cost 5 spirit points to draft runes
	}
	
	public void AddGold(int amount)
	{
		Gold += amount;
		GD.Print($"Gained {amount} Gold! Total: {Gold}");
	}
	
	public bool SpendGold(int amount)
	{
		if (Gold >= amount)
		{
			Gold -= amount;
			return true;
		}
		return false;
	}
	
	public int GetWeaponSellValue(Weapon weapon)
	{
		// Base sell value is weapon damage + rune count * 2
		int baseValue = weapon.BaseDamage + weapon.EquippedRunes.Count * 2;
		return Math.Max(1, baseValue); // Minimum 1 gold
	}
	
	public bool SellWeapon(int weaponIndex)
	{
		if (weaponIndex >= 0 && weaponIndex < Weapons.Count && Weapons.Count > 1)
		{
			// Can't sell if it's the only weapon
			var weapon = Weapons[weaponIndex];
			int sellValue = GetWeaponSellValue(weapon);
			
			// If selling equipped weapon, equip the first available weapon
			if (weaponIndex == EquippedWeaponIndex)
			{
				EquippedWeaponIndex = weaponIndex == 0 ? 1 : 0;
			}
			// Adjust equipped index if needed
			else if (weaponIndex < EquippedWeaponIndex)
			{
				EquippedWeaponIndex--;
			}
			
			Weapons.RemoveAt(weaponIndex);
			AddGold(sellValue);
			GD.Print($"Sold {weapon.WeaponName} for {sellValue} gold");
			return true;
		}
		return false;
	}
}
