using System;
using UnityEngine;

namespace Gameplay.Systems.Health
{
    public class HealthComponent : MonoBehaviour
    {
        public enum HealthType
        {
            Standard,
            Shielded,
            Regen,
            Armoured,
            Segmented,
            Overshield,
            Invincible,
            Elemental,

            // =========================================
            // COMPOSITE PRESETS
            // Mix multiple layers — damage flows through
            // outer layers first, core HP last
            // =========================================
            ShieldedArmoured,     // Shield → Armoured HP
            ShieldedRegen,        // Shield → Regen HP
            SegmentedArmoured,    // Armoured → Segmented HP (boss preset)
            FullBoss              // Shield → Armoured → Segmented HP
        }

        [Header("Health Type")]
        public HealthType healthType = HealthType.Standard;

        [Header("Base Stats")]
        public int maxHealth = 100;

        [Header("Shielded")]
        public int shieldAmount = 50;

        [Header("Regen")]
        public float regenRate  = 5f;
        public float regenDelay = 3f;

        [Header("Armoured")]
        public int   armourFlat = 3;
        [Range(0f, 0.9f)]
        public float armourPct  = 0.2f;

        [Header("Segmented")]
        public int segments = 5;

        [Header("Overshield")]
        public float overshieldDecayRate = 10f;

        [Header("Elemental")]
        [Tooltip("1.0 = normal | 0.5 = resistant | 1.5 = weak | 0.0 = immune")]
        public float fireResistance      = 1f;
        public float iceResistance       = 1f;
        public float lightningResistance = 1f;
        public float poisonResistance    = 1f;
        public float holyResistance      = 1f;
        public float darkResistance      = 1f;

        private IHealth _health;

        public event Action<Vector3> OnHit;
        public event Action          OnDeath;

        private void Awake()
        {
            _health = BuildHealth();

            _health.OnChanged += OnHealthChanged;
            _health.OnDeath   += HandleDeath;
        }

        private void Update()
        {
            // Tick time-based types
            switch (_health)
            {
                case CompositeHealth composite:
                    composite.Tick(Time.deltaTime);
                    break;

                case RegenHealth regen:
                    regen.Tick(Time.deltaTime);
                    break;

                case OvershieldHealth overshield:
                    overshield.Tick(Time.deltaTime);
                    break;
            }
        }

        private IHealth BuildHealth()
        {
            switch (healthType)
            {
                case HealthType.Shielded:
                    return new ShieldedHealth(maxHealth, shieldAmount);

                case HealthType.Regen:
                    return new RegenHealth(maxHealth, regenRate, regenDelay);

                case HealthType.Armoured:
                    return new ArmouredHealth(maxHealth, armourFlat, armourPct);

                case HealthType.Segmented:
                    return new SegmentedHealth(maxHealth, segments);

                case HealthType.Overshield:
                    return new OvershieldHealth(maxHealth, overshieldDecayRate);

                case HealthType.Invincible:
                    return new InvincibleHealth(maxHealth);

                case HealthType.Elemental:
                    var e = new ElementalHealth(maxHealth);
                    e.SetResistance(DamageType.Fire,      fireResistance);
                    e.SetResistance(DamageType.Ice,       iceResistance);
                    e.SetResistance(DamageType.Lightning, lightningResistance);
                    e.SetResistance(DamageType.Poison,    poisonResistance);
                    e.SetResistance(DamageType.Holy,      holyResistance);
                    e.SetResistance(DamageType.Dark,      darkResistance);
                    return e;

                // =========================================
                // COMPOSITE PRESETS
                // =========================================
                case HealthType.ShieldedArmoured:
                    return new CompositeHealth()
                        .Add(new ShieldedHealth(shieldAmount, shieldAmount))
                        .Add(new ArmouredHealth(maxHealth, armourFlat, armourPct));

                case HealthType.ShieldedRegen:
                    return new CompositeHealth()
                        .Add(new ShieldedHealth(shieldAmount, shieldAmount))
                        .Add(new RegenHealth(maxHealth, regenRate, regenDelay));

                case HealthType.SegmentedArmoured:
                    return new CompositeHealth()
                        .Add(new ArmouredHealth(maxHealth, armourFlat, armourPct))
                        .Add(new SegmentedHealth(maxHealth, segments));

                case HealthType.FullBoss:
                    return new CompositeHealth()
                        .Add(new ShieldedHealth(shieldAmount, shieldAmount))
                        .Add(new ArmouredHealth(maxHealth, armourFlat, armourPct))
                        .Add(new SegmentedHealth(maxHealth, segments));

                default:
                    return new Health(maxHealth);
            }
        }

        private void HandleDeath()
        {
            Debug.Log($"{gameObject.name} DIED");
            OnDeath?.Invoke();
        }

        private void OnHealthChanged(int value) { }

        public IHealth GetHealth() => _health;

        public void Damage(int amount, Vector3 hitPoint)
        {
            _health.Damage(amount);
            OnHit?.Invoke(hitPoint);
        }
    }
}