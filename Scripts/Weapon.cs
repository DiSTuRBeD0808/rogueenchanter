using Godot;
using System;
using System.Collections.Generic;

public partial class Weapon : Resource
{
	[Export] public string WeaponName { get; set; } = "Basic Sword";
	[Export] public int BaseDamage { get; set; } = 1;
	[Export] public int MaxRuneCapacity { get; set; } = 2;
	[Export] public float BaseCooldown { get; set; } = 2.0f;
	
	public Godot.Collections.Array<Rune> EquippedRunes { get; set; } = new Godot.Collections.Array<Rune>();
	
	public int CurrentRuneCapacity => MaxRuneCapacity - EquippedRunes.Count;
	
	// Calculate total damage including rune bonuses
	public int TotalDamage
	{
		get
		{
			int totalDamage = BaseDamage;
			foreach (var rune in EquippedRunes)
			{
				if (rune.RuneType == RuneType.Damage)
					totalDamage += rune.Value;
			}
			return totalDamage;
		}
	}
	
	// Calculate total cooldown including rune reductions
	public float TotalCooldown
	{
		get
		{
			float totalCooldown = BaseCooldown;
			foreach (var rune in EquippedRunes)
			{
				if (rune.RuneType == RuneType.Speed)
					totalCooldown -= rune.Value * 0.1f; // Each speed rune reduces cooldown by 0.1s
			}
			return Mathf.Max(0.2f, totalCooldown); // Minimum 0.2s cooldown
		}
	}
	
	public bool CanAddRune()
	{
		return CurrentRuneCapacity > 0;
	}
	
	public bool AddRune(Rune rune)
	{
		if (CanAddRune())
		{
			EquippedRunes.Add(rune);
			return true;
		}
		return false;
	}
	
	public void RemoveRune(Rune rune)
	{
		EquippedRunes.Remove(rune);
	}
	
	public override string ToString()
	{
		return $"{WeaponName} ({TotalDamage} dmg, {TotalCooldown:F1}s cd, {CurrentRuneCapacity}/{MaxRuneCapacity} runes)";
	}
}
