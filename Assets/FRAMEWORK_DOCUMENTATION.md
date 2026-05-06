# Gameplay Architecture Framework — Documentation

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [ServiceLocator](#servicelocator)
3. [EventBus](#eventbus)
4. [CommandQueue](#commandqueue)
5. [StateMachine](#statemachine)
6. [StateContext](#statecontext)
7. [Health System](#health-system)
8. [AI Systems](#ai-systems)
9. [Perception System](#perception-system)
10. [Squad System](#squad-system)
11. [Formation System](#formation-system)
12. [Ability System](#ability-system)
13. [Projectile System](#projectile-system)
14. [Movement Strategy](#movement-strategy)
15. [Camera System](#camera-system)
16. [Object Pooling](#object-pooling)
17. [Save System](#save-system)
18. [Inventory System](#inventory-system)
19. [Scene Management](#scene-management)
20. [Editor Tooling](#editor-tooling)
21. [Unit Tests](#unit-tests)

---

## Architecture Overview

### Assembly Structure

```
Framework.asmdef        — pure C#, zero Gameplay imports
  Framework.Core        — ServiceLocator, pooling
  Framework.Events      — EventBus + all events
  Framework.Commands    — ICommand, CommandQueue
  Framework.StateMachine — IState, StateMachine, StateContext
  Framework.AI.*        — AI interfaces and enums
  Framework.Systems.Health — IHealth, IHealthComponent
  Framework.Abilities   — IAbilitySystem
  Framework.Items       — IItem, IInventory
  Framework.Movement    — IMovementStrategy
  Framework.Persistence — ISaveable

Gameplay.asmdef         — references Framework
  Gameplay.AI.*         — AIController, all AI systems
  Gameplay.States       — MoveState, AttackState, PatrolState
  Gameplay.Systems.*    — Health types, Movement, Projectiles
  Gameplay.Abilities.*  — AbilitySystem, definitions
  Gameplay.Camera.*     — all camera modes
  Gameplay.Player       — PlayerStateController, PlayerLook
  Gameplay.Items        — ItemDefinition, InventoryComponent
  Gameplay.Bootstrap    — GameBootstrap, SquadFormationSetup
  Gameplay.UI           — FadePanel

FrameworkEditor.asmdef  — editor only
  Custom inspectors, gizmos

Tests.asmdef            — editor only
  All unit tests
```

### Key Principles

- **Framework never imports Gameplay** — enforced by compiler
- **All connections go through interfaces** — IHealth, IAbilitySystem, IMovementStrategy etc.
- **ServiceLocator replaces all singletons** — register in Awake, unregister in OnDestroy
- **EventBus replaces all direct callbacks** — zero coupling between systems
- **Everything opt-in** — null sub-contexts cost nothing

---

## ServiceLocator

**Location:** `Assets/Framework/Core/ServiceLocator.cs`

Central registry for all game services. Replaces static singletons.

```csharp
// Register (in Awake)
ServiceLocator.Register<MySystem>(this);

// Unregister (in OnDestroy)
ServiceLocator.Unregister<MySystem>();

// Get from anywhere
var system = ServiceLocator.Get<MySystem>();

// Check if registered
bool exists = ServiceLocator.Has<MySystem>();

// Clear all (called by SceneLifecycleManager on scene unload)
ServiceLocator.Clear();
```

**Systems registered by default:**
- `AIDirector`, `SquadSystem`, `AIGroupManager`, `AIAgentRegistry`
- `PoolRegistry`, `SaveSystem`, `PauseSystem`, `SceneLoader`

---

## EventBus

**Location:** `Assets/Framework/Events/EventBus.cs`

Global event system. Zero coupling between publisher and subscriber.

```csharp
// Subscribe (in OnEnable or Start)
EventBus.Subscribe<HealthChangedEvent>(OnHealthChanged);

// Unsubscribe (in OnDisable or OnDestroy — ALWAYS unsubscribe)
EventBus.Unsubscribe<HealthChangedEvent>(OnHealthChanged);

// Publish from anywhere
EventBus.Publish(new HealthChangedEvent(...));

// Clear all listeners (called on scene unload automatically)
EventBus.Clear();
```

**Built-in events:**

| Event | Fired when |
|---|---|
| `HealthChangedEvent` | Any agent's health changes |
| `GamePausedEvent` | Game pauses or resumes |
| `SceneLoadingEvent` | Scene transition starts |
| `GameSavedEvent` | Game saved or loaded |
| `InventoryChangedEvent` | Inventory gains or loses items |
| `ProjectileHitEvent` | Projectile hits something |

**Adding a new event:**
```csharp
// In Framework/Events/Events/Gameplay/
public struct MyEvent
{
    public int Value;
    public MyEvent(int v) { Value = v; }
}

// Publish
EventBus.Publish(new MyEvent(42));

// Subscribe
EventBus.Subscribe<MyEvent>(e => Debug.Log(e.Value));
```

---

## CommandQueue

**Location:** `Assets/Framework/Commands/`

Decouples intent from execution. States enqueue commands, the controller executes them.

```csharp
// Implement a command
public class DamageCommand : ICommand
{
    private readonly IHealth _health;
    private readonly int     _amount;

    public DamageCommand(IHealth health, int amount)
    {
        _health = health;
        _amount = amount;
    }

    public void Execute() => _health?.Damage(_amount);
}

// Enqueue from a state
context.Commands.Enqueue(new DamageCommand(health, 10));

// Execute all (in controller Update)
context.Commands.ExecuteAll();

// Clear (in StaggerState.Enter)
context.Commands.Clear();
```

---

## StateMachine

**Location:** `Assets/Framework/StateMachine/`

Drives agent behaviour. Each state is a pure C# class.

### Adding a new state

```csharp
public class MyState : IState
{
    // Required boilerplate
    private readonly List<Transition> _transitions = new();
    public void AddTransition(Transition t) => _transitions.Add(t);
    public List<Transition> GetTransitions() => _transitions;
    public void Exit() { }

    public void Enter(StateContext context)
    {
        // Runs once when state activates
        context.AnimationRequest =
            new AnimationRequest(AnimationType.Idle);
    }

    public void Update(StateContext context)
    {
        // Runs every frame while active
        // Read from context, enqueue commands
        // Never call Destroy, GetComponent, etc.
    }
}
```

### Adding a new condition

```csharp
public class MyCondition : ITransitionCondition
{
    public bool Evaluate(StateContext context)
    {
        // Return true to trigger the transition
        return context.SomeField > 0.5f;
    }
}
```

### Wiring transitions in a factory

```csharp
public class MyFactory : MonoBehaviour, IAIStateFactory
{
    public IState Build(StateContext context)
    {
        var idle   = new IdleState();
        var combat = new CombatState(new CombatStateConfig());

        idle.AddTransition(new Transition(
            new CanSeeTargetCondition(), combat));

        combat.AddTransition(new Transition(
            new TargetLostCondition(), idle));

        return idle; // starting state
    }
}
```

### Built-in conditions

| Condition | Triggers when |
|---|---|
| `CanSeeTargetCondition` | Agent spots a target |
| `TargetLostCondition` | Had target, lost sight |
| `IsInAttackRangeCondition` | Target within attack range |
| `WasHitCondition` | Agent took damage this frame |
| `StaggerFinishedCondition` | Stagger timer expired |
| `AttackPressedCondition` | Player pressed attack |
| `MovePressedCondition` | Player is moving |
| `MoveReleasedCondition` | Player stopped |

---

## StateContext

**Location:** `Assets/Framework/StateMachine/StateContext.cs`

Shared data container passed to every state every frame. All fields are opt-in — null sub-contexts cost nothing.

```
StateContext fields:
  Input          InputState           — player input (Move, Look, Attack, Pause)
  Commands       ICommandQueue        — queued commands for this frame
  HealthData     IHealth              — agent's health
  HealthComp     IHealthComponent     — agent's health component
  Self           Transform            — agent's transform
  Movement       IMovementStrategy    — how agent moves (Transform or NavMesh)
  Abilities      IAbilitySystem       — agent's ability system
  Team           Team                 — Enemy / Ally / Player / Neutral
  WasHit         bool                 — true if hit this frame (reset each frame)
  HitDirection   Vector3              — direction of last hit

  Sub-contexts (all nullable):
  PerceptionContext  IPerceptionContext   — visible targets, sensor state
  MemoryContext      IMemoryContext       — last known positions
  AlertContext       AlertContext         — alert level and value
  SquadContext       ISquadContext        — squad strategy and target

  Convenience accessors:
  Perception     IPerceptionState     — context.PerceptionContext.State
  Target         Transform            — context.PerceptionContext.Target
  VisibleTargets List<Transform>      — context.PerceptionContext.VisibleTargets
  Memory         IAIMemory            — context.MemoryContext.Memory
  AlertLevel     AlertLevel           — context.AlertContext.Level
  SquadStrategy  SquadStrategy        — current squad strategy

  AI data (written by AI systems):
  DirectorIntensity  float            — 0-1 global difficulty
  Morale             float            — 0-1 agent morale
  Fear               float            — 0-1 agent fear
  Suppression        float            — 0-1 suppression level
  Execution          AIExecutionContext — current LOD level
  SystemMask         ulong            — bitmask of active systems
```

---

## Health System

**Location:** `Assets/Gameplay/Systems/Health/`

### Health types

| Type | Description | Key fields |
|---|---|---|
| `Health` | Standard HP | Value, MaxValue |
| `ShieldedHealth` | Shield absorbs first | Shield, MaxShield |
| `RegenHealth` | HP regenerates over time | regenRate, regenDelay |
| `ArmouredHealth` | Flat + % damage reduction | Armour, ArmourPct |
| `SegmentedHealth` | HP in breakable segments | CurrentSegment, TotalSegments |
| `OvershieldHealth` | Bonus HP that decays | Overshield |
| `InvincibleHealth` | Takes no damage | — |
| `ElementalHealth` | Per-damage-type resistance | SetResistance() |
| `CompositeHealth` | Chains layers together | GetLayer<T>() |

### Composite presets (on HealthComponent)

```
ShieldedArmoured  = Shield → Armour → HP
ShieldedRegen     = Shield → RegenHP
SegmentedArmoured = Armour → SegmentedHP
FullBoss          = Shield → Armour → SegmentedHP
```

### Subscribing to health changes

```csharp
// Local — on a specific agent
healthComponent.GetHealth().OnChanged += OnHealthChanged;
healthComponent.GetHealth().OnDeath   += OnDeath;

// Global — any agent in the game
EventBus.Subscribe<HealthChangedEvent>(OnAnyHealthChanged);
```

### Adding a new health type

```csharp
// 1. Create class implementing IHealth
public class BurnHealth : IHealth
{
    public int  Value    { get; private set; }
    public int  MaxValue { get; private set; }
    public bool IsDead   => Value <= 0;
    public event Action<int> OnChanged;
    public event Action      OnDeath;

    private float _burnTimer;
    private int   _burnDamagePerSecond;

    public BurnHealth(int max, int burnDps)
    {
        MaxValue             = max;
        Value                = max;
        _burnDamagePerSecond = burnDps;
    }

    public void Damage(int amount) { ... }
    public void Heal(int amount)   { ... }
    public void Reset()            { ... }

    public void Tick(float dt)
    {
        _burnTimer += dt;
        if (_burnTimer >= 1f)
        {
            Damage(_burnDamagePerSecond);
            _burnTimer = 0f;
        }
    }
}

// 2. Add to HealthComponent.HealthType enum
// 3. Add case in HealthComponent.BuildHealth()
// 4. Handle Tick() in HealthComponent.Update()
```

---

## AI Systems

**Location:** `Assets/Gameplay/AI/Systems/`

Per-agent systems that run every frame via `AISystemManager`. Each implements `IAISystem`.

### Built-in systems (registered in AISystemsBootstrap)

| System | Category | What it does |
|---|---|---|
| `AILODSystem` | Utility | Sets context.Execution.LOD based on distance |
| `DirectorSystem` | Utility | Pushes director intensity to context |
| `SuppressionSystem` | Emotion | Calculates suppression from nearby fire |
| `MoralSystem` | Emotion | Updates morale based on squad state |
| `AlertSystem` | Emotion | Manages alert level transitions |
| `ThreatSystem` | Combat | Picks best target from visible list |
| `SquadAISystem` | Squad | Pushes squad strategy to agent context |
| `FormationAISystem` | Squad | Moves agents toward formation slots |

### Adding a custom AI system

```csharp
public class MyAISystem : IAISystem
{
    public AISystemCategory Category => AISystemCategory.Utility;

    // Optional — return false to skip this frame
    public bool ShouldRun(StateContext context)
    {
        return context.AlertLevel == AlertLevel.Combat;
    }

    public void Update(StateContext context)
    {
        // Read from context, write to context
        // Never call GetComponent or FindObject
        context.Fear += Time.deltaTime * 0.1f;
    }
}

// Register in AISystemsBootstrap.RegisterDefaults()
manager.Register(new MyAISystem());
```

### LOD levels

| Level | When | Systems that skip |
|---|---|---|
| `Low` | Far away, calm | Most systems |
| `Medium` | Mid range | Some systems |
| `High` | Close or alert | Few systems |
| `Critical` | In combat, was hit | None |

---

## Perception System

**Location:** `Assets/Gameplay/AI/Perception/`

Detects targets using configurable sensors.

### Sensor types (set in Inspector)

| Type | Description |
|---|---|
| `OverlapSphere` | Detects everything in radius |
| `Cone` | FOV cone in front of agent |
| `Raycast` | Line of sight through walls |
| `Trigger` | Unity trigger collider |

### Swapping sensor at runtime

```csharp
var perception = GetComponent<PerceptionSystem>();

// Switch to cone sensor
perception.SetSensor(new ConeSensor(
    transform, radius: 12f, fov: 90f, layers));

// Switch to raycast sensor
perception.SetSensor(new RaycastSensor(
    transform, radius: 15f, targetLayer,
    occlusionLayer, fov: 60f));
```

### Adding a custom sensor

```csharp
public class ProximitySensor : IPerceptionSensor
{
    private readonly float     _radius;
    private readonly LayerMask _layer;

    public ProximitySensor(float radius, LayerMask layer)
    {
        _radius = radius;
        _layer  = layer;
    }

    public List<Transform> Sense(Vector3 origin)
    {
        var results = new List<Transform>();
        var hits    = Physics.OverlapSphere(origin, _radius, _layer);

        foreach (var hit in hits)
            results.Add(hit.transform);

        return results;
    }
}
```

---

## Squad System

**Location:** `Assets/Gameplay/AI/Squad/`

Three separate squads — agents sorted by team automatically.

```
SquadSystem.EnemySquad   — Team.Enemy agents
SquadSystem.AllySquad    — Team.Ally agents
SquadSystem.PlayerSquad  — Team.Player agents
```

### Registration is automatic

`AIController.Start()` registers the agent.
`AIController.OnDisable()` unregisters the agent.
`PlayerStateController.Start()` registers the player.

### Reading squad data from anywhere

```csharp
var squadSystem = ServiceLocator.Get<SquadSystem>();

// Get a specific squad
var enemySquad = squadSystem.GetSquad(Team.Enemy);

// Check strategy
Debug.Log(enemySquad.CurrentStrategy);

// Get target position
Vector3 target = squadSystem.GetTargetPosition(Team.Enemy);
```

### Squad strategies

| Strategy | When | Formation |
|---|---|---|
| `Idle` | No target, calm | Tight |
| `Search` | Lost target, looking | Tight |
| `Chase` | Target spotted, moving | Tight |
| `Engage` | In combat range | Loose |
| `Retreat` | Morale < 0.3 or fear > 0.7 | Off |

---

## Formation System

**Location:** `Assets/Gameplay/AI/Formation/`

Assigns each agent a slot position relative to the squad leader.

### Enable formation

Add `SquadFormationSetup` to `_GameSystems`:

```csharp
// Or from code
ServiceLocator.Get<SquadSystem>().EnemySquad.Formation =
    new FormationData(FormationType.Wedge) { Spacing = 2.5f };
```

### Formation types

| Type | Shape |
|---|---|
| `None` | No formation |
| `Line` | Side by side |
| `Wedge` | V shape behind leader |
| `Circle` | Ring around leader |

### Adding a new formation type

```csharp
// 1. Add to FormationType enum
public enum FormationType { None, Line, Wedge, Circle, Column }

// 2. Add case in FormationSystem.GetOffset()
case FormationType.Column:
    return -formation.Leader.forward
           * formation.Spacing * index;
```

---

## Ability System

**Location:** `Assets/Gameplay/Abilities/`

### Create an ability asset

Right click → Create → Gameplay → Ability Definition

| Field | Description |
|---|---|
| Ability Id | Unique string ("Attack", "Shoot") |
| Ability Type | MeleeAttack or RangedAttack |
| Damage | Damage per use |
| Cooldown | Seconds between uses |
| Priority | Higher = preferred |
| Pool Key | Ranged only — must match pool entry |

### Assign to agent

On `CombatAIStateFactory` or `PatrolAIStateFactory` drag ability assets into the Abilities array. Multiple abilities — agent uses highest priority available.

### Adding a new ability type

```csharp
// 1. Add to AbilityType enum in AbilityDefinition.cs
public enum AbilityType { MeleeAttack, RangedAttack, Heal, AOESlam }

// 2. Add fields
[Header("Heal Only")]
public int healAmount = 20;

// 3. Add case in Build()
case AbilityType.Heal:
    return new Ability
    {
        Id       = abilityId,
        Cooldown = cooldown,
        Priority = priority,
        Execute  = (ctx) =>
            ctx.SourceHealth?.Heal(healAmount, ctx.Self.position)
    };
```

---

## Projectile System

**Location:** `Assets/Gameplay/Systems/Projectiles/`

### Setup

```
1. Create bullet prefab:
   - SphereCollider — Is Trigger ON
   - Rigidbody      — Is Kinematic ON
   - Projectile component — set Pool Key + Hit Layers

2. Register pool in GameBootstrap:
   Key: Bullet, Prefab: [bullet prefab], Size: 20

3. Create RangedAttack ability asset:
   Pool Key: Bullet

4. Drag ability onto agent factory Abilities array
```

### React to hits

```csharp
EventBus.Subscribe<ProjectileHitEvent>(OnProjectileHit);

private void OnProjectileHit(ProjectileHitEvent e)
{
    // e.Source   — the projectile
    // e.Target   — what was hit
    // e.HitPoint — world position
    // e.Damage   — damage dealt

    VFXSystem.SpawnHitEffect(e.HitPoint);
    AudioSystem.PlayHitSound(e.HitPoint);
}
```

---

## Movement Strategy

**Location:** `Assets/Framework/Movement/` and `Assets/Gameplay/Systems/Movement/`

Swappable movement — zero state changes required.

```csharp
// Transform movement (default — agents walk through walls)
context.Movement = new TransformMovementStrategy();

// NavMesh movement (proper pathfinding)
var agent = GetComponent<NavMeshAgent>();
context.Movement = new NavMeshMovementStrategy(agent);
```

`AIController` auto-detects `NavMeshAgent` on the same GameObject and sets the strategy automatically.

### To enable NavMesh

```
1. Add NavMeshAgent component to agent prefab
2. Window → AI → Navigation → Bake
3. Done — AIController switches automatically
```

### Adding a custom strategy

```csharp
public class RigidbodyMovementStrategy : IMovementStrategy
{
    private readonly Rigidbody _rb;
    public RigidbodyMovementStrategy(Rigidbody rb) => _rb = rb;

    public void MoveTo(Transform self, Vector3 dest, float speed)
    {
        var dir = (dest - self.position).normalized;
        _rb.MovePosition(self.position + dir * speed * Time.deltaTime);
    }

    public void MoveInDirection(Transform self, Vector3 dir, float speed)
        => _rb.MovePosition(self.position + dir * speed * Time.deltaTime);

    public void Stop(Transform self) => _rb.linearVelocity = Vector3.zero;

    public bool HasArrived(Transform self, Vector3 dest, float threshold = 0.5f)
        => Vector3.Distance(self.position, dest) <= threshold;
}
```

---

## Camera System

**Location:** `Assets/Gameplay/Camera/`

All modes write into `CameraSnapshot` — never touch `cam.transform` directly.

### Camera modes

| Mode | Description |
|---|---|
| `FPSCameraMode` | First person, follows head rotation |
| `ThirdPersonCameraMode` | Third person follow |
| `FreeLookCameraMode` | Orbits around target |
| `TopDownCameraMode` | Top-down fixed angle |
| `IsometricCameraMode` | Isometric fixed angle |
| `OverShoulderCameraMode` | Over-shoulder aim |
| `OrbitCameraMode` | Orbits around a point |
| `FixedCameraMode` | Static, no movement |
| `CinematicCameraMode` | Cinematic with path |

### Requesting a mode switch

```csharp
var controller = GetComponent<CameraModeController>();
controller.Request(new CameraRequest
{
    Mode      = new FPSCameraMode(headTransform),
    Priority  = 10,
    BlendTime = 0.5f
});
```

### Adding a new camera mode

```csharp
public class MyCamera : ICameraMode
{
    public void Activate(Camera cam) { }
    public void Deactivate(Camera cam) { }

    public void Tick(Camera cam, float dt, ref CameraSnapshot s)
    {
        // Write position and rotation into snapshot
        // Never write to cam.transform directly
        s.Position = somePosition;
        s.Rotation = someRotation;
    }
}
```

---

## Object Pooling

**Location:** `Assets/Framework/Core/`

### Setup

Add `GameBootstrap` to `_GameSystems`:

```
Object Pools
  Element 0 — Key: Enemy,  Prefab: [enemy],  Size: 10
  Element 1 — Key: Bullet, Prefab: [bullet], Size: 20
  Element 2 — Key: Ally,   Prefab: [ally],   Size: 5
```

### Usage from code

```csharp
var registry = ServiceLocator.Get<PoolRegistry>();

// Spawn
var obj = registry.Get("Enemy", position, rotation);

// Return immediately
registry.Return("Enemy", obj);

// Return after delay
registry.ReturnDelayed("Enemy", obj, delay: 2f);

// Check pool exists
bool exists = registry.HasPool("Enemy");
```

### Make agents return to pool on death

Add `DeathSystem` component to agent prefab. Set **Pool Key** to match pool entry key.

---

## Save System

**Location:** `Assets/Framework/Persistence/`

### Setup

Add `SaveSystem` to `_GameSystems`.

```csharp
// Save
ServiceLocator.Get<SaveSystem>().Save();

// Load
ServiceLocator.Get<SaveSystem>().Load();

// Check if save exists
bool has = ServiceLocator.Get<SaveSystem>().HasSave;

// Delete save
ServiceLocator.Get<SaveSystem>().DeleteSave();
```

### Make any system saveable

```csharp
public class MySystemSaveable : MonoBehaviour, ISaveable
{
    public string SaveId => "MySystem"; // must be unique

    private void Start()
        => ServiceLocator.Get<SaveSystem>()?.Register(this);

    private void OnDestroy()
        => ServiceLocator.Get<SaveSystem>()?.Unregister(this);

    public object CaptureState()
        => new SaveData { score = _score, level = _level };

    public void RestoreState(object state)
    {
        var data = state as SaveData;
        if (data == null) return;
        _score = data.score;
        _level = data.level;
    }

    [Serializable]
    public class SaveData { public int score; public int level; }
}
```

**Rules for save data:**
- Must be `[Serializable]`
- Only plain types: int, float, string, bool, Vector3, arrays
- No GameObject or Component references

---

## Inventory System

**Location:** `Assets/Framework/Items/` and `Assets/Gameplay/Items/`

### Setup

Add `InventoryComponent` to any GameObject. Set capacity and owner ID.

### Create item assets

Right click → Create → Gameplay → Item Definition

```csharp
// Get inventory
var inv = GetComponent<InventoryComponent>();

// Add item (drag ItemDefinition into inspector field)
public ItemDefinition healthPotion;
inv.Add(healthPotion, 3);

// Remove
inv.Remove("health_potion", 1);

// Check
bool has = inv.Has("quest_key");
int  qty = inv.GetCount("gold_coin");

// Iterate slots
foreach (var slot in inv.Inventory.Slots)
    Debug.Log($"{slot.Item.DisplayName} x{slot.Count}");
```

### React to changes

```csharp
EventBus.Subscribe<InventoryChangedEvent>(OnInventoryChanged);

private void OnInventoryChanged(InventoryChangedEvent e)
{
    // e.OwnerId — which inventory
    // e.ItemId  — which item
    // e.Delta   — +added or -removed
}
```

---

## Scene Management

**Location:** `Assets/Gameplay/Systems/`

### SceneLoader

Always use `SceneLoader` — never call `SceneManager.LoadScene` directly.

```csharp
var loader = ServiceLocator.Get<SceneLoader>();

loader.Load("GameScene");     // by name
loader.Load(2);               // by build index
loader.Reload();              // reload current
loader.LoadNext();            // next in build order
```

### FadePanel

Add to a persistent Canvas (Sort Order 999). Subscribes to `SceneLoadingEvent` automatically.

### PauseSystem

```csharp
var pause = ServiceLocator.Get<PauseSystem>();

pause.Toggle();
pause.SetPaused(true);
pause.SetPaused(false);
bool paused = pause.IsPaused;
```

---

## Editor Tooling

**Location:** `Assets/Editor/`

### Custom inspectors

| Inspector | Shows |
|---|---|
| `AIControllerEditor` | LOD, fear, morale, suppression, alert, target |
| `HealthComponentEditor` | Color-coded health bar, type info |
| `PerceptionSystemEditor` | Vision radius, attack range, FOV cone gizmos |
| `SquadSystemEditor` | Formation slot gizmos, squad info |

All data visible during Play Mode only.

---

## Unit Tests

**Location:** `Assets/Tests/`

Run via: Window → General → Test Runner → EditMode → Run All

| Suite | Tests | Covers |
|---|---|---|
| `HealthTests` | 17 | All health types, composite routing |
| `EventBusTests` | 6 | Subscribe, publish, clear |
| `ServiceLocatorTests` | 8 | Register, get, clear |
| `InventoryTests` | 10 | Add, remove, stack, capacity |
| `CommandQueueTests` | 4 | Enqueue, execute, clear |
| `PerceptionSensorTests` | 3 | MockSensor contract |

---

*Framework version: Unity 6, C# 8, two assembly definitions (Framework + Gameplay)*
