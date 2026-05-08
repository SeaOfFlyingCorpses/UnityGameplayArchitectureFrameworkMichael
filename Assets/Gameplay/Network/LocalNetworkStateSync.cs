using UnityEngine;
using Framework.Network;

namespace Gameplay.Network
{
    // =========================================
    // LocalNetworkStateSync
    // Single player no-op implementation.
    // Capture returns current transform state.
    // Apply and Interpolate do nothing.
    //
    // Attach alongside LocalNetworkEntity.
    // When adding multiplayer:
    //   Replace with network-library specific sync.
    // =========================================
    public class LocalNetworkStateSync : MonoBehaviour,
        INetworkStateSync
    {
        private Rigidbody  _rb;
        private int        _tick;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public NetworkStateSnapshot Capture()
        {
            return new NetworkStateSnapshot
            {
                Tick         = _tick++,
                Position     = transform.position,
                Rotation     = transform.rotation,
                Velocity     = _rb != null
                               ? _rb.linearVelocity
                               : Vector3.zero,
                Health       = -1,
                CurrentState = null
            };
        }

        // Single player — no sync needed
        public void Apply(NetworkStateSnapshot snapshot) { }

        public void Interpolate(
            NetworkStateSnapshot from,
            NetworkStateSnapshot to,
            float t) { }
    }
}
