using UnityEngine;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Gameplay.Systems
{
    // =========================================
    // CheckpointTrigger
    // Place in level — player walks through it
    // to set their respawn point.
    //
    // Setup:
    //   1. Add empty GameObject
    //   2. Add BoxCollider — Is Trigger ON
    //   3. Add this component
    //   4. Set Player Tag
    // =========================================
    public class CheckpointTrigger : MonoBehaviour
    {
        public string playerTag = "Player";
        public bool   activateOnce = true;

        private bool _activated;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            if (activateOnce && _activated) return;

            _activated = true;

            var respawn = other.GetComponent<RespawnSystem>();
            respawn?.SetCheckpoint(transform);

            EventBus.Publish(
                new CheckpointReachedEvent(transform));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _activated
                ? Color.green
                : new Color(1f, 1f, 0f, 0.4f);

            var col = GetComponent<BoxCollider>();
            if (col != null)
                Gizmos.DrawWireCube(
                    transform.position + col.center,
                    col.size);
        }
    }
}
