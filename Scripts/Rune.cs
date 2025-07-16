using Godot;
using System;

public enum RuneType
{
	Damage,     // Increases attack damage
	Speed,      // Reduces attack cooldown
	Critical,   // Adds critical hit chance
	Vampiric,   // Life steal on attack
	Explosive   // Area damage
}

public partial class Rune : Resource
{
	[Export] public string RuneName { get; set; }
	[Export] public RuneType RuneType { get; set; }
	[Export] public int Value { get; set; }
	[Export] public string Description { get; set; }
	[Export] public Color RuneColor { get; set; } = Colors.White;
	
	public Rune()
	{
		// Default constructor for Godot
	}
	
	public Rune(RuneType type, int value)
	{
		RuneType = type;
		Value = value;
		GenerateNameAndDescription();
		SetRuneColor();
	}
	
	private void GenerateNameAndDescription()
	{
		switch (RuneType)
		{
			case RuneType.Damage:
				RuneName = $"Rune of Power +{Value}";
				Description = $"Increases attack damage by {Value}";
				break;
			case RuneType.Speed:
				RuneName = $"Rune of Swiftness +{Value}";
				Description = $"Reduces attack cooldown by {Value * 0.1f:F1} seconds";
				break;
			case RuneType.Critical:
				RuneName = $"Rune of Precision +{Value}%";
				Description = $"Adds {Value}% critical hit chance";
				break;
			case RuneType.Vampiric:
				RuneName = $"Rune of Vampirism +{Value}";
				Description = $"Heal {Value} HP on successful attack";
				break;
			case RuneType.Explosive:
				RuneName = $"Rune of Explosion +{Value}";
				Description = $"Deal {Value} damage to nearby enemies";
				break;
		}
	}
	
	private void SetRuneColor()
	{
		switch (RuneType)
		{
			case RuneType.Damage:
				RuneColor = Colors.OrangeRed;
				break;
			case RuneType.Speed:
				RuneColor = Colors.LightBlue;
				break;
			case RuneType.Critical:
				RuneColor = Colors.Gold;
				break;
			case RuneType.Vampiric:
				RuneColor = Colors.DarkRed;
				break;
			case RuneType.Explosive:
				RuneColor = Colors.Purple;
				break;
		}
	}
	
	public override string ToString()
	{
		return $"{RuneName}: {Description}";
	}
}
