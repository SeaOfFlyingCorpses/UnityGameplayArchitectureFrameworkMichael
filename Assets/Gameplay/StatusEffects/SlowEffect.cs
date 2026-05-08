namespace Gameplay.StatusEffects
{
    // =========================================
    // SlowEffect
    // Reduces agent movement speed by a
    // percentage. MoveCommand multiplies speed
    // by context.SpeedMultiplier automatically.
    //
    // Add SpeedMultiplier to StateContext and
    // read it in MoveCommand.Execute().
    // =========================================
    public class SlowEffect : StatusEffectBase
    {
        public override string Id          => "slow";
        public override string DisplayName => "Slowed";

        private readonly float _speedMultiplier;
        private float          _previousMultiplier;

        // slowAmount 0.5 = half speed, 0.3 = 70% reduction
        public SlowEffect(
            float slowAmount = 0.5f,
            float duration   = 3f)
            : base(duration)
        {
            _speedMultiplier = slowAmount;
        }

        protected override void Apply(
            Framework.StateMachine.StateContext ctx)
        {
            _previousMultiplier   = ctx.SpeedMultiplier;
            ctx.SpeedMultiplier   = _speedMultiplier;
        }

        protected override void Expire(
            Framework.StateMachine.StateContext ctx)
        {
            // Restore previous speed
            ctx.SpeedMultiplier = _previousMultiplier;
        }
    }
}
