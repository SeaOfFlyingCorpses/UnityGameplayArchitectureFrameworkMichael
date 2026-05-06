using UnityEngine;

namespace Gameplay.Abilities
{
    // =========================================
    // AbilityType — what kind of ability this is
    // =========================================
    public enum AbilityType
    {
        MeleeAttack,    // BasicAttackAbility
        RangedAttack,   // ProjectileAbility
    }

    // =========================================
    // AbilityDefinition
    // ScriptableObject — create one asset per
    // ability type and drag onto any agent.
    //
    // Create:
    //   Right click Project → Create →
    //   Gameplay → Ability Definition
    //
    // Usage:
    //   Create "SoldierMeleeAttack.asset"
    //   Create "ArcherRangedAttack.asset"
    //   Drag onto CombatAIStateFactory.Abilities[]
    // =========================================
    [CreateAssetMenu(
        fileName = "AbilityDefinition",
        menuName = "Gameplay/Ability Definition")]
    public class AbilityDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string     abilityId = "Attack";
        public AbilityType abilityType = AbilityType.MeleeAttack;

        [Header("Stats")]
        [Tooltip("Damage dealt per use")]
        public int   damage   = 10;

        [Tooltip("Seconds between uses")]
        public float cooldown = 1.2f;

        [Tooltip("Priority — higher = preferred when multiple abilities available")]
        public int   priority = 0;

        [Header("Ranged Only")]
        [Tooltip("Pool key for projectile — must match GameBootstrap pool entry")]
        public string projectilePoolKey = "Bullet";

        // =========================================
        // BUILD — creates the runtime Ability
        // =========================================
        public Ability Build()
        {
            switch (abilityType)
            {
                case AbilityType.RangedAttack:
                    return Definitions.ProjectileAbility.Create(
                        poolKey:  projectilePoolKey,
                        damage:   damage,
                        cooldown: cooldown,
                        id:       abilityId
                    );

                default: // MeleeAttack
                    return Definitions.BasicAttackAbility.Create(
                        damage:   damage,
                        cooldown: cooldown,
                        id:       abilityId
                    );
            }
        }
    }
}
