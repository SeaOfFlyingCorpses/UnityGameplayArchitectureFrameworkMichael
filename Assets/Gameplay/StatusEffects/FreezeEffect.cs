namespace Gameplay.StatusEffects
{
    // =========================================
    // FreezeEffect
    // Stops agent movement completely.
    // Writes to context — movement strategies
    // check context.IsFrozen before moving.
    // =========================================
    public class FreezeEffect : StatusEffectBase
    {
        public override string Id          => "freeze";
        public override string DisplayName => "Frozen";

        public FreezeEffect(float duration = 2f)
            : base(duration) { }

        protected override void Apply(
            Framework.StateMachine.StateContext ctx)
        {
            ctx.Movement?.Stop(ctx.Self);
        }

        protected override void OnTick(
            Framework.StateMachine.StateContext ctx, float dt)
        {
            // Keep stopping movement each frame
            ctx.Movement?.Stop(ctx.Self);
        }

        protected override void Expire(
            Framework.StateMachine.StateContext ctx) { }
    }
}
