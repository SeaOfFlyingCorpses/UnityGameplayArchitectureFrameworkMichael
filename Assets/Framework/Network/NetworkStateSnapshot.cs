using UnityEngine;

namespace Framework.Network
{
    // =========================================
    // NetworkStateSnapshot
    // Captures the minimum state needed to
    // synchronize an entity across the network.
    //
    // Position, rotation, velocity, health.
    // Delta-compressed — only changed fields sent.
    //
    // Used by INetworkStateSync implementations.
    // =========================================
    public struct NetworkStateSnapshot
    {
        public int       Tick;
        public Vector3   Position;
        public Quaternion Rotation;
        public Vector3   Velocity;
        public int       Health;
        public string    CurrentState; // state machine state name

        // =========================================
        // Delta — only fields that changed
        // =========================================
        public static NetworkStateSnapshot Delta(
            NetworkStateSnapshot from,
            NetworkStateSnapshot to)
        {
            return new NetworkStateSnapshot
            {
                Tick         = to.Tick,
                Position     = to.Position  != from.Position
                               ? to.Position : Vector3.zero,
                Rotation     = to.Rotation  != from.Rotation
                               ? to.Rotation : Quaternion.identity,
                Velocity     = to.Velocity  != from.Velocity
                               ? to.Velocity : Vector3.zero,
                Health       = to.Health    != from.Health
                               ? to.Health : -1,
                CurrentState = to.CurrentState != from.CurrentState
                               ? to.CurrentState : null
            };
        }

        // =========================================
        // Apply delta onto a base snapshot
        // =========================================
        public static NetworkStateSnapshot Apply(
            NetworkStateSnapshot @base,
            NetworkStateSnapshot delta)
        {
            return new NetworkStateSnapshot
            {
                Tick         = delta.Tick,
                Position     = delta.Position  != Vector3.zero
                               ? delta.Position : @base.Position,
                Rotation     = delta.Rotation  != Quaternion.identity
                               ? delta.Rotation : @base.Rotation,
                Velocity     = delta.Velocity  != Vector3.zero
                               ? delta.Velocity : @base.Velocity,
                Health       = delta.Health    >= 0
                               ? delta.Health : @base.Health,
                CurrentState = delta.CurrentState != null
                               ? delta.CurrentState : @base.CurrentState
            };
        }
    }
}
