using System;

namespace Gameplay.Systems.Health
{
    // =========================================
    // InvincibleHealth
    // Absorbs all damage silently. Never dies.
    // Events never fire — intentional.
    //
    // Use case: tutorial sequences, cutscene actors,
    //           invulnerability frames (i-frames),
    //           debugging / God mode,
    //           scripted events where death is not
    //           allowed until a trigger fires
    // =========================================
    public class InvincibleHealth : IHealth
    {
        public int  Value    => MaxValue;
        public int  MaxValue { get; }
        public bool IsDead   => false;

        // Intentionally never fired — invincible health
        // never changes value and never dies.
        // Pragma suppresses the CS0067 warning cleanly.
#pragma warning disable CS0067
        public event Action<int> OnChanged;
        public event Action      OnDeath;
#pragma warning restore CS0067

        public InvincibleHealth(int maxValue = 9999)
        {
            MaxValue = maxValue;
        }

        public void Damage(int amount) { }
        public void Heal(int amount)   { }
        public void Reset()            { }
    }
}