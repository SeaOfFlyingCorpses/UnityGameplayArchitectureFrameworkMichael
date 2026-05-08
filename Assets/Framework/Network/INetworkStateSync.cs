namespace Framework.Network
{
    // =========================================
    // INetworkStateSync
    // Any component that needs to synchronize
    // state across the network.
    //
    // Capture — called on server to get state
    // Apply   — called on clients to set state
    //
    // Implementations:
    //   LocalNetworkStateSync — no-op, single player
    //   MirrorNetworkStateSync — syncs via Mirror
    //   NGONetworkStateSync    — syncs via NGO
    // =========================================
    public interface INetworkStateSync
    {
        NetworkStateSnapshot Capture ();
        void                 Apply   (NetworkStateSnapshot snapshot);
        void                 Interpolate (
            NetworkStateSnapshot from,
            NetworkStateSnapshot to,
            float t);
    }
}
