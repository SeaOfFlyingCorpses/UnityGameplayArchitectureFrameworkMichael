using Framework.Systems.Health;
using System;
using UnityEngine;

namespace Gameplay.Systems.Health
{
    // =========================================
    // OvershieldHealth
    // Allows healing above MaxValue as a temporary
    // overshield that decays over time.
    // Damage hits the overshield first, then HP.
    //
    // Use case: Apex Legends Phoenix Kit,
    //           WoW absorb shields, pickup-based
    //           temporary health boosts
    // =========================================
    public class OvershieldHealth : IHealth
    {
        public int  Value    => _hp;
        public int  MaxValue => _maxHp;
        public bool IsDead   => _hp <= 0;

        public int Overshield => _overshield;

        public event Action<int> OnChanged;
        public event Action      OnDeath;
        public event Action<int> OnOvershieldChanged;

        private int   _hp;
        private int   _maxHp;
        private int   _overshield;
        private float _decayRate;   // overshield lost per second

        public OvershieldHealth(int maxHp, float decayRate = 10f)
        {
            _maxHp     = maxHp;
            _hp        = maxHp;
            _decayRate = decayRate;
        }

        public void Damage(int amount)
        {
            if (IsDead) return;

            // Overshield absorbs first
            if (_overshield > 0)
            {
                int absorbed    = Math.Min(_overshield, amount);
                _overshield    -= absorbed;
                amount         -= absorbed;
                OnOvershieldChanged?.Invoke(_overshield);
            }

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

            int hpRoom = _maxHp - _hp;

            if (amount <= hpRoom)
            {
                // Normal heal — fills HP first
                _hp += amount;
                OnChanged?.Invoke(_hp);
            }
            else
            {
                // Fill HP then overflow into overshield
                _hp          = _maxHp;
                _overshield += (amount - hpRoom);
                OnChanged?.Invoke(_hp);
                OnOvershieldChanged?.Invoke(_overshield);
            }
        }

        // =========================================
        // TICK — call every frame to decay overshield
        // =========================================
        public void Tick(float deltaTime)
        {
            if (_overshield <= 0) return;

            _overshield = Math.Max(0,
                _overshield - Mathf.RoundToInt(_decayRate * deltaTime));

            OnOvershieldChanged?.Invoke(_overshield);
        }

        public void Reset()
        {
            _hp          = _maxHp;
            _overshield  = 0;
            OnChanged?.Invoke(_hp);
            OnOvershieldChanged?.Invoke(_overshield);
        }
    }
}