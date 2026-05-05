using System;

namespace Gameplay.Systems.Health
{
    // =========================================
    // IHealth
    // Contract for any health implementation.
    // Implement this to create:
    //   - ShieldedHealth  (absorbs damage first)
    //   - RegenHealth     (regenerates over time)
    //   - InvincibleHealth (tutorial / cutscene)
    //   - MockHealth      (unit tests)
    // =========================================
    public interface IHealth
    {
        int   Value    { get; }
        int   MaxValue { get; }
        bool  IsDead   { get; }

        // =========================================
        // EVENTS
        // OnChanged — fires with current value after damage/heal
        // OnDeath   — fires once when Value reaches zero
        // =========================================
        event Action<int> OnChanged;
        event Action      OnDeath;

        void Damage(int amount);
        void Heal(int amount);
        void Reset();
    }
}