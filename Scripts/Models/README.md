# Models Directory Structure

This directory contains all data models, enums, and data structures used throughout the game.

## **Folder Organization**

### **`Enums/`**
Contains all enumeration types used across the game:
- `GameEnums.cs` - Core game states and UI states
- `ItemEnums.cs` - Item types, rarity, equipment slots
- `CombatEnums.cs` - Damage types, status effects, enemy types

### **`Player/`**
Contains player-related data models:
- `PlayerData.cs` - Core player stats, progression, and abilities

### **`Combat/`**
Contains combat-related data models:
- `EnemyData.cs` - Enemy statistics, AI behavior, and rewards

### **`Items/`**
Contains item and inventory-related data models:
- `ItemData.cs` - Item properties, stats, and metadata

### **`Game/`**
Contains game-wide data models:
- `GameSettings.cs` - Configuration, settings, and session data

## **Design Principles**

1. **Separation of Concerns**: Each model handles only its specific data
2. **Immutable Where Possible**: Minimize side effects
3. **Signal-Based Updates**: Models emit signals when data changes
4. **Validation**: Models validate their own data integrity
5. **Self-Contained**: Models don't depend on other systems

## **Usage Examples**

```csharp
// Creating player data
var player = new PlayerData();
player.GainExperience(100);

// Creating an enemy
var enemy = new EnemyData(5, EnemyType.Elite);

// Creating an item
var sword = new ItemData("Fire Sword", ItemType.Weapon, ItemRarity.Rare);
```
