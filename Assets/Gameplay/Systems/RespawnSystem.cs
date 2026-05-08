using System.Collections;
using UnityEngine;
using Framework.Core;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Gameplay.Systems
{
    // =========================================
    // RespawnSystem
    // Handles player death and respawn.
    // Attach to the Player GameObject.
    //
    // Modes:
    //   Checkpoint  — respawn at last checkpoint
    //   StageStart  — respawn at stage start
    //   Instant     — respawn immediately in place
    //
    // Subscribes to HealthComponent.OnDeath.
    // Fires RespawnEvent via EventBus.
    // Works with StageSystem — resets wave on
    // last life lost.
    // =========================================
    public class RespawnSystem : MonoBehaviour
    {
        public enum RespawnMode
        {
            Checkpoint,
            StageStart,
            Instant
        }

        [Header("Settings")]
        public RespawnMode mode          = RespawnMode.Checkpoint;
        public float       respawnDelay  = 3f;
        public int         maxLives      = 3;
        public bool        infiniteLives = false;

        [Header("Checkpoint")]
        public Transform   startPosition;

        private int       _lives;
        private Transform _lastCheckpoint;
        private bool      _isDead;

        private Gameplay.Systems.Health.HealthComponent _health;

        public int  Lives    => _lives;
        public bool GameOver => !infiniteLives && _lives <= 0;

        private void Awake()
        {
            _health = GetComponent<
                Gameplay.Systems.Health.HealthComponent>();
            _lives  = maxLives;

            if (startPosition == null)
                startPosition = transform;

            _lastCheckpoint = startPosition;
        }

        private void OnEnable()
        {
            if (_health != null)
                _health.OnDeath += OnDeath;
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.OnDeath -= OnDeath;
        }

        // =========================================
        // CHECKPOINT
        // Call from checkpoint trigger zone
        // =========================================
        public void SetCheckpoint(Transform checkpoint)
        {
            _lastCheckpoint = checkpoint;
            Debug.Log($"[RespawnSystem] Checkpoint set: " +
                      $"{checkpoint.name}");
        }

        // =========================================
        // DEATH HANDLER
        // =========================================
        private void OnDeath()
        {
            if (_isDead) return;
            _isDead = true;

            if (!infiniteLives)
                _lives--;

            if (GameOver)
            {
                EventBus.Publish(new GameOverEvent());
                return;
            }

            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            // Disable player during respawn delay
            var controller = GetComponent<
                Gameplay.Player.PlayerStateController>();
            if (controller != null) controller.enabled = false;

            yield return new WaitForSeconds(respawnDelay);

            // Move to respawn position
            Vector3 respawnPos = GetRespawnPosition();
            transform.position = respawnPos;

            // Restore health
            _health?.Reset();

            // Re-enable
            if (controller != null) controller.enabled = true;

            _isDead = false;

            EventBus.Publish(new RespawnEvent(
                transform, respawnPos, _lives));
        }

        private Vector3 GetRespawnPosition()
        {
            switch (mode)
            {
                case RespawnMode.Checkpoint:
                    return _lastCheckpoint != null
                        ? _lastCheckpoint.position
                        : startPosition.position;

                case RespawnMode.StageStart:
                    return startPosition != null
                        ? startPosition.position
                        : transform.position;

                default:
                    return transform.position;
            }
        }
    }
}
