using Framework.StatusEffects;

namespace Gameplay.StatusEffects
{
    // =========================================
    // StatusEffectBase
    // Abstract base — handles duration tracking.
    // Subclass this for every status effect.
    //
    // Override:
    //   OnApply()  — start visual/audio
    //   OnTick()   — apply damage/slow per frame
    //   OnExpire() — clean up
    // =========================================
    public abstract class StatusEffectBase : IStatusEffect
    {
        public abstract string Id          { get; }
        public abstract string DisplayName { get; }
        public virtual  bool   CanStack    => false;
        public virtual  bool   CanRefresh  => true;

        public float Duration  { get; }
        public float Remaining { get; private set; }
        public bool  IsExpired => Remaining <= 0f;

        protected StatusEffectBase(float duration)
        {
            Duration  = duration;
            Remaining = duration;
        }

        public void OnApply(Framework.StateMachine.StateContext context)
        {
            Remaining = Duration; // refresh on re-apply
            Apply(context);
        }

        public void Tick(
            Framework.StateMachine.StateContext context,
            float deltaTime)
        {
            if (IsExpired) return;

            Remaining -= deltaTime;
            OnTick(context, deltaTime);
        }

        public void OnExpire(Framework.StateMachine.StateContext context)
        {
            Expire(context);
        }

        // =========================================
        // OVERRIDE IN SUBCLASS
        // =========================================
        protected virtual void Apply  (Framework.StateMachine.StateContext ctx) { }
        protected virtual void OnTick (Framework.StateMachine.StateContext ctx, float dt) { }
        protected virtual void Expire (Framework.StateMachine.StateContext ctx) { }
    }
}
