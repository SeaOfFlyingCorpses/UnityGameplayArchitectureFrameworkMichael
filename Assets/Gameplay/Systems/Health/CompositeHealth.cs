using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Systems.Health
{
    // =========================================
    // CompositeHealth
    // Chains multiple IHealth layers together.
    // Damage flows through layers in order —
    // each layer absorbs what it can, remainder
    // passes to the next.
    //
    // The LAST layer added is the real HP pool.
    // IsDead is true only when the last layer dies.
    //
    // Examples:
    //   Shield → Armour → HP
    //   Overshield → Shield → HP
    //   Armour → Segmented HP
    //
    // Usage:
    //   var health = new CompositeHealth()
    //       .Add(new ShieldedHealth(100, 50))
    //       .Add(new ArmouredHealth(200, armourFlat: 5));
    // =========================================
    public class CompositeHealth : IHealth
    {
        private readonly List<IHealth> _layers = new();

        // =========================================
        // IHealth — reads from the last (core) layer
        // =========================================
        public int  Value    => _layers.Count > 0 ? _layers[_layers.Count - 1].Value    : 0;
        public int  MaxValue => _layers.Count > 0 ? _layers[_layers.Count - 1].MaxValue : 0;
        public bool IsDead   => _layers.Count > 0 && _layers[_layers.Count - 1].IsDead;

        public event Action<int> OnChanged;
        public event Action      OnDeath;

        // =========================================
        // ADD — fluent builder
        // Add layers outer → inner:
        //   .Add(shield).Add(armour).Add(hp)
        // Damage hits shield first, then armour, then hp
        // =========================================
        public CompositeHealth Add(IHealth layer)
        {
            if (layer == null)
                return this;

            // Forward each layer's events outward
            layer.OnChanged += _ => OnChanged?.Invoke(Value);
            layer.OnDeath   += () =>
            {
                // Only fire global death when the last layer dies
                if (IsDead)
                    OnDeath?.Invoke();
            };

            _layers.Add(layer);
            return this;
        }

        // =========================================
        // DAMAGE — flows through layers in order
        // Each layer absorbs as much as it can.
        // Remainder passes to the next layer.
        // =========================================
        public void Damage(int amount)
        {
            if (IsDead || amount <= 0)
                return;

            for (int i = 0; i < _layers.Count; i++)
            {
                var layer = _layers[i];

                if (layer.IsDead)
                    continue;

                int before = layer.Value;
                layer.Damage(amount);
                int absorbed = before - layer.Value;
                amount      -= absorbed;

                if (amount <= 0)
                    break;
            }
        }

        // =========================================
        // HEAL — heals the last living layer
        // =========================================
        public void Heal(int amount)
        {
            if (IsDead)
                return;

            // Heal the innermost non-dead layer first
            for (int i = _layers.Count - 1; i >= 0; i--)
            {
                if (!_layers[i].IsDead)
                {
                    _layers[i].Heal(amount);
                    return;
                }
            }
        }

        // =========================================
        // RESET — resets all layers
        // =========================================
        public void Reset()
        {
            foreach (var layer in _layers)
                layer.Reset();
        }

        // =========================================
        // TICK — drives time-based layers
        // Call from HealthComponent.Update()
        // =========================================
        public void Tick(float deltaTime)
        {
            foreach (var layer in _layers)
            {
                switch (layer)
                {
                    case RegenHealth regen:
                        regen.Tick(deltaTime);
                        break;

                    case OvershieldHealth os:
                        os.Tick(deltaTime);
                        break;
                }
            }
        }

        // =========================================
        // LAYER ACCESS — for UI / debug
        // =========================================
        public T GetLayer<T>() where T : class, IHealth
        {
            foreach (var layer in _layers)
                if (layer is T match)
                    return match;

            return null;
        }
    }
}