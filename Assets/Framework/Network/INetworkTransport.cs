namespace Framework.Network
{
    // =========================================
    // INetworkTransport
    // Abstracts the networking library.
    // Swap Mirror/NGO/Photon without touching
    // any game code.
    //
    // Implementations:
    //   OfflineTransport  — single player, always
    //                       returns IsServer=true,
    //                       IsOwner=true
    //   MirrorTransport   — wraps Mirror
    //   NGOTransport      — wraps Netcode for GO
    //   PhotonTransport   — wraps Photon Fusion
    // =========================================
    public interface INetworkTransport
    {
        bool IsConnected { get; }
        bool IsServer    { get; }
        bool IsClient    { get; }
        bool IsHost      { get; } // server + client on same machine

        void StartHost   ();
        void StartServer ();
        void StartClient (string address);
        void Stop        ();

        int  LocalClientId { get; }
    }
}
