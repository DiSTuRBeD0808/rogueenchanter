using Godot;

public partial class GameManager : Node
{
	private Player player;
	private CanvasLayer ui;
	private Button attackButton;
	private Timer attackTimer;
	private RichTextLabel combatLog;
	private Label experienceLabel;
	private Label levelLabel;
	private Label weaponLabel;
	private Label spiritPointsLabel;
	private Label goldLabel;
	private VBoxContainer inventoryContainer;
	private VBoxContainer runeContainer;
	private Button draftRunesButton;
	private Button weaponDetailsButton;
	private VBoxContainer weaponDetailsContainer;
	public bool isWeaponDetailsVisible = false;
	private Button shopButton;
	private VBoxContainer shopContainer;
	public bool isShopVisible = false;
	private CheckBox autoAttackCheckbox;
	
	// Stat panel elements
	private VBoxContainer statPanel;
	private VBoxContainer enemyStatPanel;
	private Label playerLevelLabel;
	private Label playerExpLabel;
	private Label playerDamageLabel;
	private Label playerCooldownLabel;
	private Label playerSpiritLabel;
	private Label playerGoldLabel;

	public override void _Ready()
	{
		GD.Print("🎮 GameManager: Starting setup...");
		
		// Add GameManager to group so enemies can find it
		AddToGroup("game_manager");
		
		// Create the entire game scene programmatically
		SetupScene();
		GD.Print("🎮 GameManager: Setup complete!");
	}

	private void SetupScene()
	{
		GD.Print("📋 Creating UI layer...");
		// Create UI layer
		ui = new CanvasLayer();
		ui.Name = "CanvasLayer";  // Set the name explicitly!
		AddChild(ui);

		GD.Print("👤 Creating player...");
		// Create player
		player = new Player();
		player.Name = "Player";  // Set the name explicitly!
		AddChild(player);
		
		// Create player visual
		var playerRect = new ColorRect();
		playerRect.Name = "ColorRect";
		playerRect.Size = new Vector2(50, 50);
		playerRect.Color = Colors.Blue;
		playerRect.Position = new Vector2(-25, -25);
		player.AddChild(playerRect);
		
		// Add collision shape for player (since Player inherits from CharacterBody2D)
		var collisionShape = new CollisionShape2D();
		var circleShape = new CircleShape2D();
		circleShape.Radius = 25;
		collisionShape.Shape = circleShape;
		player.AddChild(collisionShape);
		
		// Create attack timer for player
		attackTimer = new Timer();
		attackTimer.Name = "AttackTimer";
		attackTimer.WaitTime = 2.0f; // Default, will be updated by weapon stats
		attackTimer.OneShot = true;
		player.AddChild(attackTimer);

		// Position player (center-left, below attack button and stat panels)
		player.Position = new Vector2(400, 400);

		GD.Print("🎨 Creating UI elements...");
		// Create UI elements
		SetupUI();
		GD.Print("✅ Scene setup complete!");
	}

	private void SetupUI()
	{
		GD.Print("🔘 Creating Attack Button...");
		// Attack Button - Bottom center
		attackButton = new Button();
		attackButton.Name = "AttackButton";
		attackButton.Text = "⚔️ ATTACK";
		attackButton.Size = new Vector2(140, 60);
		attackButton.Position = new Vector2(640, 500); // Bottom center (assuming 1280x720 screen)
		
		// Style the attack button
		attackButton.AddThemeColorOverride("font_color", Colors.White);
		attackButton.AddThemeFontSizeOverride("font_size", 18);
		ui.AddChild(attackButton);

		GD.Print("📜 Creating Combat Log...");
		// Combat Log - Right side
		combatLog = new RichTextLabel();
		combatLog.Name = "CombatLog";
		combatLog.Size = new Vector2(400, 400);
		combatLog.Position = new Vector2(850, 50); // Right side
		combatLog.BbcodeEnabled = true;
		combatLog.ScrollFollowing = true;
		
		// Style the combat log background
		var logStyle = new StyleBoxFlat();
		logStyle.BgColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
		logStyle.BorderColor = Colors.Gray;
		logStyle.BorderWidthTop = 2;
		logStyle.BorderWidthBottom = 2;
		logStyle.BorderWidthLeft = 2;
		logStyle.BorderWidthRight = 2;
		combatLog.AddThemeStyleboxOverride("normal", logStyle);
		ui.AddChild(combatLog);

		GD.Print("📊 Creating Experience Label...");
		// Experience Label - Bottom left
		experienceLabel = new Label();
		experienceLabel.Name = "ExperienceLabel";
		experienceLabel.Text = "XP: 0/10";
		experienceLabel.Position = new Vector2(50, 500);
		experienceLabel.AddThemeColorOverride("font_color", Colors.Cyan);
		experienceLabel.AddThemeFontSizeOverride("font_size", 16);
		ui.AddChild(experienceLabel);

		GD.Print("🏆 Creating Level Label...");
		// Level Label - Bottom left, below XP
		levelLabel = new Label();
		levelLabel.Name = "LevelLabel";
		levelLabel.Text = "Level: 1";
		levelLabel.Position = new Vector2(50, 530);
		levelLabel.AddThemeColorOverride("font_color", Colors.Gold);
		levelLabel.AddThemeFontSizeOverride("font_size", 16);
		ui.AddChild(levelLabel);

		// Add game title
		var titleLabel = new Label();
		titleLabel.Text = "🗡️ Rogue Enchanter";
		titleLabel.Position = new Vector2(50, 20);
		titleLabel.AddThemeColorOverride("font_color", Colors.Gold);
		titleLabel.AddThemeFontSizeOverride("font_size", 28);
		ui.AddChild(titleLabel);

		// Add combat log title
		var logTitleLabel = new Label();
		logTitleLabel.Text = "📜 Combat Log";
		logTitleLabel.Position = new Vector2(850, 20);
		logTitleLabel.AddThemeColorOverride("font_color", Colors.White);
		logTitleLabel.AddThemeFontSizeOverride("font_size", 18);
		ui.AddChild(logTitleLabel);
		
		// Create comprehensive stat panel - positioned to the left of player
		SetupStatPanel();
		SetupEnemyStatPanel();

		GD.Print("⚔️ Creating Weapon Details Button...");
		// Weapon details button - moved below player info
		weaponDetailsButton = new Button();
		weaponDetailsButton.Name = "WeaponDetailsButton";
		weaponDetailsButton.Text = "⚔️ Weapon Details";
		weaponDetailsButton.Size = new Vector2(160, 30);
		weaponDetailsButton.Position = new Vector2(50, 630);
		weaponDetailsButton.AddThemeColorOverride("font_color", Colors.White);
		weaponDetailsButton.AddThemeFontSizeOverride("font_size", 12);
		weaponDetailsButton.Pressed += OnWeaponDetailsPressed;
		ui.AddChild(weaponDetailsButton);

		GD.Print("🏪 Creating Shop Button...");
		// Shop button - next to weapon details button
		shopButton = new Button();
		shopButton.Name = "ShopButton";
		shopButton.Text = "🏪 Shop";
		shopButton.Size = new Vector2(100, 30);
		shopButton.Position = new Vector2(220, 630);
		shopButton.AddThemeColorOverride("font_color", Colors.White);
		shopButton.AddThemeFontSizeOverride("font_size", 12);
		shopButton.Pressed += OnShopPressed;
		ui.AddChild(shopButton);

		GD.Print("🎒 Creating Inventory UI...");
		// Inventory section - under combat log
		var inventoryTitle = new Label();
		inventoryTitle.Text = "🎒 Inventory";
		inventoryTitle.Position = new Vector2(850, 470);
		inventoryTitle.AddThemeColorOverride("font_color", Colors.Orange);
		inventoryTitle.AddThemeFontSizeOverride("font_size", 16);
		ui.AddChild(inventoryTitle);

		inventoryContainer = new VBoxContainer();
		inventoryContainer.Name = "InventoryContainer";
		inventoryContainer.Position = new Vector2(850, 500);
		inventoryContainer.Size = new Vector2(400, 200);
		ui.AddChild(inventoryContainer);

		GD.Print("🔮 Creating Rune UI...");
		// Rune section - to the right of inventory
		var runeTitle = new Label();
		runeTitle.Text = "🔮 Runes";
		runeTitle.Position = new Vector2(1270, 470);
		runeTitle.AddThemeColorOverride("font_color", Colors.Purple);
		runeTitle.AddThemeFontSizeOverride("font_size", 16);
		ui.AddChild(runeTitle);

		// Draft Runes button
		draftRunesButton = new Button();
		draftRunesButton.Name = "DraftRunesButton";
		draftRunesButton.Text = "🎲 Draft Runes (5 SP)";
		draftRunesButton.Size = new Vector2(180, 40);
		draftRunesButton.Position = new Vector2(1270, 500);
		draftRunesButton.AddThemeColorOverride("font_color", Colors.White);
		draftRunesButton.AddThemeFontSizeOverride("font_size", 12);
		draftRunesButton.Pressed += OnDraftRunesPressed;
		ui.AddChild(draftRunesButton);

		runeContainer = new VBoxContainer();
		runeContainer.Name = "RuneContainer";
		runeContainer.Position = new Vector2(1270, 550);
		runeContainer.Size = new Vector2(200, 150);
		ui.AddChild(runeContainer);

		GD.Print("⚔️ Creating Weapon Details Container...");
		// Weapon details container - positioned below the button
		weaponDetailsContainer = new VBoxContainer();
		weaponDetailsContainer.Name = "WeaponDetailsContainer";
		weaponDetailsContainer.Position = new Vector2(50, 670);
		weaponDetailsContainer.Size = new Vector2(400, 200);
		weaponDetailsContainer.Visible = false;
		
		// Style the container background
		var detailsStyle = new StyleBoxFlat();
		detailsStyle.BgColor = new Color(0.2f, 0.2f, 0.3f, 0.9f);
		detailsStyle.BorderColor = Colors.Gold;
		detailsStyle.BorderWidthTop = 2;
		detailsStyle.BorderWidthBottom = 2;
		detailsStyle.BorderWidthLeft = 2;
		detailsStyle.BorderWidthRight = 2;
		weaponDetailsContainer.AddThemeStyleboxOverride("panel", detailsStyle);
		ui.AddChild(weaponDetailsContainer);

		GD.Print("🏪 Creating Shop Container...");
		// Shop container - positioned next to weapon details
		shopContainer = new VBoxContainer();
		shopContainer.Name = "ShopContainer";
		shopContainer.Position = new Vector2(460, 670);
		shopContainer.Size = new Vector2(350, 200);
		shopContainer.Visible = false;
		
		// Style the shop container background
		var shopStyle = new StyleBoxFlat();
		shopStyle.BgColor = new Color(0.1f, 0.3f, 0.1f, 0.9f);
		shopStyle.BorderColor = Colors.Gold;
		shopStyle.BorderWidthTop = 2;
		shopStyle.BorderWidthBottom = 2;
		shopStyle.BorderWidthLeft = 2;
		shopStyle.BorderWidthRight = 2;
		shopContainer.AddThemeStyleboxOverride("panel", shopStyle);
		ui.AddChild(shopContainer);
		
		GD.Print($"✅ UI Elements created! CanvasLayer children: {ui.GetChildCount()}");
		for (int i = 0; i < ui.GetChildCount(); i++)
		{
			var child = ui.GetChild(i);
			GD.Print($"  - {child.Name} ({child.GetType().Name}) at path: {child.GetPath()}");
		}
	}

	private void OnDraftRunesPressed()
	{
		GD.Print("Draft Runes button pressed!");
		// Let the player handle the rune drafting logic
		if (player != null)
		{
			player.CallDeferred("ShowRuneDraft");
		}
	}
	
	private void OnWeaponDetailsPressed()
	{
		GD.Print("Weapon Details button pressed!");
		
		isWeaponDetailsVisible = !isWeaponDetailsVisible;
		weaponDetailsContainer.Visible = isWeaponDetailsVisible;
		
		if (isWeaponDetailsVisible)
		{
			weaponDetailsButton.Text = "❌ Close Details";
			UpdateWeaponDetailsUI();
		}
		else
		{
			weaponDetailsButton.Text = "⚔️ Weapon Details";
		}
	}
	
	private void OnShopPressed()
	{
		GD.Print("Shop button pressed!");
		
		isShopVisible = !isShopVisible;
		shopContainer.Visible = isShopVisible;
		
		if (isShopVisible)
		{
			shopButton.Text = "❌ Close Shop";
			UpdateShopUI();
		}
		else
		{
			shopButton.Text = "🏪 Shop";
		}
	}
	
	public void UpdateInventoryUI()
	{
		if (inventoryContainer == null || player == null) return;
		
		// Clear existing inventory buttons
		foreach (Node child in inventoryContainer.GetChildren())
		{
			child.QueueFree();
		}
		
		// Add weapon buttons
		for (int i = 0; i < player.PlayerInventory.Weapons.Count; i++)
		{
			var weapon = player.PlayerInventory.Weapons[i];
			int sellValue = player.PlayerInventory.GetWeaponSellValue(weapon);
			var button = new Button();
			button.Text = $"{weapon.WeaponName} ({weapon.TotalDamage} dmg) [Sell: {sellValue}g]";
			button.Size = new Vector2(380, 30);
			
			// Highlight equipped weapon
			if (i == player.PlayerInventory.EquippedWeaponIndex)
			{
				button.AddThemeColorOverride("font_color", Colors.Yellow);
				button.Text = "★ " + button.Text;
			}
			else
			{
				button.AddThemeColorOverride("font_color", Colors.White);
			}
			
			// Add tooltip
			button.TooltipText = $"Left-click to equip\nRight-click to sell for {sellValue} gold";
			
			// Add right-click functionality for selling
			int weaponIndex = i; // Capture for closure
			button.Pressed += () => OnWeaponSelected(weaponIndex);
			
			// Add right-click to sell
			button.GuiInput += (InputEvent @event) => {
				if (@event is InputEventMouseButton mouseEvent && 
					mouseEvent.Pressed && 
					mouseEvent.ButtonIndex == MouseButton.Right)
				{
					OnWeaponSellRequested(weaponIndex);
				}
			};
			
			inventoryContainer.AddChild(button);
		}
	}
	
	private void OnWeaponSelected(int weaponIndex)
	{
		GD.Print($"Selected weapon index: {weaponIndex}");
		if (player != null)
		{
			player.CallDeferred("EquipWeapon", weaponIndex);
		}
	}
	
	public void UpdateRuneUI(Godot.Collections.Array<Rune> runes)
	{
		if (runeContainer == null) return;
		
		// Clear existing rune buttons
		foreach (Node child in runeContainer.GetChildren())
		{
			child.QueueFree();
		}
		
		// Add rune buttons
		for (int i = 0; i < runes.Count; i++)
		{
			var rune = runes[i];
			var button = new Button();
			button.Text = rune.RuneName;
			button.Size = new Vector2(180, 25);
			button.AddThemeColorOverride("font_color", rune.RuneColor);
			button.AddThemeFontSizeOverride("font_size", 10);
			
			int runeIndex = i; // Capture for closure
			button.Pressed += () => OnRuneSelected(runeIndex);
			runeContainer.AddChild(button);
		}
	}
	
	public void UpdateDraftButton()
	{
		if (draftRunesButton == null || player == null) return;
		
		bool canAfford = player.PlayerInventory.CanAffordRuneDraft();
		draftRunesButton.Disabled = !canAfford;
		
		if (canAfford)
		{
			draftRunesButton.AddThemeColorOverride("font_color", Colors.White);
		}
		else
		{
			draftRunesButton.AddThemeColorOverride("font_color", Colors.Gray);
		}
	}
	
	public void UpdateWeaponDetailsUI()
	{
		if (weaponDetailsContainer == null || player == null) return;
		
		// Clear existing details
		foreach (Node child in weaponDetailsContainer.GetChildren())
		{
			child.QueueFree();
		}
		
		var weapon = player.PlayerInventory.EquippedWeapon;
		
		// Weapon name and stats
		var nameLabel = new Label();
		nameLabel.Text = $"⚔️ {weapon.WeaponName}";
		nameLabel.AddThemeColorOverride("font_color", Colors.Gold);
		nameLabel.AddThemeFontSizeOverride("font_size", 18);
		weaponDetailsContainer.AddChild(nameLabel);
		
		var statsLabel = new Label();
		float levelScaling = 1.0f + (player.Level - 1) * 0.1f;
		int scaledDamage = Mathf.RoundToInt(weapon.TotalDamage * levelScaling);
		statsLabel.Text = $"Damage: {weapon.BaseDamage} (base) → {weapon.TotalDamage} (runes) → {scaledDamage} (level {player.Level})";
		statsLabel.AddThemeColorOverride("font_color", Colors.Orange);
		statsLabel.AddThemeFontSizeOverride("font_size", 12);
		weaponDetailsContainer.AddChild(statsLabel);
		
		var cooldownLabel = new Label();
		cooldownLabel.Text = $"Cooldown: {weapon.BaseCooldown:F1}s → {weapon.TotalCooldown:F1}s";
		cooldownLabel.AddThemeColorOverride("font_color", Colors.LightBlue);
		cooldownLabel.AddThemeFontSizeOverride("font_size", 12);
		weaponDetailsContainer.AddChild(cooldownLabel);
		
		var capacityLabel = new Label();
		capacityLabel.Text = $"Rune Capacity: {weapon.CurrentRuneCapacity}/{weapon.MaxRuneCapacity}";
		capacityLabel.AddThemeColorOverride("font_color", Colors.Purple);
		capacityLabel.AddThemeFontSizeOverride("font_size", 12);
		weaponDetailsContainer.AddChild(capacityLabel);
		
		// Spacer
		var spacer = new Control();
		spacer.CustomMinimumSize = new Vector2(0, 10);
		weaponDetailsContainer.AddChild(spacer);
		
		// Equipped runes section
		var runesTitle = new Label();
		runesTitle.Text = "🔮 Equipped Runes (Right-click to remove):";
		runesTitle.AddThemeColorOverride("font_color", Colors.Purple);
		runesTitle.AddThemeFontSizeOverride("font_size", 14);
		weaponDetailsContainer.AddChild(runesTitle);
		
		if (weapon.EquippedRunes.Count > 0)
		{
			for (int i = 0; i < weapon.EquippedRunes.Count; i++)
			{
				var rune = weapon.EquippedRunes[i];
				var runeButton = new Button();
				runeButton.Text = $"• {rune.RuneName} - {rune.Description}";
				runeButton.AddThemeColorOverride("font_color", rune.RuneColor);
				runeButton.AddThemeFontSizeOverride("font_size", 11);
				runeButton.Size = new Vector2(380, 25);
				runeButton.Alignment = HorizontalAlignment.Left;
				
				// Add right-click functionality
				int runeIndex = i; // Capture for closure
				runeButton.GuiInput += (InputEvent @event) => {
					if (@event is InputEventMouseButton mouseEvent && 
						mouseEvent.Pressed && 
						mouseEvent.ButtonIndex == MouseButton.Right)
					{
						OnRuneRemovalRequested(runeIndex);
					}
				};
				
				weaponDetailsContainer.AddChild(runeButton);
			}
		}
		else
		{
			var noRunesLabel = new Label();
			noRunesLabel.Text = "  No runes equipped";
			noRunesLabel.AddThemeColorOverride("font_color", Colors.Gray);
			noRunesLabel.AddThemeFontSizeOverride("font_size", 11);
			weaponDetailsContainer.AddChild(noRunesLabel);
		}
	}
	
	public void UpdateShopUI()
	{
		if (shopContainer == null || player == null) return;
		
		// Clear existing shop items
		foreach (Node child in shopContainer.GetChildren())
		{
			child.QueueFree();
		}
		
		// Shop title
		var titleLabel = new Label();
		titleLabel.Text = "🏪 Upgrades Shop";
		titleLabel.AddThemeColorOverride("font_color", Colors.Gold);
		titleLabel.AddThemeFontSizeOverride("font_size", 18);
		shopContainer.AddChild(titleLabel);
		
		// Add shop upgrades
		for (int i = 0; i < player.PlayerShop.Upgrades.Count; i++)
		{
			var upgrade = player.PlayerShop.Upgrades[i];
			var button = new Button();
			button.Text = upgrade.GetDisplayText();
			button.Size = new Vector2(330, 30);
			button.TooltipText = upgrade.GetTooltipText();
			
			// Color based on affordability
			bool canAfford = player.PlayerShop.CanAffordUpgrade(upgrade, player.PlayerInventory.Gold);
			bool alreadyPurchased = (upgrade.Type == UpgradeType.AutoAttack && upgrade.Level > 0);
			
			if (alreadyPurchased)
			{
				button.AddThemeColorOverride("font_color", Colors.Green);
				button.Disabled = true;
			}
			else if (canAfford)
			{
				button.AddThemeColorOverride("font_color", Colors.White);
			}
			else
			{
				button.AddThemeColorOverride("font_color", Colors.Gray);
				button.Disabled = true;
			}
			
			int upgradeIndex = i; // Capture for closure
			button.Pressed += () => OnUpgradePurchased(upgradeIndex);
			shopContainer.AddChild(button);
		}
		
		// Current gold display
		var goldLabel = new Label();
		goldLabel.Text = $"💰 Your Gold: {player.PlayerInventory.Gold}";
		goldLabel.AddThemeColorOverride("font_color", Colors.Gold);
		goldLabel.AddThemeFontSizeOverride("font_size", 14);
		shopContainer.AddChild(goldLabel);
	}
	
	private void OnUpgradePurchased(int upgradeIndex)
	{
		GD.Print($"Purchasing upgrade index: {upgradeIndex}");
		if (player != null)
		{
			player.CallDeferred("PurchaseUpgrade", upgradeIndex);
		}
	}
	
	private void OnRuneRemovalRequested(int runeIndex)
	{
		GD.Print($"Removing rune at index: {runeIndex}");
		if (player != null)
		{
			player.CallDeferred("RemoveRune", runeIndex);
		}
	}
	
	private void OnWeaponSellRequested(int weaponIndex)
	{
		GD.Print($"Selling weapon at index: {weaponIndex}");
		if (player != null)
		{
			player.CallDeferred("SellWeapon", weaponIndex);
		}
	}

	public void UpdateAttackUI()
	{
		if (player == null) return;
		
		// Add auto-attack checkbox if auto-attack is purchased and checkbox doesn't exist
		if (player.HasAutoAttack() && autoAttackCheckbox == null)
		{
			autoAttackCheckbox = new CheckBox();
			autoAttackCheckbox.Name = "AutoAttackCheckbox";
			autoAttackCheckbox.Text = "Auto";
			autoAttackCheckbox.Size = new Vector2(60, 30);
			autoAttackCheckbox.Position = new Vector2(580, 505); // Left of attack button
			autoAttackCheckbox.AddThemeColorOverride("font_color", Colors.White);
			autoAttackCheckbox.AddThemeFontSizeOverride("font_size", 12);
			autoAttackCheckbox.TooltipText = "Enable auto-attack when off cooldown";
			autoAttackCheckbox.Toggled += OnAutoAttackToggled;
			ui.AddChild(autoAttackCheckbox);
		}
		
		// Show/hide checkbox based on auto-attack availability
		if (autoAttackCheckbox != null)
		{
			autoAttackCheckbox.Visible = player.HasAutoAttack();
		}
	}
	
	private void OnAutoAttackToggled(bool buttonPressed)
	{
		GD.Print($"Auto-attack toggled: {buttonPressed}");
		if (player != null)
		{
			player.SetAutoAttack(buttonPressed);
		}
	}

	private void OnRuneSelected(int runeIndex)
	{
		GD.Print($"Selected rune index: {runeIndex}");
		if (player != null)
		{
			player.CallDeferred("SelectRune", runeIndex);
		}
	}
	
	private void SetupStatPanel()
	{
		// Create stat panel container - positioned to the left of player (around x=200)
		statPanel = new VBoxContainer();
		statPanel.Name = "StatPanel";
		statPanel.Position = new Vector2(200, 80);
		statPanel.Size = new Vector2(180, 400);
		
		// Style the stat panel background
		var statStyle = new StyleBoxFlat();
		statStyle.BgColor = new Color(0.1f, 0.1f, 0.2f, 0.9f);
		statStyle.BorderColor = Colors.Cyan;
		statStyle.BorderWidthTop = 2;
		statStyle.BorderWidthBottom = 2;
		statStyle.BorderWidthLeft = 2;
		statStyle.BorderWidthRight = 2;
		statPanel.AddThemeStyleboxOverride("panel", statStyle);
		ui.AddChild(statPanel);
		
		// Panel title
		var statTitle = new Label();
		statTitle.Text = "📊 Player Stats";
		statTitle.AddThemeColorOverride("font_color", Colors.Cyan);
		statTitle.AddThemeFontSizeOverride("font_size", 16);
		statTitle.HorizontalAlignment = HorizontalAlignment.Center;
		statPanel.AddChild(statTitle);
		
		// Add spacer
		var spacer1 = new Control();
		spacer1.CustomMinimumSize = new Vector2(0, 10);
		statPanel.AddChild(spacer1);
		
		// Level and Experience
		playerLevelLabel = new Label();
		playerLevelLabel.Text = "Level: 1";
		playerLevelLabel.AddThemeColorOverride("font_color", Colors.Gold);
		playerLevelLabel.AddThemeFontSizeOverride("font_size", 14);
		statPanel.AddChild(playerLevelLabel);
		
		playerExpLabel = new Label();
		playerExpLabel.Text = "XP: 0/10";
		playerExpLabel.AddThemeColorOverride("font_color", Colors.Cyan);
		playerExpLabel.AddThemeFontSizeOverride("font_size", 12);
		statPanel.AddChild(playerExpLabel);
		
		// Add spacer
		var spacer2 = new Control();
		spacer2.CustomMinimumSize = new Vector2(0, 10);
		statPanel.AddChild(spacer2);
		
		// Combat Stats
		var combatTitle = new Label();
		combatTitle.Text = "⚔️ Combat";
		combatTitle.AddThemeColorOverride("font_color", Colors.Orange);
		combatTitle.AddThemeFontSizeOverride("font_size", 14);
		statPanel.AddChild(combatTitle);
		
		playerDamageLabel = new Label();
		playerDamageLabel.Text = "Damage: 5";
		playerDamageLabel.AddThemeColorOverride("font_color", Colors.Red);
		playerDamageLabel.AddThemeFontSizeOverride("font_size", 12);
		statPanel.AddChild(playerDamageLabel);
		
		playerCooldownLabel = new Label();
		playerCooldownLabel.Text = "Cooldown: 2.0s";
		playerCooldownLabel.AddThemeColorOverride("font_color", Colors.LightBlue);
		playerCooldownLabel.AddThemeFontSizeOverride("font_size", 12);
		statPanel.AddChild(playerCooldownLabel);
		
		// Add HP and defensive stats
		var hpLabel = new Label();
		hpLabel.Text = "HP: 100/100";
		hpLabel.AddThemeColorOverride("font_color", Colors.Green);
		hpLabel.AddThemeFontSizeOverride("font_size", 12);
		statPanel.AddChild(hpLabel);
		
		var armorLabel = new Label();
		armorLabel.Text = "Armor: 0";
		armorLabel.AddThemeColorOverride("font_color", Colors.Gray);
		armorLabel.AddThemeFontSizeOverride("font_size", 12);
		statPanel.AddChild(armorLabel);
		
		var armorPiercingLabel = new Label();
		armorPiercingLabel.Text = "Armor Piercing: 0";
		armorPiercingLabel.AddThemeColorOverride("font_color", Colors.Yellow);
		armorPiercingLabel.AddThemeFontSizeOverride("font_size", 12);
		statPanel.AddChild(armorPiercingLabel);
		
		var hpRegenLabel = new Label();
		hpRegenLabel.Text = "HP Regen: 1.0/s";
		hpRegenLabel.AddThemeColorOverride("font_color", Colors.LightGreen);
		hpRegenLabel.AddThemeFontSizeOverride("font_size", 12);
		statPanel.AddChild(hpRegenLabel);
		
		// Add spacer
		var spacer3 = new Control();
		spacer3.CustomMinimumSize = new Vector2(0, 10);
		statPanel.AddChild(spacer3);
		
		// Resources
		var resourceTitle = new Label();
		resourceTitle.Text = "💰 Resources";
		resourceTitle.AddThemeColorOverride("font_color", Colors.Green);
		resourceTitle.AddThemeFontSizeOverride("font_size", 14);
		statPanel.AddChild(resourceTitle);
		
		playerSpiritLabel = new Label();
		playerSpiritLabel.Text = "Spirit Points: 0";
		playerSpiritLabel.AddThemeColorOverride("font_color", Colors.Magenta);
		playerSpiritLabel.AddThemeFontSizeOverride("font_size", 12);
		statPanel.AddChild(playerSpiritLabel);
		
		playerGoldLabel = new Label();
		playerGoldLabel.Text = "Gold: 10";
		playerGoldLabel.AddThemeColorOverride("font_color", Colors.Gold);
		playerGoldLabel.AddThemeFontSizeOverride("font_size", 12);
		statPanel.AddChild(playerGoldLabel);
		
		// Update references for compatibility
		experienceLabel = playerExpLabel;
		levelLabel = playerLevelLabel;
		weaponLabel = playerDamageLabel; // This will be updated by UpdateStatPanel
		spiritPointsLabel = playerSpiritLabel;
		goldLabel = playerGoldLabel;
	}
	
	private void SetupEnemyStatPanel()
	{
		// Create enemy stat panel on the right side
		enemyStatPanel = new VBoxContainer();
		enemyStatPanel.Name = "EnemyStatPanel";
		enemyStatPanel.Position = new Vector2(600, 80);  // Right side of screen
		enemyStatPanel.Size = new Vector2(180, 300);
		
		// Background style
		var enemyStatStyle = new StyleBoxFlat();
		enemyStatStyle.BgColor = new Color(0.3f, 0.1f, 0.1f, 0.8f);  // Dark red background
		enemyStatStyle.BorderColor = Colors.DarkRed;
		enemyStatStyle.BorderWidthLeft = 2;
		enemyStatStyle.BorderWidthRight = 2;
		enemyStatStyle.BorderWidthTop = 2;
		enemyStatStyle.BorderWidthBottom = 2;
		enemyStatStyle.CornerRadiusTopLeft = 8;
		enemyStatStyle.CornerRadiusTopRight = 8;
		enemyStatStyle.CornerRadiusBottomLeft = 8;
		enemyStatStyle.CornerRadiusBottomRight = 8;
		enemyStatPanel.AddThemeStyleboxOverride("panel", enemyStatStyle);
		ui.AddChild(enemyStatPanel);
		
		// Title
		var enemyTitle = new Label();
		enemyTitle.Text = "👹 Enemy Stats";
		enemyTitle.AddThemeColorOverride("font_color", Colors.Red);
		enemyTitle.AddThemeFontSizeOverride("font_size", 16);
		enemyTitle.HorizontalAlignment = HorizontalAlignment.Center;
		enemyStatPanel.AddChild(enemyTitle);
		
		// Spacer
		var spacer = new Control();
		spacer.CustomMinimumSize = new Vector2(0, 10);
		enemyStatPanel.AddChild(spacer);
		
		// Enemy Level
		var enemyLevelLabel = new Label();
		enemyLevelLabel.Name = "EnemyLevelLabel";
		enemyLevelLabel.Text = "Level: 1";
		enemyLevelLabel.AddThemeColorOverride("font_color", Colors.Orange);
		enemyLevelLabel.AddThemeFontSizeOverride("font_size", 14);
		enemyStatPanel.AddChild(enemyLevelLabel);
		
		// Enemy HP
		var enemyHpLabel = new Label();
		enemyHpLabel.Name = "EnemyHpLabel";
		enemyHpLabel.Text = "HP: 5/5";
		enemyHpLabel.AddThemeColorOverride("font_color", Colors.Red);
		enemyHpLabel.AddThemeFontSizeOverride("font_size", 12);
		enemyStatPanel.AddChild(enemyHpLabel);
		
		// Enemy Damage
		var enemyDamageLabel = new Label();
		enemyDamageLabel.Name = "EnemyDamageLabel";
		enemyDamageLabel.Text = "Damage: 3";
		enemyDamageLabel.AddThemeColorOverride("font_color", Colors.DarkRed);
		enemyDamageLabel.AddThemeFontSizeOverride("font_size", 12);
		enemyStatPanel.AddChild(enemyDamageLabel);
		
		// Enemy Armor
		var enemyArmorLabel = new Label();
		enemyArmorLabel.Name = "EnemyArmorLabel";
		enemyArmorLabel.Text = "Armor: 0";
		enemyArmorLabel.AddThemeColorOverride("font_color", Colors.Gray);
		enemyArmorLabel.AddThemeFontSizeOverride("font_size", 12);
		enemyStatPanel.AddChild(enemyArmorLabel);
		
		// Enemy Attack Speed
		var enemyAttackSpeedLabel = new Label();
		enemyAttackSpeedLabel.Name = "EnemyAttackSpeedLabel";
		enemyAttackSpeedLabel.Text = "Attack Speed: 3.0s";
		enemyAttackSpeedLabel.AddThemeColorOverride("font_color", Colors.Orange);
		enemyAttackSpeedLabel.AddThemeFontSizeOverride("font_size", 12);
		enemyStatPanel.AddChild(enemyAttackSpeedLabel);
		
		// Enemy HP Regen
		var enemyHpRegenLabel = new Label();
		enemyHpRegenLabel.Name = "EnemyHpRegenLabel";
		enemyHpRegenLabel.Text = "HP Regen: 0.2/s";
		enemyHpRegenLabel.AddThemeColorOverride("font_color", Colors.Pink);
		enemyHpRegenLabel.AddThemeFontSizeOverride("font_size", 12);
		enemyStatPanel.AddChild(enemyHpRegenLabel);
	}
	
	public void UpdateStatPanel()
	{
		if (player == null) 
		{
			GD.Print("UpdateStatPanel: player is null!");
			return;
		}
		
		// Update level and experience
		if (playerLevelLabel != null)
		{
			playerLevelLabel.Text = $"Level: {player.Level}";
			GD.Print($"Updated Level label to: {player.Level}");
		}
		else
		{
			GD.Print("UpdateStatPanel: playerLevelLabel is null!");
		}
		
		if (playerExpLabel != null)
		{
			playerExpLabel.Text = $"XP: {player.Experience}/{player.ExperienceToNextLevel}";
			GD.Print($"Updated XP label to: {player.Experience}/{player.ExperienceToNextLevel}");
		}
		else
		{
			GD.Print("UpdateStatPanel: playerExpLabel is null!");
		}
		
		// Update combat stats
		if (playerDamageLabel != null)
		{
			var weapon = player.PlayerInventory.EquippedWeapon;
			int shopDamageBonus = player.PlayerShop.Upgrades[0].Level; // Weapon Mastery
			float levelScaling = 1.0f + (player.Level - 1) * 0.1f;
			float efficiency = player.CalculateAttackEfficiency();
			int totalDamage = Mathf.RoundToInt((weapon.TotalDamage + shopDamageBonus) * levelScaling * efficiency);
			
			string damageText = $"Damage: {totalDamage}";
			if (efficiency > 1.0f)
			{
				damageText += $" (x{efficiency:F1})";
				playerDamageLabel.AddThemeColorOverride("font_color", Colors.Gold);
			}
			else
			{
				playerDamageLabel.AddThemeColorOverride("font_color", Colors.Red);
			}
			
			playerDamageLabel.Text = damageText;
			playerDamageLabel.TooltipText = $"Base: {weapon.BaseDamage} + Runes: +{weapon.TotalDamage - weapon.BaseDamage} + Shop: +{shopDamageBonus} + Level: x{levelScaling:F1} + Efficiency: x{efficiency:F1}";
		}
		
		if (playerCooldownLabel != null)
		{
			var weapon = player.PlayerInventory.EquippedWeapon;
			var speedUpgrade = player.PlayerShop.Upgrades[1]; // Combat Training
			float speedReduction = speedUpgrade.Level * 0.1f;
			float actualCooldown = weapon.TotalCooldown - speedReduction;
			float effectiveCooldown = player.GetEffectiveCooldown();
			float efficiency = player.CalculateAttackEfficiency();
			
			string cooldownText = $"Cooldown: {effectiveCooldown:F1}s";
			if (efficiency > 1.0f)
			{
				cooldownText += $" (x{efficiency:F1} eff)";
				playerCooldownLabel.AddThemeColorOverride("font_color", Colors.Gold);
			}
			else
			{
				playerCooldownLabel.AddThemeColorOverride("font_color", Colors.LightBlue);
			}
			
			playerCooldownLabel.Text = cooldownText;
			playerCooldownLabel.TooltipText = $"Base: {weapon.BaseCooldown:F1}s + Runes: {weapon.TotalCooldown - weapon.BaseCooldown:F1}s + Shop: -{speedReduction:F1}s = {actualCooldown:F1}s actual\nEffective: {effectiveCooldown:F1}s (min 0.2s), Efficiency: x{efficiency:F1}";
		}
		
		// Update new combat stats
		var statPanelChildren = statPanel.GetChildren();
		
		// HP (3rd child after damage and cooldown)
		if (statPanelChildren.Count > 2)
		{
			var hpLabel = statPanelChildren[2] as Label;
			if (hpLabel != null)
			{
				if (player.IsDead)
				{
					hpLabel.Text = "HP: DEAD";
					hpLabel.AddThemeColorOverride("font_color", Colors.DarkRed);
				}
				else
				{
					hpLabel.Text = $"HP: {player.CurrentHp}/{player.MaxHp}";
					// Color based on HP percentage
					float hpPercentage = (float)player.CurrentHp / player.MaxHp;
					if (hpPercentage > 0.7f)
						hpLabel.AddThemeColorOverride("font_color", Colors.Green);
					else if (hpPercentage > 0.3f)
						hpLabel.AddThemeColorOverride("font_color", Colors.Yellow);
					else
						hpLabel.AddThemeColorOverride("font_color", Colors.Red);
				}
			}
		}
		
		// Armor (4th child)
		if (statPanelChildren.Count > 3)
		{
			var armorLabel = statPanelChildren[3] as Label;
			if (armorLabel != null)
			{
				int shopArmorBonus = player.PlayerShop.Upgrades[3].Level; // Heavy Armor upgrade
				int totalArmor = player.Armor + shopArmorBonus;
				armorLabel.Text = $"Armor: {totalArmor}";
			}
		}
		
		// Armor Piercing (5th child)
		if (statPanelChildren.Count > 4)
		{
			var armorPiercingLabel = statPanelChildren[4] as Label;
			if (armorPiercingLabel != null)
			{
				float weaponArmorPiercing = player.PlayerInventory.EquippedWeapon?.ArmorPiercing ?? 0;
				float totalArmorPiercing = player.ArmorPiercing + weaponArmorPiercing;
				armorPiercingLabel.Text = $"Armor Piercing: {totalArmorPiercing}";
			}
		}
		
		// HP Regen (6th child)
		if (statPanelChildren.Count > 5)
		{
			var hpRegenLabel = statPanelChildren[5] as Label;
			if (hpRegenLabel != null)
			{
				hpRegenLabel.Text = $"HP Regen: {player.HpRegen:F1}/s";
			}
		}
		
		// Update resources
		if (playerSpiritLabel != null)
		{
			playerSpiritLabel.Text = $"Spirit Points: {player.PlayerInventory.SpiritPoints}";
		}
		
		if (playerGoldLabel != null)
		{
			playerGoldLabel.Text = $"Gold: {player.PlayerInventory.Gold}";
		}
	}
	
	public void UpdateEnemyStatPanel(Enemy enemy)
	{
		if (enemy == null || enemyStatPanel == null) return;
		
		// Get enemy stat labels
		var enemyLevelLabel = enemyStatPanel.GetNode<Label>("EnemyLevelLabel");
		var enemyHpLabel = enemyStatPanel.GetNode<Label>("EnemyHpLabel");
		var enemyDamageLabel = enemyStatPanel.GetNode<Label>("EnemyDamageLabel");
		var enemyArmorLabel = enemyStatPanel.GetNode<Label>("EnemyArmorLabel");
		var enemyAttackSpeedLabel = enemyStatPanel.GetNode<Label>("EnemyAttackSpeedLabel");
		var enemyHpRegenLabel = enemyStatPanel.GetNode<Label>("EnemyHpRegenLabel");
		
		// Update enemy stats
		if (enemyLevelLabel != null)
		{
			enemyLevelLabel.Text = $"Level: {enemy.PlayerLevel}";
		}
		
		if (enemyHpLabel != null)
		{
			enemyHpLabel.Text = $"HP: {enemy.CurrentHp}/{enemy.MaxHp}";
			// Color based on HP percentage
			float hpPercentage = (float)enemy.CurrentHp / enemy.MaxHp;
			if (hpPercentage > 0.7f)
				enemyHpLabel.AddThemeColorOverride("font_color", Colors.Red);
			else if (hpPercentage > 0.3f)
				enemyHpLabel.AddThemeColorOverride("font_color", Colors.Orange);
			else
				enemyHpLabel.AddThemeColorOverride("font_color", Colors.DarkRed);
		}
		
		if (enemyDamageLabel != null)
		{
			enemyDamageLabel.Text = $"Damage: {enemy.Damage}";
		}
		
		if (enemyArmorLabel != null)
		{
			enemyArmorLabel.Text = $"Armor: {enemy.Armor}";
		}
		
		if (enemyAttackSpeedLabel != null)
		{
			enemyAttackSpeedLabel.Text = $"Attack Speed: {enemy.AttackCooldown:F1}s";
		}
		
		if (enemyHpRegenLabel != null)
		{
			enemyHpRegenLabel.Text = $"HP Regen: {enemy.HpRegen:F1}/s";
		}
	}
}
