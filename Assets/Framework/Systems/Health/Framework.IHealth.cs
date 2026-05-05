using System;

namespace Framework.Systems.Health
{
    public interface IHealth
    {
        int   Value    { get; }
        int   MaxValue { get; }
        bool  IsDead   { get; }
        event Action<int> OnChanged;
        event Action      OnDeath;
        void Damage(int amount);
        void Heal(int amount);
        void Reset();
    }
}
