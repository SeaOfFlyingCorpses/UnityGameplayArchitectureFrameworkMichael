using UnityEngine;
using Framework.Events;
using Framework.Events.Events.Gameplay;
using Framework.Systems.Health;
using Framework.Core;

namespace Gameplay.Systems.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [Header("Settings")]
        public float speed    = 15f;
        public float lifetime = 3f;
        public int   damage   = 10;

        [Header("Pool")]
        public string poolKey = "Bullet";

        [Header("Layer")]
        public LayerMask hitLayers;

        private Vector3  _direction;
        private float    _timer;
        private bool     _active;
        private float    _activationDelay = 0.05f; // grace period before collision
        private float    _activationTimer;
        private Collider _col;

        private void Awake()
        {
            _col = GetComponent<Collider>();
        }

        public void Launch(Vector3 origin, Vector3 direction, int overrideDamage = -1)
        {
            transform.position = origin;
            transform.forward  = direction.normalized;

            _direction       = direction.normalized;
            _timer           = 0f;
            _activationTimer = 0f;
            _active          = true;

            // Disable collider briefly so spawn-point overlap
            // doesn't immediately return bullet to pool
            if (_col != null)
                _col.enabled = false;

            if (overrideDamage > 0)
                damage = overrideDamage;
        }

        private void Update()
        {
            if (!_active) return;

            // Re-enable collider after grace period
            if (_col != null && !_col.enabled)
            {
                _activationTimer += Time.deltaTime;
                if (_activationTimer >= _activationDelay)
                    _col.enabled = true;
            }

            transform.position += _direction * speed * Time.deltaTime;

            _timer += Time.deltaTime;
            if (_timer >= lifetime)
                ReturnToPool();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_active) return;
            if (_col != null && !_col.enabled) return;

            if (((1 << other.gameObject.layer) & hitLayers) == 0)
                return;

            var healthComp = other.GetComponent<IHealthComponent>()
                          ?? other.GetComponentInParent<IHealthComponent>();

            if (healthComp != null)
            {
                healthComp.Damage(damage, transform.position);

                EventBus.Publish(new ProjectileHitEvent(
                    source:    gameObject,
                    target:    other.gameObject,
                    hitPoint:  transform.position,
                    hitNormal: -_direction,
                    damage:    damage));
            }

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            _active = false;

            if (_col != null)
                _col.enabled = true; // restore for next use

            var registry = ServiceLocator.Get<PoolRegistry>();

            if (registry != null && registry.HasPool(poolKey))
                registry.Return(poolKey, gameObject);
            else
                gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _active = false;
            _timer  = 0f;
        }
    }
}