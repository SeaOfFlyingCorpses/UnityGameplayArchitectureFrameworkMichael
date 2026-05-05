using Framework.Systems.Health;
using System;
using UnityEngine;

namespace Gameplay.Systems.Health
{
    // =========================================
    // ArmouredHealth
    // Reduces all incoming damage by armour value.
    // Supports flat reduction and percentage reduction.
    // Minimum 1 damage always gets through.
    //
    // Use case: RPGs, souls-likes, strategy games,
    //           tank units, heavily armoured enemies
    // =========================================
    public class ArmouredHealth : IHealth
    {
        public int  Value    => _hp;
        public int  MaxValue => _maxHp;
        public bool IsDead   => _hp <= 0;

        public int   Armour     => _armour;
        public float ArmourPct  => _armourPct;

        public event Action<int> OnChanged;
        public event Action      OnDeath;

        private int   _hp;
        private int   _maxHp;
        private int   _armour;       // flat reduction per hit
        private float _armourPct;    // 0.0 to 1.0 — percentage reduction

        // =========================================
        // armourPct = 0.3 means 30% of damage blocked
        // armour    = 5   means 5 flat damage blocked per hit
        // Both stack — percentage applied first, then flat
        // =========================================
        public ArmouredHealth(int maxHp, int armour = 0, float armourPct = 0f)
        {
            _maxHp    = maxHp;
            _hp       = maxHp;
            _armour   = armour;
            _armourPct = Mathf.Clamp01(armourPct);
        }

        public void Damage(int amount)
        {
            if (IsDead) return;

            // Percentage reduction first
            float reduced = amount * (1f - _armourPct);

            // Flat reduction
            int final = Mathf.Max(1, (int)reduced - _armour);

            _hp = Math.Max(0, _hp - final);
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

        public void SetArmour(int armour)
        {
            _armour = Math.Max(0, armour);
        }

        public void Reset()
        {
            _hp = _maxHp;
            OnChanged?.Invoke(_hp);
        }
    }
}