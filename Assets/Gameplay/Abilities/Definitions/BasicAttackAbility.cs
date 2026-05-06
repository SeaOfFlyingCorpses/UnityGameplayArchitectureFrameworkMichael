using Gameplay.Systems.Health;

namespace Gameplay.Abilities.Definitions
{
    // =========================================
    // BasicAttackAbility
    // Standard melee/ranged attack.
    // All values configurable — no hardcoding.
    //
    // Usage:
    //   BasicAttackAbility.Create()           // defaults
    //   BasicAttackAbility.Create(damage: 25) // custom damage
    //   BasicAttackAbility.Create(damage: 15, cooldown: 0.8f, id: "HeavyAttack")
    // =========================================
    public static class BasicAttackAbility
    {
        public static Ability Create(
            int    damage   = 10,
            float  cooldown = 1.2f,
            string id       = "Attack")
        {
            return new Ability
            {
                Id       = id,
                Cooldown = cooldown,
                Execute  = (ctx) =>
                {
                    if (ctx.TargetHealth != null)
                        ctx.TargetHealth.Damage(damage, ctx.Target.position);
                }
            };
        }
    }
}