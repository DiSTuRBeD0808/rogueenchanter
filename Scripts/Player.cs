using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public int Experience = 0;
	[Export] public int Level = 1;
	[Export] public int ExperienceToNextLevel = 10;
	
	public Inventory PlayerInventory = new Inventory();
	public Shop PlayerShop = new Shop();
	
	private Enemy currentEnemy;
	
	private Button attackButton;
	private Timer attackCooldownTimer;
	private RichTextLabel combatLog;
	private Label experienceLabel;
	private Label levelLabel;
	private Label weaponLabel;
	private Label spiritPointsLabel;
	private Label goldLabel;
	private Godot.Collections.Array<Rune> currentRuneDraft = new Godot.Collections.Array<Rune>();
	private GameManager gameManager;
	
	// Auto-attack system
	private bool isAutoAttackEnabled = false;
	private CheckBox autoAttackCheckbox;
	
 	public override void _Ready()
	{
		GD.Print("🧙 Player: Starting _Ready()...");
		
		// Add player to group so enemies can find it
		AddToGroup("player");
		
		// Try to get the attack timer
		attackCooldownTimer = GetNodeOrNull<Timer>("AttackTimer");
		if (attackCooldownTimer != null)
		{
			attackCooldownTimer.Timeout += OnAttackCooldownComplete;
		}
		else
		{
			GD.PrintErr("AttackTimer node not found! Make sure you have a Timer node named 'AttackTimer' as a child of the Player.");
		}

		GD.Print($"🧙 Player: My path is: {GetPath()}");
		GD.Print($"🧙 Player: My parent is: {GetParent()?.GetPath() ?? "null"}");
		
		// Defer UI setup to ensure GameManager finishes first
		CallDeferred(nameof(SetupUI));

		// Defer enemy spawning until after _Ready() completes
		CallDeferred(nameof(SpawnEnemy));
	}
	
	private void SetupUI()
	{
		GD.Print("🧙 Player: Setting up UI references...");

		// Try to get the attack timer (it should exist now that GameManager finished)
		if (attackCooldownTimer == null)
		{
			attackCooldownTimer = GetNodeOrNull<Timer>("AttackTimer");
			if (attackCooldownTimer != null)
			{
				attackCooldownTimer.Timeout += OnAttackCooldownComplete;
				GD.Print("✅ Attack timer found and connected!");
			}
			else
			{
				GD.PrintErr("❌ AttackTimer still not found!");
			}
		}

		// Try to get the attack button - For GameManager structure
		attackButton = GetNodeOrNull<Button>("../CanvasLayer/AttackButton");
		
		if (attackButton != null)
		{
			attackButton.Pressed += OnAttackButtonPressed;
			GD.Print("✅ Attack button found and connected!");
		}
		else
		{
			GD.PrintErr("❌ AttackButton not found at '../CanvasLayer/AttackButton'");
		}

		// Try to get the combat log
		combatLog = GetNodeOrNull<RichTextLabel>("../CanvasLayer/CombatLog");
		if (combatLog != null)
		{
			GD.Print("✅ Combat log found!");
		}
		else
		{
			GD.PrintErr("❌ CombatLog not found at '../CanvasLayer/CombatLog'");
		}

		// Try to get experience and level labels
		experienceLabel = GetNodeOrNull<Label>("../CanvasLayer/ExperienceLabel");
		if (experienceLabel != null)
		{
			GD.Print("✅ Experience label found!");
		}
		else
		{
			GD.PrintErr("❌ ExperienceLabel not found at '../CanvasLayer/ExperienceLabel'");
		}

		levelLabel = GetNodeOrNull<Label>("../CanvasLayer/LevelLabel");
		if (levelLabel != null)
		{
			GD.Print("✅ Level label found!");
		}
		else
		{
			GD.PrintErr("❌ LevelLabel not found at '../CanvasLayer/LevelLabel'");
		}

		// Try to get weapon and spirit points labels
		weaponLabel = GetNodeOrNull<Label>("../CanvasLayer/WeaponLabel");
		if (weaponLabel != null)
		{
			GD.Print("✅ Weapon label found!");
		}
		else
		{
			GD.PrintErr("❌ WeaponLabel not found at '../CanvasLayer/WeaponLabel'");
		}

		spiritPointsLabel = GetNodeOrNull<Label>("../CanvasLayer/SpiritPointsLabel");
		if (spiritPointsLabel != null)
		{
			GD.Print("✅ Spirit points label found!");
		}
		else
		{
			GD.PrintErr("❌ SpiritPointsLabel not found at '../CanvasLayer/SpiritPointsLabel'");
		}

		// Try to get gold label
		goldLabel = GetNodeOrNull<Label>("../CanvasLayer/GoldLabel");
		if (goldLabel != null)
		{
			GD.Print("✅ Gold label found!");
		}
		else
		{
			GD.PrintErr("❌ GoldLabel not found at '../CanvasLayer/GoldLabel'");
		}

		// Initialize UI
		UpdateExperienceDisplay();
		UpdateWeaponDisplay();
		
		// Initialize inventory and rune UI
		CallDeferred("InitializeUI");
		
		AddToCombatLog("Combat started!", Colors.Yellow);
	}

	private void SpawnEnemy()
	{
		GD.Print("SpawnEnemy called");
		
		// Create enemy programmatically with level scaling
		GD.Print($"Creating enemy programmatically with player level: {Level}");
		var enemy = new Enemy(Level); // Pass player level for HP scaling
		GetParent().CallDeferred("add_child", enemy);
		
		// Create child nodes for the enemy
		var healthLabel = new Label();
		healthLabel.Name = "HealthLabel";
		healthLabel.Position = new Vector2(0, -30);
		healthLabel.AddThemeColorOverride("font_color", Colors.White);
		healthLabel.AddThemeFontSizeOverride("font_size", 14);
		healthLabel.HorizontalAlignment = HorizontalAlignment.Center;
		healthLabel.VerticalAlignment = VerticalAlignment.Center;
		healthLabel.Size = new Vector2(60, 20); // Give it a size so centering works
		healthLabel.Position = new Vector2(-30, -40); // Offset by half the width to center it
		enemy.AddChild(healthLabel);
		
		var colorRect = new ColorRect();
		colorRect.Name = "ColorRect";
		colorRect.Size = new Vector2(50, 50);
		colorRect.Color = new Color(1, 0, 0); // Red
		colorRect.Position = new Vector2(-25, -25); // Center it
		enemy.AddChild(colorRect);
		
		enemy.Position = new Vector2(600, 250); // Position to the right of player
		currentEnemy = enemy;
		GD.Print($"Enemy created at position: {enemy.Position} with {enemy.MaxHp} HP");
		AddToCombatLog($"A level {Level} enemy appears! ({enemy.MaxHp} HP)", Colors.Red);
	}
	
	private void OnAttackButtonPressed()
	{
		GD.Print("Attack button pressed!");
		
		if (currentEnemy != null && IsInstanceValid(currentEnemy))
		{
			GD.Print($"Attacking enemy at position: {currentEnemy.Position}, HP: {currentEnemy.CurrentHp}");
			
			// Calculate damage with level scaling and shop upgrades
			int weaponDamage = PlayerInventory.EquippedWeapon.TotalDamage;
			
			// Add shop damage upgrades
			var damageUpgrade = PlayerShop.Upgrades[0]; // Weapon Mastery
			int shopDamageBonus = damageUpgrade.Level;
			
			float levelScaling = 1.0f + (Level - 1) * 0.1f; // +10% damage per level
			int totalDamage = Mathf.RoundToInt((weaponDamage + shopDamageBonus) * levelScaling);
			
			// Try to get player visual feedback
			var playerRect = GetNodeOrNull<ColorRect>("ColorRect");
			if (playerRect != null)
			{
				playerRect.Modulate = new Color(0.5f, 0.5f, 1);
				
				GetTree().CreateTimer(0.1f).Timeout += () => {
					if (IsInstanceValid(playerRect))
						playerRect.Modulate = Colors.White; // reset to white
				};
			}
			
			// Log the attack
			AddToCombatLog($"You attack the enemy for {totalDamage} damage!", Colors.Orange);
			
	   		currentEnemy.TakeDamage(totalDamage);
			GD.Print($"Enemy HP after damage: {currentEnemy.CurrentHp}");
			
			// Check if enemy was defeated after taking damage
			if (currentEnemy.CurrentHp <= 0)
			{
				GD.Print("Enemy defeated, spawning new one");
				AddToCombatLog("Enemy defeated!", Colors.Green);
				
				// Award experience for killing enemy
				int expReward = 5 + (Level - 1) * 2; // More XP at higher levels
				GainExperience(expReward);
				
				// Award gold for killing enemy
				int goldReward = 2 + Level; // 2 base + 1 per level
				PlayerInventory.AddGold(goldReward);
				AddToCombatLog($"Found {goldReward} gold!", Colors.Gold);
				UpdateGoldRelatedUI();
				
				// Check for weapon drop
				if (LootSystem.ShouldDropWeapon())
				{
					var droppedWeapon = LootSystem.GenerateRandomWeapon(Level);
					PlayerInventory.AddWeapon(droppedWeapon);
					AddToCombatLog($"Found weapon: {droppedWeapon.WeaponName}!", Colors.Yellow);
					
					// Update inventory UI
					GetGameManager()?.UpdateInventoryUI();
				}
				
				SpawnEnemy();
			}
			
			// Update button state if button exists
			if (attackButton != null)
			{
				attackButton.Text = "Cooldown...";
				attackButton.Disabled = true;
			}
			
			// Start cooldown if timer exists
			if (attackCooldownTimer != null)
			{
				float baseCooldown = PlayerInventory.EquippedWeapon.TotalCooldown;
				
				// Apply shop speed upgrades
				var speedUpgrade = PlayerShop.Upgrades[1]; // Combat Training
				float speedReduction = speedUpgrade.Level * 0.1f;
				
				float finalCooldown = Mathf.Max(0.2f, baseCooldown - speedReduction);
				attackCooldownTimer.WaitTime = finalCooldown;
				attackCooldownTimer.Start();
			}
		}
		else
		{
			GD.Print("No valid enemy to attack!");
			AddToCombatLog("No enemy to attack!", Colors.Red);
		}
	}
	
	public bool HasAutoAttack()
	{
		return PlayerShop.Upgrades[2].Level > 0; // Auto-attack is the 3rd upgrade (index 2)
	}
	
	private void OnAttackCooldownComplete()
	{
		if (attackButton != null)
		{
			attackButton.Text = "Attack";
			attackButton.Disabled = false;
		}
		
		// Auto-attack logic
		if (isAutoAttackEnabled && HasAutoAttack() && currentEnemy != null && IsInstanceValid(currentEnemy))
		{
			CallDeferred("OnAttackButtonPressed");
		}
	}
	
	private void AddToCombatLog(string message, Color color = default)
	{
		if (combatLog != null)
		{
			if (color == default)
				color = Colors.White;
			
			// Add timestamp for immersion
			var timeStamp = DateTime.Now.ToString("HH:mm:ss");
			var coloredMessage = $"[color=#{color.ToHtml()}][{timeStamp}] {message}[/color]";
			
			combatLog.AppendText(coloredMessage + "\n");
			
			// Auto-scroll to bottom
			combatLog.ScrollToLine(combatLog.GetLineCount() - 1);
		}
		else
		{
			// Fallback to console if no combat log UI
			GD.Print($"Combat Log: {message}");
		}
	}
	
	private void UpdateExperienceDisplay()
	{
		// Update the new stat panel instead of individual labels
		GetGameManager()?.UpdateStatPanel();
	}
	
	private void UpdateWeaponDisplay()
	{
		// Update the new stat panel instead of individual labels
		GetGameManager()?.UpdateStatPanel();
	}
	
	private void GainExperience(int amount)
	{
		Experience += amount;
		AddToCombatLog($"Gained {amount} experience!", Colors.Cyan);
		
		// Check for level up
		while (Experience >= ExperienceToNextLevel)
		{
			LevelUp();
		}
		
		UpdateExperienceDisplay();
		UpdateWeaponDisplay(); // Update in case spirit points changed
	}
	
	private void LevelUp()
	{
		Experience -= ExperienceToNextLevel;
		Level++;
		
		// Give spirit points on level up
		int spiritPointsGained = 3 + (Level / 3); // 3 base + bonus every 3 levels
		PlayerInventory.AddSpiritPoints(spiritPointsGained);
		
		ExperienceToNextLevel = (int)(ExperienceToNextLevel * 1.5f); // Exponential scaling
		
		AddToCombatLog($"LEVEL UP! You are now level {Level}!", Colors.Gold);
		AddToCombatLog($"Gained {spiritPointsGained} Spirit Points!", Colors.Magenta);
		AddToCombatLog($"Next level requires {ExperienceToNextLevel} XP", Colors.Gray);
		
		// Visual feedback for level up
		var playerRect = GetNodeOrNull<ColorRect>("ColorRect");
		if (playerRect != null)
		{
			// Golden flash for level up
			playerRect.Modulate = Colors.Gold;
			GetTree().CreateTimer(0.3f).Timeout += () => {
				if (IsInstanceValid(playerRect))
					playerRect.Modulate = Colors.White;
			};
		}
		
		// Update UI
		GetGameManager()?.UpdateDraftButton();
	}

	public void LogEnemyDamage(int damage, int currentHp, int maxHp)
	{
		AddToCombatLog($"Enemy takes {damage} damage! ({currentHp}/{maxHp} HP)", Colors.LightCoral);
	}
	
	public void ShowRuneDraft()
	{
		if (!PlayerInventory.CanAffordRuneDraft())
		{
			AddToCombatLog("Not enough Spirit Points to draft runes!", Colors.Red);
			return;
		}
		
		// Spend spirit points
		PlayerInventory.SpendSpiritPoints(PlayerInventory.GetRuneDraftCost());
		
		// Generate rune draft
		currentRuneDraft = new Godot.Collections.Array<Rune>();
		var runes = LootSystem.GenerateRuneDraft(Level);
		foreach (var rune in runes)
		{
			currentRuneDraft.Add(rune);
		}
		
		AddToCombatLog("🎲 Rune draft available! Choose one:", Colors.Purple);
		if (!PlayerInventory.EquippedWeapon.CanAddRune())
		{
			AddToCombatLog("⚠️ Warning: Current weapon has no rune capacity!", Colors.Orange);
		}
		foreach (var rune in currentRuneDraft)
		{
			AddToCombatLog($"  • {rune}", Colors.White);
		}
		
		// Update UI
		UpdateWeaponDisplay();
		GetGameManager()?.UpdateRuneUI(currentRuneDraft);
		GetGameManager()?.UpdateDraftButton();
	}
	
	public void SelectRune(int runeIndex)
	{
		if (runeIndex < 0 || runeIndex >= currentRuneDraft.Count)
		{
			AddToCombatLog("Invalid rune selection!", Colors.Red);
			return;
		}
		
		var selectedRune = currentRuneDraft[runeIndex];
		
		if (!PlayerInventory.EquippedWeapon.CanAddRune())
		{
			AddToCombatLog("Current weapon has no rune capacity left!", Colors.Red);
			return;
		}
		
		if (PlayerInventory.EquippedWeapon.AddRune(selectedRune))
		{
			AddToCombatLog($"Added {selectedRune.RuneName} to {PlayerInventory.EquippedWeapon.WeaponName}!", Colors.Green);
			
			// Clear the draft
			currentRuneDraft.Clear();
			GetGameManager()?.UpdateRuneUI(currentRuneDraft);
			UpdateWeaponDisplay();
			
			// Update weapon details if visible
			var gameManager = GetGameManager();
			if (gameManager != null && gameManager.isWeaponDetailsVisible)
			{
				gameManager.UpdateWeaponDetailsUI();
			}
		}
		else
		{
			AddToCombatLog("Failed to add rune to weapon!", Colors.Red);
		}
	}
	
	public void EquipWeapon(int weaponIndex)
	{
		PlayerInventory.EquipWeapon(weaponIndex);
		UpdateWeaponDisplay();
		GetGameManager()?.UpdateInventoryUI();
		AddToCombatLog($"Equipped: {PlayerInventory.EquippedWeapon.WeaponName}", Colors.Orange);
		
		// Update weapon details if visible
		var gameManager = GetGameManager();
		if (gameManager != null && gameManager.isWeaponDetailsVisible)
		{
			gameManager.UpdateWeaponDetailsUI();
		}
	}
	
	public void ShowWeaponDetails()
	{
		var weapon = PlayerInventory.EquippedWeapon;
		AddToCombatLog("=== WEAPON DETAILS ===", Colors.Gold);
		AddToCombatLog($"Name: {weapon.WeaponName}", Colors.White);
		AddToCombatLog($"Base Damage: {weapon.BaseDamage} → Total: {weapon.TotalDamage}", Colors.Orange);
		AddToCombatLog($"Base Cooldown: {weapon.BaseCooldown:F1}s → Total: {weapon.TotalCooldown:F1}s", Colors.LightBlue);
		AddToCombatLog($"Rune Capacity: {weapon.CurrentRuneCapacity}/{weapon.MaxRuneCapacity}", Colors.Purple);
		
		if (weapon.EquippedRunes.Count > 0)
		{
			AddToCombatLog("Equipped Runes:", Colors.Purple);
			foreach (var rune in weapon.EquippedRunes)
			{
				AddToCombatLog($"  • {rune}", rune.RuneColor);
			}
		}
		else
		{
			AddToCombatLog("No runes equipped", Colors.Gray);
		}
		AddToCombatLog("==================", Colors.Gold);
	}
	
	public void RemoveRune(int runeIndex)
	{
		var weapon = PlayerInventory.EquippedWeapon;
		if (runeIndex >= 0 && runeIndex < weapon.EquippedRunes.Count)
		{
			var removedRune = weapon.EquippedRunes[runeIndex];
			weapon.RemoveRune(removedRune);
			
			AddToCombatLog($"Removed {removedRune.RuneName} from {weapon.WeaponName}", Colors.Yellow);
			
			// Update UI
			UpdateWeaponDisplay();
			GetGameManager()?.UpdateWeaponDetailsUI();
		}
		else
		{
			AddToCombatLog("Invalid rune selection for removal!", Colors.Red);
		}
	}
	
	public void SellWeapon(int weaponIndex)
	{
		if (PlayerInventory.Weapons.Count <= 1)
		{
			AddToCombatLog("Cannot sell your last weapon!", Colors.Red);
			return;
		}
		
		var weapon = PlayerInventory.Weapons[weaponIndex];
		int sellValue = PlayerInventory.GetWeaponSellValue(weapon);
		
		if (PlayerInventory.SellWeapon(weaponIndex))
		{
			AddToCombatLog($"Sold {weapon.WeaponName} for {sellValue} gold!", Colors.Gold);
			UpdateWeaponDisplay();
			UpdateGoldRelatedUI();
			GetGameManager()?.UpdateInventoryUI();
			
			// Update weapon details if visible and we sold the equipped weapon
			var gameManager = GetGameManager();
			if (gameManager != null && gameManager.isWeaponDetailsVisible)
			{
				gameManager.UpdateWeaponDetailsUI();
			}
		}
		else
		{
			AddToCombatLog("Failed to sell weapon!", Colors.Red);
		}
	}
	
	public void PurchaseUpgrade(int upgradeIndex)
	{
		if (upgradeIndex < 0 || upgradeIndex >= PlayerShop.Upgrades.Count)
		{
			GD.Print("Invalid upgrade index!");
			return;
		}
		
		var upgrade = PlayerShop.Upgrades[upgradeIndex];
		int cost = upgrade.GetCurrentCost();
		
		if (PlayerInventory.Gold >= cost)
		{
			PlayerInventory.Gold -= cost;
			upgrade.Level++;
			
			GD.Print($"Purchased {upgrade.Type} upgrade! New level: {upgrade.Level}");
			GD.Print($"Gold remaining: {PlayerInventory.Gold}");
			
			// Handle auto-attack purchase
			if (upgrade.Type == UpgradeType.AutoAttack)
			{
				AddToCombatLog("Auto-Attack system unlocked! Check the box next to attack button to enable.", Colors.Green);
				GetGameManager()?.UpdateAttackUI(); // Update attack button area to show checkbox
			}
			
			// Update UI
			UpdateGoldRelatedUI();
			gameManager?.UpdateInventoryUI();
		}
		else
		{
			GD.Print("Not enough gold for upgrade!");
		}
	}
	
	private GameManager GetGameManager()
	{
		if (gameManager == null)
		{
			gameManager = GetNodeOrNull<GameManager>("../");
		}
		return gameManager;
	}
	
	private void InitializeUI()
	{
		var gameManager = GetGameManager();
		if (gameManager != null)
		{
			gameManager.UpdateInventoryUI();
			gameManager.UpdateDraftButton();
			gameManager.UpdateRuneUI(new Godot.Collections.Array<Rune>());
			gameManager.UpdateStatPanel();
			gameManager.UpdateAttackUI(); // Initialize auto-attack checkbox if purchased
		}
	}
	
	private void UpdateGoldRelatedUI()
	{
		// Update the stat panel
		GetGameManager()?.UpdateStatPanel();
		
		// Update shop UI if it exists and is visible
		var gameManager = GetGameManager();
		if (gameManager != null && gameManager.isShopVisible)
		{
			gameManager.UpdateShopUI();
		}
	}
	
	public void SetAutoAttack(bool enabled)
	{
		isAutoAttackEnabled = enabled;
		GD.Print($"Auto-attack set to: {enabled}");
		if (enabled)
		{
			AddToCombatLog("Auto-attack enabled! Will attack automatically when off cooldown.", Colors.Green);
		}
		else
		{
			AddToCombatLog("Auto-attack disabled.", Colors.Yellow);
		}
	}
}
