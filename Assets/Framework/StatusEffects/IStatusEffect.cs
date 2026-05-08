namespace Framework.StatusEffects
{
    // =========================================
    // IStatusEffect
    // A timed effect applied to an agent.
    // Can stack, refresh, or be unique.
    //
    // OnApply   — fires when effect starts
    // Tick      — fires every frame while active
    // OnExpire  — fires when duration ends
    // =========================================
    public interface IStatusEffect
    {
        string  Id         { get; } // unique key e.g. "burn", "slow"
        string  DisplayName{ get; }
        float   Duration   { get; }
        float   Remaining  { get; }
        bool    IsExpired  { get; }
        bool    CanStack   { get; } // true = multiple instances allowed
        bool    CanRefresh { get; } // true = reapplying resets duration

        void OnApply  (Framework.StateMachine.StateContext context);
        void Tick     (Framework.StateMachine.StateContext context, float deltaTime);
        void OnExpire (Framework.StateMachine.StateContext context);
    }
}
