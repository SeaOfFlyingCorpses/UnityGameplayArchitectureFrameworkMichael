using Framework.Systems.Health;
using System;
using UnityEngine;

namespace Gameplay.Systems.Health
{
    // =========================================
    // ShieldedHealth
    // Shield absorbs damage first.
    // When shield is depleted, remainder hits HP.
    // Shield does not regenerate automatically —
    // call RestoreShield() from a ShieldSystem.
    //
    // Use case: Halo, Overwatch tanks, sci-fi enemies
    // =========================================
    public class ShieldedHealth : IHealth
    {
        public int  Value    => _hp;
        public int  MaxValue => _maxHp;
        public bool IsDead   => _hp <= 0;

        public int  Shield    => _shield;
        public int  MaxShield => _maxShield;
        public bool ShieldUp  => _shield > 0;

        public event Action<int> OnChanged;
        public event Action      OnDeath;
        public event Action<int> OnShieldChanged;
        public event Action      OnShieldBroken;

        private int _hp;
        private int _maxHp;
        private int _shield;
        private int _maxShield;

        public ShieldedHealth(int maxHp, int maxShield)
        {
            _maxHp     = maxHp;
            _hp        = maxHp;
            _maxShield = maxShield;
            _shield    = maxShield;
        }

        public void Damage(int amount)
        {
            if (IsDead) return;

            // Shield absorbs first
            if (_shield > 0)
            {
                int absorbed  = Math.Min(_shield, amount);
                _shield      -= absorbed;
                amount       -= absorbed;

                OnShieldChanged?.Invoke(_shield);

                if (_shield == 0)
                    OnShieldBroken?.Invoke();
            }

            // Remainder hits HP
            if (amount > 0)
            {
                _hp = Math.Max(0, _hp - amount);
                OnChanged?.Invoke(_hp);

                if (IsDead)
                    OnDeath?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (IsDead) return;
            _hp = Math.Min(_maxHp, _hp + amount);
            OnChanged?.Invoke(_hp);
        }

        public void RestoreShield(int amount)
        {
            _shield = Math.Min(_maxShield, _shield + amount);
            OnShieldChanged?.Invoke(_shield);
        }

        public void Reset()
        {
            _hp     = _maxHp;
            _shield = _maxShield;
            OnChanged?.Invoke(_hp);
            OnShieldChanged?.Invoke(_shield);
        }
    }
}