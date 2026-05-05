using Framework.Systems.Health;
using System;
using UnityEngine;

namespace Gameplay.Systems.Health
{
    // =========================================
    // SegmentedHealth
    // Total HP divided into equal segments.
    // Fires OnSegmentBroken each time a segment
    // is depleted — useful for phase changes,
    // visual feedback, or triggering mechanics.
    //
    // Use case: Destiny boss shields, Cuphead,
    //           fighting game super bars,
    //           multi-phase boss encounters
    // =========================================
    public class SegmentedHealth : IHealth
    {
        public int  Value    => _hp;
        public int  MaxValue => _maxHp;
        public bool IsDead   => _hp <= 0;

        public int CurrentSegment => _hp > 0
            ? Mathf.CeilToInt((float)_hp / _segmentSize)
            : 0;

        public int TotalSegments => _totalSegments;

        public event Action<int> OnChanged;
        public event Action      OnDeath;
        public event Action<int> OnSegmentBroken; // passes segments remaining

        private int _hp;
        private int _maxHp;
        private int _segmentSize;
        private int _totalSegments;
        private int _lastSegment;

        public SegmentedHealth(int maxHp, int segments)
        {
            _totalSegments = Mathf.Max(1, segments);
            _maxHp         = maxHp;
            _hp            = maxHp;
            _segmentSize   = Mathf.CeilToInt((float)maxHp / _totalSegments);
            _lastSegment   = _totalSegments;
        }

        public void Damage(int amount)
        {
            if (IsDead) return;

            _hp = Math.Max(0, _hp - amount);
            OnChanged?.Invoke(_hp);

            int currentSegment = CurrentSegment;

            if (currentSegment < _lastSegment)
            {
                _lastSegment = currentSegment;
                OnSegmentBroken?.Invoke(currentSegment);
            }

            if (IsDead)
                OnDeath?.Invoke();
        }

        public void Heal(int amount)
        {
            if (IsDead) return;
            _hp          = Math.Min(_maxHp, _hp + amount);
            _lastSegment = CurrentSegment;
            OnChanged?.Invoke(_hp);
        }

        public void Reset()
        {
            _hp          = _maxHp;
            _lastSegment = _totalSegments;
            OnChanged?.Invoke(_hp);
        }
    }
}