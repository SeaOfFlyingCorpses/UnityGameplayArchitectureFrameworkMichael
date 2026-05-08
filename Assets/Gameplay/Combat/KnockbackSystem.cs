using UnityEngine;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Gameplay.Combat
{
    // =========================================
    // KnockbackSystem
    // Applies knockback force on hit.
    // Works for both 2D (Rigidbody2D) and
    // 3D (Rigidbody).
    //
    // Subscribes to HealthChangedEvent —
    // auto-applies knockback when agent is hit.
    // Zero coupling — no attacker reference needed.
    //
    // Setup:
    //   Add to any agent that should be knocked back
    //   Set force and duration in Inspector
    // =========================================
    public class KnockbackSystem : UnityEngine.MonoBehaviour
    {
        [Header("Knockback Settings")]
        public float force          = 8f;
        public float upwardForce    = 3f;
        public float duration       = 0.2f;

        [Header("2D Settings")]
        public bool  is2D           = false;

        private Rigidbody   _rb;
        private Rigidbody2D _rb2d;
        private bool        _isBeingKnockedBack;
        private float       _knockbackTimer;

        private Gameplay.Systems.Health.HealthComponent _health;

        private void Awake()
        {
            _rb    = GetComponent<Rigidbody>();
            _rb2d  = GetComponent<Rigidbody2D>();
            _health = GetComponent<Gameplay.Systems.Health.HealthComponent>();
        }

        private void OnEnable()
        {
            if (_health != null)
                _health.OnHit += OnHit;
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.OnHit -= OnHit;
        }

        private void Update()
        {
            if (!_isBeingKnockedBack) return;

            _knockbackTimer -= Time.deltaTime;

            if (_knockbackTimer <= 0f)
                _isBeingKnockedBack = false;
        }

        private void OnHit(Vector3 hitPoint)
        {
            Apply(hitPoint);
        }

        // =========================================
        // Apply — can be called from any system
        // =========================================
        public void Apply(Vector3 sourcePosition,
                          float   overrideForce = -1f)
        {
            float f = overrideForce > 0f ? overrideForce : force;

            Vector3 dir = (transform.position - sourcePosition).normalized;
            dir.y = 0f; // keep horizontal

            if (is2D || _rb2d != null)
            {
                if (_rb2d == null) return;
                Vector2 dir2D = new Vector2(dir.x, upwardForce);
                _rb2d.linearVelocity = Vector2.zero;
                _rb2d.AddForce(dir2D.normalized * f, ForceMode2D.Impulse);
            }
            else
            {
                if (_rb == null) return;
                Vector3 dir3D = dir + Vector3.up * upwardForce;
                _rb.linearVelocity = Vector3.zero;
                _rb.AddForce(dir3D.normalized * f, ForceMode.Impulse);
            }

            _isBeingKnockedBack = true;
            _knockbackTimer     = duration;
        }

        public bool IsBeingKnockedBack => _isBeingKnockedBack;
    }
}
