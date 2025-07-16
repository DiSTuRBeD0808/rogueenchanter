using Godot;
using System;

public partial class Enemy : Node2D
{
	[Export] public int BaseMaxHp = 5;
	public int MaxHp;
	public int CurrentHp;
	private Label healthLabel;

	public Enemy()
	{
		// Default constructor for editor use
		MaxHp = BaseMaxHp;
	}

	public Enemy(int playerLevel)
	{
		// Scale HP based on player level: base HP + (level - 1) * 2
		MaxHp = BaseMaxHp + (playerLevel - 1) * 2;
	}

	public override void _Ready()
	{
		// If MaxHp wasn't set by constructor, use base value
		if (MaxHp == 0)
			MaxHp = BaseMaxHp;
			
		CurrentHp = MaxHp;
		healthLabel = GetNodeOrNull<Label>("HealthLabel");
		if (healthLabel == null)
		{
			GD.PrintErr("HealthLabel not found! Make sure you have a Label node named 'HealthLabel' as a child of Enemy.");
		}
		UpdateHealthDisplay();
	}

	public void TakeDamage(int amount)
	{
		CurrentHp -= amount;
		UpdateHealthDisplay();

		// Flash red
		var colorRect = GetNodeOrNull<ColorRect>("ColorRect");
		if (colorRect != null)
		{
			colorRect.Modulate = new Color(1, 0.5f, 0.5f);
			GetTree().CreateTimer(0.1f).Timeout += () => {
				if (IsInstanceValid(colorRect))
					colorRect.Modulate = new Color(1, 0, 0);
			};
		}

		// Find player to log damage (if combat log exists)
		var player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (player != null)
		{
			// Use reflection or a public method to access the combat log
			player.CallDeferred("LogEnemyDamage", amount, CurrentHp, MaxHp);
		}

		if (CurrentHp <= 0)
		{
			QueueFree();
		}
	}

	private void UpdateHealthDisplay()
	{
		if (healthLabel != null)
		{
			healthLabel.Text = $"{CurrentHp}/{MaxHp}";
		}
	}
}
