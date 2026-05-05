using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Systems.Health
{
    // =========================================
    // Damage types — extend freely
    // =========================================
    public enum DamageType
    {
        Physical,
        Fire,
        Ice,
        Lightning,
        Poison,
        Holy,
        Dark
    }

    // =========================================
    // ElementalHealth
    // Applies damage multipliers per damage type.
    // resistance < 1.0 = takes less damage (resistant)
    // resistance > 1.0 = takes more damage (weak)
    // resistance = 0.0 = immune
    //
    // Use case: Pokémon, Dark Souls, Monster Hunter,
    //           any RPG with elemental systems
    // =========================================
    public class ElementalHealth : IHealth
    {
        public int  Value    => _hp;
        public int  MaxValue => _maxHp;
        public bool IsDead   => _hp <= 0;

        public event Action<int> OnChanged;
        public event Action      OnDeath;

        private int _hp;
        private int _maxHp;

        private readonly Dictionary<DamageType, float> _resistances = new();

        public ElementalHealth(int maxHp)
        {
            _maxHp = maxHp;
            _hp    = maxHp;
        }

        // =========================================
        // SET RESISTANCE
        // 0.5 = 50% damage taken (resistant)
        // 1.5 = 150% damage taken (weak)
        // 0.0 = immune
        // =========================================
        public void SetResistance(DamageType type, float multiplier)
        {
            _resistances[type] = Mathf.Max(0f, multiplier);
        }

        // =========================================
        // DAMAGE WITH TYPE
        // =========================================
        public void Damage(DamageType type, int amount)
        {
            if (IsDead) return;

            float multiplier = _resistances.TryGetValue(type, out var r) ? r : 1f;
            int   final      = Mathf.Max(1, Mathf.RoundToInt(amount * multiplier));

            Damage(final);
        }

        // =========================================
        // IHealth.Damage — treated as Physical
        // =========================================
        public void Damage(int amount)
        {
            if (IsDead) return;

            _hp = Math.Max(0, _hp - amount);
            OnChanged?.Invoke(_hp);

            if (IsDead)
                OnDeath?.Invoke();
        }

        public void Heal(int amount)
        {
            if (IsDead) return;
            _hp = Math.Min(_maxHp, _hp + amount);
            OnChanged?.Invoke(_hp);
        }

        public void Reset()
        {
            _hp = _maxHp;
            OnChanged?.Invoke(_hp);
        }
    }
}
