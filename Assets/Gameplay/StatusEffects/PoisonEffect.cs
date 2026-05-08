namespace Gameplay.StatusEffects
{
    // =========================================
    // PoisonEffect
    // Deals poison damage over time.
    // Can stack — multiple poison sources
    // each deal their own damage.
    //
    // Usage:
    //   context.StatusEffects?.Apply(
    //       new PoisonEffect(damage: 3, duration: 5f),
    //       context);
    // =========================================
    public class PoisonEffect : StatusEffectBase
    {
        public override string Id          => "poison";
        public override string DisplayName => "Poisoned";
        public override bool   CanStack    => true;

        private readonly int   _damagePerSecond;
        private float          _tickTimer;
        private const float    TickInterval = 1f;

        public PoisonEffect(int damage = 3, float duration = 5f)
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
