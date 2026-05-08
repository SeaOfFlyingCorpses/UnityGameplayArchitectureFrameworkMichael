# How To Add Co-op Multiplayer

## Step 1 — Install a library

In Unity Package Manager:
  Window → Package Manager → Add by name:
  com.unity.netcode.gameobjects

## Step 2 — Add NetworkManager to _GameSystems

  Add Component → NetworkManager
  Set Connection Data → localhost for LAN
  Or add Unity Relay for internet co-op

## Step 3 — Add NetworkObject to Player prefab

  Add Component → NetworkObject
  Add Component → NetworkTransform
    (position/rotation auto-syncs)

## Step 4 — Add NetworkObject to Enemy prefab

  Add Component → NetworkObject
  Add Component → NetworkTransform

## Step 5 — Gate AI on server only

  In AIController.Update():

  private void Update()
  {
      // Only server runs AI logic
      var net = GetComponent<INetworkEntity>();
      if (net != null && !net.IsServer) return;

      TickMovement();
      TickPerception();
      TickAISystems();
      TickStateMachine();
      ResetFrameFlags();
      ExecuteCommands();
  }

## Step 6 — Gate input on owner only

  In PlayerStateController.Update():

  private void Update()
  {
      // Only owner reads input
      var net = GetComponent<INetworkEntity>();
      if (net != null && !net.IsOwner) return;

      _stateMachine.Update();
      _context.Commands.ExecuteAll();
  }

## Step 7 — Make health networked

  Replace LocalNetworkEntity with NGONetworkEntity adapter:

  public class NGONetworkEntity : NetworkBehaviour, INetworkEntity
  {
      public int  NetworkId     => (int)NetworkObjectId;
      public bool IsOwner       => IsOwner;     // NGO property
      public bool IsServer      => IsServer;    // NGO property
      public bool IsLocalPlayer => IsLocalPlayer; // NGO property

      public void OnNetworkSpawn()   => base.OnNetworkSpawn();
      public void OnNetworkDespawn() => base.OnNetworkDespawn();
  }

  Register it:
  ServiceLocator.Register<INetworkEntity>(this);

## Step 8 — Sync health with NetworkVariable

  In HealthComponent add:
  NetworkVariable<int> _networkHealth = new NetworkVariable<int>();

  On server: _networkHealth.Value = _health.Value;
  On clients: display _networkHealth.Value in UI

## Result

  Two players can connect.
  Server runs all AI.
  Each client controls their own player.
  Health syncs automatically.
  Positions sync via NetworkTransform.

## What the framework already handles for you

  - AIController gates on IsServer (step 5 above)
  - PlayerStateController gates on IsOwner (step 6)
  - All game events go through EventBus (no direct refs)
  - CommandQueue is already prediction-ready
  - StateContext is pure data (easy to serialize)

## Easiest starting point

  Unity Gaming Services → Relay + NGO
  Free tier handles up to 200 concurrent players
  No dedicated server needed
  Works through NAT automatically
