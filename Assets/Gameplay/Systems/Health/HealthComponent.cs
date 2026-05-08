using System;
using UnityEngine;
using Framework.Events;
using Framework.Events.Events.Gameplay;
using Framework.Systems.Damage;
using Framework.Systems.Health;

namespace Gameplay.Systems.Health
{
    public class HealthComponent : MonoBehaviour, IHealthComponent
    {
        public enum HealthType
        {
            Standard, Shielded, Regen, Armoured, Segmented,
            Overshield, Invincible, Elemental,
            ShieldedArmoured, ShieldedRegen, SegmentedArmoured, FullBoss
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

        [Header("Elemental Resistances")]
        [Tooltip("1.0=normal | 0.5=resistant | 1.5=weak | 0.0=immune")]
        public float fireResistance      = 1f;
        public float iceResistance       = 1f;
        public float lightningResistance = 1f;
        public float poisonResistance    = 1f;
        public float holyResistance      = 1f;
        public float darkResistance      = 1f;

        [Header("Settings")]
        [Tooltip("While true, no damage is taken. " +
                 "Use for spawn grace period or cutscenes.")]
        public bool isInvincible = false;

        // =========================================
        // Use Framework interface — zero Gameplay
        // dependency on concrete health types
        // =========================================
        private Framework.Systems.Health.IHealth _health;
        private Vector3 _lastHitPoint;
        private int     _previousValue;

        public event Action<Vector3> OnHit;
        public event Action          OnDeath;

        // =========================================
        // UNITY LIFECYCLE
        // =========================================
        private void Awake()
        {
            _health = BuildHealth();
            _previousValue = _health.Value;

            _health.OnChanged += OnHealthChanged;
            _health.OnDeath   += HandleDeath;
        }

        private void Update()
        {
            switch (_health)
            {
                case CompositeHealth   c: c.Tick(Time.deltaTime); break;
                case RegenHealth       r: r.Tick(Time.deltaTime); break;
                case OvershieldHealth  o: o.Tick(Time.deltaTime); break;
            }
        }

        // =========================================
        // IHealthComponent
        // =========================================
        public Framework.Systems.Health.IHealth GetHealth() => _health;

        public void Damage(int amount, Vector3 hitPoint)
        {
            if (isInvincible || _health.IsDead) return;

            _lastHitPoint = hitPoint;
            _health.Damage(amount);
            OnHit?.Invoke(hitPoint);
        }

        public void Heal(int amount)
        {
            if (_health.IsDead) return;
            _health.Heal(amount);
        }

        public void Reset()
        {
            _health.Reset();
            _previousValue = _health.Value;
        }

        public void SetInvincible(bool invincible)
        {
            isInvincible = invincible;
        }

        // =========================================
        // INTERNAL HANDLERS
        // =========================================
        private void OnHealthChanged(int newValue)
        {
            int delta = newValue - _previousValue;
            _previousValue = newValue;

            EventBus.Publish(new HealthChangedEvent(
                gameObject,
                newValue,
                delta,
                _health.MaxValue,
                _lastHitPoint));

            _lastHitPoint = Vector3.zero;
        }

        private void HandleDeath()
        {
            Debug.Log($"{gameObject.name} died.");
            OnDeath?.Invoke();
        }

        // =========================================
        // BUILD HEALTH
        // =========================================
        private Framework.Systems.Health.IHealth BuildHealth()
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
    }
}