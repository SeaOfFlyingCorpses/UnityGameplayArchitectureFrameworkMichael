using UnityEngine;
using Framework.Core;

namespace Gameplay.Network
{
    // =========================================
    // OfflineTransport
    // Single player implementation of
    // INetworkTransport.
    // Always returns IsServer=true, IsOwner=true.
    // Zero network traffic — just stubs.
    //
    // This is the DEFAULT transport.
    // Add to _GameSystems and it registers
    // itself with ServiceLocator.
    //
    // To add multiplayer later:
    //   Remove OfflineTransport
    //   Add MirrorTransport (or NGO/Photon)
    //   Zero other code changes needed
    // =========================================
    public class OfflineTransport : MonoBehaviour,
        Framework.Network.INetworkTransport
    {
        public bool IsConnected  => true;
        public bool IsServer     => true;
        public bool IsClient     => true;
        public bool IsHost       => true;
        public int  LocalClientId => 0;

        private void Awake()
        {
            ServiceLocator.Register<
                Framework.Network.INetworkTransport>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<
                Framework.Network.INetworkTransport>();
        }

        public void StartHost()   { }
        public void StartServer() { }
        public void StartClient(string address) { }
        public void Stop()        { }
    }
}
