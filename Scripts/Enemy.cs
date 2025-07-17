using Godot;
using System;

public partial class Enemy : Node2D
{
	[Export] public int BaseMaxHp = 5;
	[Export] public int BaseDamage = 3;
	[Export] public int BaseArmor = 0;
	[Export] public float BaseAttackCooldown = 3.0f;
	[Export] public float BaseHpRegen = 0.5f;
	
	public int MaxHp;
	public int CurrentHp;
	public int Damage;
	public int Armor;
	public float AttackCooldown;
	public float HpRegen;
	public int PlayerLevel;
	
	private Label healthLabel;
	private Timer attackTimer;
	private Timer hpRegenTimer;

	public Enemy()
	{
		// Default constructor for editor use
		PlayerLevel = 1;
		InitializeStats();
	}

	public Enemy(int playerLevel)
	{
		PlayerLevel = playerLevel;
		InitializeStats();
	}
	
	private void InitializeStats()
	{
		// Scale stats based on player level
		// HP scales more aggressively now: base + (level - 1) * 4
		MaxHp = BaseMaxHp + (PlayerLevel - 1) * 4;
		
		// Damage scales moderately: base + (level - 1) * 1
		Damage = BaseDamage + (PlayerLevel - 1) * 1;
		
		// Armor starts appearing at level 3: 0 for levels 1-2, then (level - 2) armor
		Armor = PlayerLevel > 2 ? BaseArmor + (PlayerLevel - 3) : 0;
		
		// Attack cooldown decreases slightly with level (enemies get faster)
		AttackCooldown = Mathf.Max(1.5f, BaseAttackCooldown - (PlayerLevel - 1) * 0.1f);
		
		// HP regen starts very low but increases exponentially with level
		// Formula: BaseHpRegen * (1.3^(level-1)) * 0.3 - gives very little regen early, exponential growth later
		HpRegen = BaseHpRegen * Mathf.Pow(1.3f, PlayerLevel - 1) * 0.3f;
	}

	public override void _Ready()
	{
		// If stats weren't initialized, do it now
		if (MaxHp == 0)
			InitializeStats();
			
		CurrentHp = MaxHp;
		healthLabel = GetNodeOrNull<Label>("HealthLabel");
		if (healthLabel == null)
		{
			GD.PrintErr("HealthLabel not found! Make sure you have a Label node named 'HealthLabel' as a child of Enemy.");
		}
		UpdateHealthDisplay();
		
		// Set up attack timer
		attackTimer = new Timer();
		attackTimer.Name = "AttackTimer";
		attackTimer.WaitTime = AttackCooldown;
		attackTimer.Autostart = true;
		attackTimer.Timeout += OnAttackPlayer;
		AddChild(attackTimer);
		
		// Set up HP regeneration timer
		hpRegenTimer = new Timer();
		hpRegenTimer.Name = "HpRegenTimer";
		hpRegenTimer.WaitTime = 1.0f; // Tick every second
		hpRegenTimer.Autostart = true;
		hpRegenTimer.Timeout += OnHpRegen;
		AddChild(hpRegenTimer);
	}

	public void TakeDamage(int damage, int armorPiercing = 0)
	{
		// Calculate effective armor (reduced by armor piercing)
		int effectiveArmor = Mathf.Max(0, Armor - armorPiercing);
		
		// Apply armor reduction (simple: each point of armor reduces damage by 1, minimum 1 damage)
		int finalDamage = Mathf.Max(1, damage - effectiveArmor);
		
		CurrentHp -= finalDamage;
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
			player.CallDeferred("LogEnemyDamage", finalDamage, CurrentHp, MaxHp);
			
			// Update enemy stat panel - get GameManager directly from tree
			var gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameManager;
			if (gameManager == null)
			{
				// Fallback: try to get it through the player's parent
				gameManager = player.GetParent() as GameManager;
			}
			gameManager?.UpdateEnemyStatPanel(this);
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
			string armorText = Armor > 0 ? $" [{Armor} armor]" : "";
			healthLabel.Text = $"{CurrentHp}/{MaxHp}{armorText}";
		}
	}
	
	private void OnAttackPlayer()
	{
		var player = GetTree().GetFirstNodeInGroup("player") as Player;
		if (player != null && IsInstanceValid(player) && !player.IsDead)
		{
			player.TakeDamage(Damage, 0); // Enemies don't have armor piercing for now
			
			// Visual feedback for enemy attack
			var colorRect = GetNodeOrNull<ColorRect>("ColorRect");
			if (colorRect != null)
			{
				colorRect.Modulate = Colors.Yellow;
				GetTree().CreateTimer(0.1f).Timeout += () => {
					if (IsInstanceValid(colorRect))
						colorRect.Modulate = Colors.White;
				};
			}
		}
	}
	
	private void OnHpRegen()
	{
		if (CurrentHp < MaxHp && CurrentHp > 0) // Only regen if alive and not at full HP
		{
			int regenAmount = Mathf.RoundToInt(HpRegen);
			CurrentHp = Mathf.Min(MaxHp, CurrentHp + regenAmount);
			UpdateHealthDisplay();
			
			// Update enemy stat panel when HP regens
			var gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameManager;
			gameManager?.UpdateEnemyStatPanel(this);
		}
	}
}
