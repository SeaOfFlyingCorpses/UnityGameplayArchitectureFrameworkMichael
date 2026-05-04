using Gameplay.Systems.Health;

namespace Gameplay.Abilities.Definitions
{
    public static class BasicAttackAbility
    {
        public static Ability Create()
        {
            return new Ability
            {
                Id = "Attack",
                Cooldown = 1.2f,
                Execute = (ctx) =>
                {
                    if (ctx.TargetHealth != null)
                    {
                        ctx.TargetHealth.Damage(10, ctx.Target.position);
                    }
                }
            };
        }
    }
}