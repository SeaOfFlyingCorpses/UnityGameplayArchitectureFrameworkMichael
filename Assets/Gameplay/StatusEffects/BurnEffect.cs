using UnityEngine;
using Framework.Systems.Damage;

namespace Gameplay.StatusEffects
{
    // =========================================
    // BurnEffect
    // Deals fire damage over time.
    // Refreshes on re-apply (no stack by default).
    //
    // Usage:
    //   context.StatusEffects?.Apply(
    //       new BurnEffect(damage: 5, duration: 3f),
    //       context);
    // =========================================
    public class BurnEffect : StatusEffectBase
    {
        public override string Id          => "burn";
        public override string DisplayName => "Burning";

        private readonly int   _damagePerSecond;
        private float          _tickTimer;
        private const float    TickInterval = 1f;

        public BurnEffect(int damage = 5, float duration = 3f)
            : base(duration)
        {
            _damagePerSecond = damage;
        }

        protected override void OnTick(
            Framework.StateMachine.StateContext ctx, float dt)
        {
            _tickTimer += dt;

            if (_tickTimer < TickInterval) return;

            _tickTimer = 0f;

            ctx.HealthComp?.Damage(
                _damagePerSecond,
                ctx.Self.position);
        }
    }
}
