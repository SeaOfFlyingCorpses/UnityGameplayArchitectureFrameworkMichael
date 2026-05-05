using System;

namespace Gameplay.Systems.Health
{
    // =========================================
    // InvincibleHealth
    // Absorbs all damage silently. Never dies.
    // Events never fire.
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

        public event Action<int> OnChanged;
        public event Action      OnDeath;

        public InvincibleHealth(int maxValue = 9999)
        {
            MaxValue = maxValue;
        }

        public void Damage(int amount) { }  // silently absorbed
        public void Heal(int amount)   { }
        public void Reset()            { }
    }
}
