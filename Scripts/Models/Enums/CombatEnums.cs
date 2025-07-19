/// <summary>
/// Types of damage that can be dealt
/// </summary>
public enum DamageType
{
    Physical,
    Magic,
    Fire,
    Ice,
    Lightning,
    Poison,
    True // Ignores armor
}

/// <summary>
/// Status effect types
/// </summary>
public enum StatusEffectType
{
    Poison,
    Burn,
    Freeze,
    Stun,
    Regeneration,
    Shield,
    AttackBoost,
    DefenseBoost
}

/// <summary>
/// Enemy types/categories
/// </summary>
public enum EnemyType
{
    Melee,
    Ranged,
    Magic,
    Boss,
    Elite
}
