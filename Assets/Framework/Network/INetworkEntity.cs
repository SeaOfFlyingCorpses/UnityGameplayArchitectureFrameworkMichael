namespace Framework.Network
{
    // =========================================
    // INetworkEntity
    // Any object that has a network identity.
    // Implemented by players, AI, projectiles,
    // pickups — anything that needs sync.
    //
    // IsOwner — true if this machine controls
    //   this entity. Use to gate input reading.
    //   Single player: always true.
    //   Multiplayer: true only on owner's machine.
    //
    // IsServer — true if running on the server.
    //   Use to gate authoritative logic (damage,
    //   AI decisions, spawn logic).
    //
    // NetworkId — unique id across all clients.
    //   Assigned by network layer on spawn.
    // =========================================
    public interface INetworkEntity
    {
        int  NetworkId  { get; }
        bool IsOwner    { get; }
        bool IsServer   { get; }
        bool IsLocalPlayer { get; }

        void OnNetworkSpawn  ();
        void OnNetworkDespawn();
    }
}
