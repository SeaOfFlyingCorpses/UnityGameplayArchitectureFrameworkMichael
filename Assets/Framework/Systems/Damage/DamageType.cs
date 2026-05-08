namespace Framework.Systems.Damage
{
    // =========================================
    // DamageType
    // Standardized damage categories.
    // Used by IDamageSource, ElementalHealth,
    // and status effects.
    //
    // Physical types affect armour.
    // Elemental types affect resistances.
    // True damage bypasses everything.
    // =========================================
    public enum DamageType
    {
        // Physical
        Physical,   // standard melee/ranged
        Slash,      // swords, claws
        Pierce,     // arrows, spears
        Blunt,      // hammers, fists

        // Elemental
        Fire,
        Ice,
        Lightning,
        Poison,
        Acid,

        // Special
        True,       // bypasses all resistances and armour
        Heal,       // negative damage (healing)
        Pure,       // bypasses shields only

        // Magic
        Holy,
        Dark,
        Arcane,
        Nature
    }
}