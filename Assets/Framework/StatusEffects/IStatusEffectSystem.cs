using System.Collections.Generic;

namespace Framework.StatusEffects
{
    // =========================================
    // IStatusEffectSystem
    // Manages active effects on a single agent.
    // Each agent has its own instance.
    //
    // Usage:
    //   context.StatusEffects?.Apply(new BurnEffect());
    //   context.StatusEffects?.Remove("burn");
    //   context.StatusEffects?.Has("slow");
    // =========================================
    public interface IStatusEffectSystem
    {
        IReadOnlyList<IStatusEffect> Active { get; }

        void Apply   (IStatusEffect effect,
                      Framework.StateMachine.StateContext context);
        void Remove  (string effectId,
                      Framework.StateMachine.StateContext context);
        bool Has     (string effectId);
        void Clear   (Framework.StateMachine.StateContext context);
        void Tick    (Framework.StateMachine.StateContext context,
                      float deltaTime);
    }
}
