using Framework.Systems.Health;
using System;
using UnityEngine;

namespace Gameplay.Systems.Health
{
    // =========================================
    // RegenHealth
    // HP regenerates after a delay without damage.
    // Call Tick(deltaTime) from a MonoBehaviour
    // or an IAISystem each frame.
    //
    // Use case: Halo player, most modern FPS heroes,
    //           RPG characters with regen stat
    // =========================================
    public class RegenHealth : IHealth
    {
        public int  Value    => _hp;
        public int  MaxValue => _maxHp;
        public bool IsDead   => _hp <= 0;

        public event Action<int> OnChanged;
        public event Action      OnDeath;

        private int   _hp;
        private int   _maxHp;
        private float _regenRate;        // HP per second
        private float _regenDelay;       // seconds after damage before regen starts
        private float _timeSinceHit;
        private float _regenAccumulator;

        public RegenHealth(int maxHp, float regenRate = 5f, float regenDelay = 3f)
        {
            _maxHp     = maxHp;
            _hp        = maxHp;
            _regenRate = regenRate;
            _regenDelay = regenDelay;
        }

        public void Damage(int amount)
        {
            if (IsDead) return;

            _hp            = Math.Max(0, _hp - amount);
            _timeSinceHit  = 0f;
            _regenAccumulator = 0f;

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

        // =========================================
        // TICK — call every frame from outside
        // RegenHealthSystem or HealthComponent.Update
        // =========================================
        public void Tick(float deltaTime)
        {
            if (IsDead || _hp >= _maxHp) return;

            _timeSinceHit += deltaTime;

            if (_timeSinceHit < _regenDelay) return;

            _regenAccumulator += _regenRate * deltaTime;

            int regen = (int)_regenAccumulator;
            if (regen <= 0) return;

            _regenAccumulator -= regen;
            _hp = Math.Min(_maxHp, _hp + regen);

            OnChanged?.Invoke(_hp);
        }

        public void Reset()
        {
            _hp               = _maxHp;
            _timeSinceHit     = 0f;
            _regenAccumulator = 0f;
            OnChanged?.Invoke(_hp);
        }
    }
}