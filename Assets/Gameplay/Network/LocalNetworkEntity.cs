using UnityEngine;
using Framework.Network;

namespace Gameplay.Network
{
    // =========================================
    // LocalNetworkEntity
    // Single player implementation of
    // INetworkEntity.
    // Always IsOwner=true, IsServer=true.
    //
    // Attach to Player, enemies, projectiles
    // for future multiplayer compatibility.
    //
    // When adding multiplayer:
    //   Replace with NetworkObject (Mirror/NGO)
    //   or PunView (Photon).
    //   Your game code reads INetworkEntity —
    //   zero changes needed.
    // =========================================
    public class LocalNetworkEntity : MonoBehaviour, INetworkEntity
    {
        [Header("Network Identity")]
        [Tooltip("Unique id — assign manually " +
                 "or let NetworkManager assign at runtime")]
        public int networkId;

        public int  NetworkId      => networkId;
        public bool IsOwner        => true;
        public bool IsServer       => true;
        public bool IsLocalPlayer  => true;

        private void Awake()
        {
            OnNetworkSpawn();
        }

        private void OnDestroy()
        {
            OnNetworkDespawn();
        }

        public void OnNetworkSpawn()   { }
        public void OnNetworkDespawn() { }
    }
}
