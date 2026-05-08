namespace Gameplay.StatusEffects
{
    // =========================================
    // StunEffect
    // Stops movement AND prevents actions.
    // Sets context.IsStunned which state machine
    // conditions can check.
    //
    // Add IsStunnedCondition to any state
    // transition to interrupt on stun.
    // =========================================
    public class StunEffect : StatusEffectBase
    {
        public override string Id          => "stun";
        public override string DisplayName => "Stunned";

        public StunEffect(float duration = 1.5f)
            : base(duration) { }

        protected override void Apply(
            Framework.StateMachine.StateContext ctx)
        {
            ctx.Movement?.Stop(ctx.Self);
            ctx.Commands?.Clear();
        }

        protected override void OnTick(
            Framework.StateMachine.StateContext ctx, float dt)
        {
            ctx.Movement?.Stop(ctx.Self);
            ctx.Commands?.Clear();
        }

        protected override void Expire(
            Framework.StateMachine.StateContext ctx) { }
    }
}
