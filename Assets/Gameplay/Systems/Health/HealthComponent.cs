using System;
using UnityEngine;
using Framework.Events;
using Framework.Events.Events.Gameplay;
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

        [Header("Elemental")]
        [Tooltip("1.0=normal | 0.5=resistant | 1.5=weak | 0.0=immune")]
        public float fireResistance      = 1f;
        public float iceResistance       = 1f;
        public float lightningResistance = 1f;
        public float poisonResistance    = 1f;
        public float holyResistance      = 1f;
        public float darkResistance      = 1f;

        // Use Framework interface — zero Gameplay dependency on IHealth
        private Framework.Systems.Health.IHealth _health;
        private Vector3 _lastHitPoint;

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
            switch (_health)
            {
                case CompositeHealth c: c.Tick(Time.deltaTime); break;
                case RegenHealth r:     r.Tick(Time.deltaTime); break;
                case OvershieldHealth o: o.Tick(Time.deltaTime); break;
            }
        }

        private void HandleDeath()
        {
            Debug.Log($"{gameObject.name} DIED");
            OnDeath?.Invoke();
        }

        private void OnHealthChanged(int newValue)
        {
            EventBus.Publish(new HealthChangedEvent(
                gameObject, newValue, newValue, _health.MaxValue, _lastHitPoint));
            _lastHitPoint = Vector3.zero;
        }

        // IHealthComponent
        public Framework.Systems.Health.IHealth GetHealth() => _health;

        public void Damage(int amount, Vector3 hitPoint)
        {
            _lastHitPoint = hitPoint;
            _health.Damage(amount);
            OnHit?.Invoke(hitPoint);
        }

        private Framework.Systems.Health.IHealth BuildHealth()
        {
            switch (healthType)
            {
                case HealthType.Shielded:         return new ShieldedHealth(maxHealth, shieldAmount);
                case HealthType.Regen:            return new RegenHealth(maxHealth, regenRate, regenDelay);
                case HealthType.Armoured:         return new ArmouredHealth(maxHealth, armourFlat, armourPct);
                case HealthType.Segmented:        return new SegmentedHealth(maxHealth, segments);
                case HealthType.Overshield:       return new OvershieldHealth(maxHealth, overshieldDecayRate);
                case HealthType.Invincible:       return new InvincibleHealth(maxHealth);
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
                default: return new Health(maxHealth);
            }
        }
    }
}