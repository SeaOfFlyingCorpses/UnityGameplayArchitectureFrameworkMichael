using System.Collections.Generic;
using Framework.StatusEffects;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Gameplay.StatusEffects
{
    // =========================================
    // StatusEffectSystem
    // Plain C# — one instance per agent.
    // Created in AIController.BindSystems()
    // and PlayerStateController.
    //
    // Apply effects:
    //   context.StatusEffects.Apply(
    //       new BurnEffect(damage:5, duration:3f),
    //       context);
    //
    // The StatusEffectsAISystem calls Tick()
    // automatically each frame via AISystemManager.
    // =========================================
    public class StatusEffectSystem : IStatusEffectSystem
    {
        private readonly List<IStatusEffect> _active = new();
        private readonly List<IStatusEffect> _toRemove = new();

        public IReadOnlyList<IStatusEffect> Active => _active;

        public void Apply(
            IStatusEffect effect,
            Framework.StateMachine.StateContext context)
        {
            if (effect == null) return;

            // Check for existing effect with same id
            for (int i = 0; i < _active.Count; i++)
            {
                if (_active[i].Id != effect.Id) continue;

                if (effect.CanRefresh)
                {
                    // Refresh existing — call OnApply to reset timer
                    _active[i].OnApply(context);
                    return;
                }

                if (!effect.CanStack) return; // already applied, no stack
            }

            _active.Add(effect);
            effect.OnApply(context);

            EventBus.Publish(new StatusEffectAppliedEvent(
                effect.Id, effect.DisplayName, effect.Duration));
        }

        public void Remove(
            string effectId,
            Framework.StateMachine.StateContext context)
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (_active[i].Id != effectId) continue;
                _active[i].OnExpire(context);
                _active.RemoveAt(i);
                break;
            }
        }

        public bool Has(string effectId)
        {
            foreach (var e in _active)
                if (e.Id == effectId) return true;
            return false;
        }

        public void Clear(Framework.StateMachine.StateContext context)
        {
            foreach (var e in _active)
                e.OnExpire(context);
            _active.Clear();
        }

        public void Tick(
            Framework.StateMachine.StateContext context,
            float deltaTime)
        {
            _toRemove.Clear();

            foreach (var effect in _active)
            {
                effect.Tick(context, deltaTime);

                if (effect.IsExpired)
                    _toRemove.Add(effect);
            }

            foreach (var expired in _toRemove)
            {
                expired.OnExpire(context);
                _active.Remove(expired);

                EventBus.Publish(new StatusEffectExpiredEvent(
                    expired.Id, expired.DisplayName));
            }
        }
    }
}
