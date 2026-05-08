# Gameplay Architecture Framework — Complete Documentation

**Unity 6 · C# · Two Assembly Definitions**

---

## Table of Contents

1. [Architecture Overview](#1-architecture-overview)
2. [The Golden Rules](#2-the-golden-rules)
3. [How To Add Any New System](#3-how-to-add-any-new-system)
4. [Framework Layer — Core](#4-framework-layer--core)
5. [Framework Layer — State Machine](#5-framework-layer--state-machine)
6. [Framework Layer — Interfaces](#6-framework-layer--interfaces)
7. [Framework Layer — Events](#7-framework-layer--events)
8. [Gameplay Layer — AI](#8-gameplay-layer--ai)
9. [Gameplay Layer — Health System](#9-gameplay-layer--health-system)
10. [Gameplay Layer — Camera System](#10-gameplay-layer--camera-system)
11. [Gameplay Layer — Audio System](#11-gameplay-layer--audio-system)
12. [Gameplay Layer — 2D Support](#12-gameplay-layer--2d-support)
13. [Gameplay Layer — Abilities & Combat](#13-gameplay-layer--abilities--combat)
14. [Gameplay Layer — Progression](#14-gameplay-layer--progression)
15. [Gameplay Layer — Inventory & Items](#15-gameplay-layer--inventory--items)
16. [Gameplay Layer — Dialogue & Quests](#16-gameplay-layer--dialogue--quests)
17. [Gameplay Layer — Interaction](#17-gameplay-layer--interaction)
18. [Gameplay Layer — Status Effects](#18-gameplay-layer--status-effects)
19. [Gameplay Layer — Stage & Respawn](#19-gameplay-layer--stage--respawn)
20. [Gameplay Layer — Network Foundation](#20-gameplay-layer--network-foundation)
21. [Behaviour Tree](#21-behaviour-tree)
22. [Editor Tools & Debug](#22-editor-tools--debug)
23. [Unit Tests](#23-unit-tests)
24. [Scene Setup Checklist](#24-scene-setup-checklist)
25. [Export & Package Guide](#25-export--package-guide)
26. [Extending The Framework](#26-extending-the-framework)

---

## 1. Architecture Overview

The framework is split into exactly two runtime assemblies enforced by the compiler.

```
Framework.asmdef
  Pure C#. Zero Unity gameplay imports.
  Zero Gameplay imports — enforced by compiler.
  Contains: interfaces, events, data, state machine core.

Gameplay.asmdef
  References Framework.
  Contains: all MonoBehaviours, implementations, Unity-specific code.

FrameworkEditor.asmdef
  Editor only. References both.
  Contains: custom inspectors, gizmos.

Tests.asmdef
  Editor only. References both.
  Contains: all unit tests.
```

### Why this matters

If you ever write `using Gameplay.*` inside a Framework file, it will not compile. This is intentional. It means:

- Framework can be exported to any project with zero changes
- Gameplay systems cannot accidentally depend on each other through Framework
- AI, cameras, health, audio all talk through interfaces — never concrete types

### The two folders

```
Assets/
  Framework/          ← interfaces, events, data contracts
    AI/               ← AI interfaces and enums
    Audio/            ← IAudioSystem
    Commands/         ← ICommand, ICommandQueue, CommandQueue
    Core/             ← ServiceLocator, GameObjectPool, PoolRegistry
    Dialogue/         ← IDialogueLine, IDialogueTree, IDialogueSystem
    Events/           ← EventBus + all event structs
    Input/            ← InputState
    Interaction/      ← IInteractable
    Items/            ← IItem, IInventory, IItemDatabase
    Movement/         ← IMovementStrategy
    Network/          ← INetworkEntity, INetworkTransport
    Persistance/      ← ISaveable, SaveSystem
    Progression/      ← ILevelSystem, IClassSystem, ICharacterClass
    Quests/           ← IQuest, IQuestObjective, IQuestSystem
    StateMachine/     ← IState, StateContext, StateMachine, conditions
    StatusEffects/    ← IStatusEffect, IStatusEffectSystem
    Systems/Damage/   ← DamageType, DamageInfo, IDamageSource
    Systems/Health/   ← IHealth, IHealthComponent

  Gameplay/           ← all implementations
    AI/               ← AIController, all AI systems and states
    Abilities/        ← AbilitySystem, ability definitions
    Audio/            ← UnityAudioSystem, WwiseAudioSystem, MusicSystem
    Bootstrap/        ← GameBootstrap, SquadFormationSetup
    Camera/           ← all camera modes (3D and 2D)
    Combat/           ← StaggerSystem, KnockbackSystem, ComboSystem
    Dialogue/         ← DialogueSystem, DialogueTree, DialogueAsset
    Input/            ← InputHandler
    Interaction/      ← InteractionSystem, InteractableDoor, InteractableItem
    Items/            ← InventoryComponent, ItemDefinition, ItemDatabase, LootSystem
    Network/          ← OfflineTransport, LocalNetworkEntity
    Player/           ← PlayerStateController, PlayerLook
    Progression/      ← LevelSystem, StatSystem, ClassSystem, XPSource
    Quests/           ← QuestSystem, Quest, QuestAsset
    States/           ← all player/shared states
    StatusEffects/    ← all effect implementations
    Systems/          ← Health types, Movement strategies, Projectile, Stage, Respawn
    Tools/            ← AIDebugOverlay
    UI/               ← FadePanel
```

---

## 2. The Golden Rules

These rules apply to every system in the framework. If you follow them, everything is independent, removable, and modular.

**Rule 1 — Framework never imports Gameplay**
Framework files only import `UnityEngine`, `System`, and other Framework namespaces.

**Rule 2 — Use interfaces, not concrete types**
StateContext holds `IHealth` not `Health`. `IMovementStrategy` not `NavMeshMovementStrategy`. This means you can swap implementations without changing anything else.

**Rule 3 — Register with ServiceLocator in Awake, unregister in OnDestroy**
```csharp
private void Awake()   => ServiceLocator.Register<MySystem>(this);
private void OnDestroy => ServiceLocator.Unregister<MySystem>();
```

**Rule 4 — Communicate through EventBus, not direct references**
Instead of `healthUI.UpdateDisplay(hp)`, publish `HealthChangedEvent`. The UI subscribes. Neither knows about the other.

**Rule 5 — Always unsubscribe in OnDisable or OnDestroy**
```csharp
private void OnEnable()  => EventBus.Subscribe<MyEvent>(OnMyEvent);
private void OnDisable() => EventBus.Unsubscribe<MyEvent>(OnMyEvent);
```

**Rule 6 — States are pure C#, never MonoBehaviours**
States only read from and write to StateContext. They never call `GetComponent`, `FindObject`, or `Destroy`.

**Rule 7 — One responsibility per file**
`HealthComponent` manages health. `DeathSystem` handles death behaviour. `StaggerSystem` handles stagger. Never merge.

---

## 3. How To Add Any New System

Every new system follows the same four steps regardless of what it does.

### Step 1 — Define the interface in Framework

```csharp
// Assets/Framework/MyFeature/IMySystem.cs
namespace Framework.MyFeature
{
    public interface IMySystem
    {
        void DoSomething(int value);
        bool IsReady { get; }
    }
}
```

### Step 2 — Define events in Framework

```csharp
// Assets/Framework/Events/Events/Gameplay/MySystemEvents.cs
namespace Framework.Events.Events.Gameplay
{
    public struct MySomethingHappenedEvent
    {
        public int Value;
        public MySomethingHappenedEvent(int v) { Value = v; }
    }
}
```

### Step 3 — Implement in Gameplay

```csharp
// Assets/Gameplay/MyFeature/MySystem.cs
using UnityEngine;
using Framework.Core;
using Framework.Events;
using Framework.Events.Events.Gameplay;
using Framework.MyFeature;

namespace Gameplay.MyFeature
{
    public class MySystem : MonoBehaviour, IMySystem
    {
        public bool IsReady { get; private set; }

        private void Awake()
        {
            ServiceLocator.Register<IMySystem>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IMySystem>();
        }

        public void DoSomething(int value)
        {
            IsReady = true;
            EventBus.Publish(new MySomethingHappenedEvent(value));
        }
    }
}
```

### Step 4 — Use from anywhere

```csharp
// From any MonoBehaviour or state
var system = ServiceLocator.Get<IMySystem>();
system?.DoSomething(42);

// React to it from anywhere else
EventBus.Subscribe<MySomethingHappenedEvent>(OnSomethingHappened);
```

That's the complete pattern. Every system in this framework was built this way.

---

## 4. Framework Layer — Core

### ServiceLocator

**File:** `Framework/Core/ServiceLocator.cs`

Central registry for all services. Replaces all static singletons.

```csharp
// Register (always in Awake)
ServiceLocator.Register<IMySystem>(this);

// Unregister (always in OnDestroy)
ServiceLocator.Unregister<IMySystem>();

// Get from anywhere — null-safe
var system = ServiceLocator.Get<IMySystem>();
system?.DoSomething();

// Check existence
bool exists = ServiceLocator.Has<IMySystem>();

// Nuclear option — clears everything
// Called automatically by SceneLifecycleManager on scene unload
ServiceLocator.Clear();
```

**Important:** `ServiceLocator.Get<T>()` returns null if the service is not registered. Always use `?.` null-conditional operator when calling methods on the result.

### GameObjectPool and PoolRegistry

**Files:** `Framework/Core/GameObjectPool.cs`, `Framework/Core/PoolRegistry.cs`

Reuses GameObjects instead of Instantiate/Destroy. Critical for performance with bullets, enemies, effects.

**Setup in GameBootstrap Inspector:**
```
Object Pools
  Key: Enemy   Prefab: [enemy prefab]   Size: 10
  Key: Bullet  Prefab: [bullet prefab]  Size: 20
```

**Usage:**
```csharp
var registry = ServiceLocator.Get<PoolRegistry>();

// Get an object from pool
GameObject obj = registry.Get("Enemy", position, rotation);

// Return to pool
registry.Return("Enemy", obj);

// Check pool exists before using
if (registry.HasPool("Bullet"))
    registry.Get("Bullet", spawnPos, Quaternion.identity);
```

**Key design note:** `GameObjectPool` deactivates the prefab before `Instantiate`. This prevents `Awake`/`OnEnable` from firing during pre-warm. If you forget this, pooled agents register with SquadSystem before their context exists.

**Making an agent return to pool on death:**
Add `DeathSystem` component to the prefab and set its `poolKey` field to match the pool entry key.

### SceneLifecycleManager

**File:** `Framework/Core/SceneLifecycleManager.cs`

Clears the EventBus when a scene unloads, preventing stale subscribers from causing errors.

Add to `_GameSystems`. No configuration needed — it subscribes to `SceneManager.sceneUnloaded` automatically.

### FrameworkProfiler

**File:** `Framework/Core/FrameworkProfiler.cs`

Unity Profiler markers for all hot paths. Zero cost in release builds.

```csharp
// Used internally — you don't call these directly
// View in: Window → Analysis → Profiler → CPU Usage

Framework.AISystemManager.Update     — all AI systems per frame
Framework.StateMachine.Update        — state transitions + update
Framework.Perception.Tick            — sensor queries
Framework.CommandQueue.ExecuteAll    — command execution
Framework.Pool.Get                   — pool retrieval
Framework.Pool.Return                — pool return
Framework.CameraMode.Tick            — camera calculations
```

---

## 5. Framework Layer — State Machine

### How the state machine works

Every agent (player, enemy, NPC) runs a `StateMachine` with a `StateContext`. Each frame:
1. `StateMachine.Update()` checks all transitions on the current state
2. If a condition is true, switches to the target state
3. Calls `Update()` on the current state

### StateContext

**File:** `Framework/StateMachine/StateContext.cs`

The single data container passed to every state every frame. States read from it and write to it. They never call `GetComponent` or touch the scene directly.

```csharp
// Core
context.Input          // InputState — move, look, attack, jump, dash
context.Commands       // ICommandQueue — enqueue movement/damage commands
context.HealthData     // IHealth — current HP values
context.HealthComp     // IHealthComponent — to call Damage/Heal
context.Self           // Transform — agent's transform
context.Movement       // IMovementStrategy — how to move
context.Abilities      // IAbilitySystem — ability execution

// AI sub-contexts (all nullable — check before use)
context.PerceptionContext  // visible targets, sensor state
context.MemoryContext      // last known positions
context.AlertContext       // alert level and value
context.SquadContext       // squad strategy and role

// Shortcuts (read from sub-contexts)
context.Target         // current target transform
context.VisibleTargets // list of visible targets
context.AlertLevel     // current alert level enum
context.SquadStrategy  // current squad strategy enum

// Combat
context.WasHit         // true for one frame after being hit
context.HitDirection   // direction of last hit
context.Abilities      // ability system

// AI emotion (written by AI systems)
context.Morale         // 0-1, falls when allies die
context.Fear           // 0-1, rises when taking damage
context.Suppression    // 0-1, rises from nearby fire
context.DirectorIntensity // 0-1 global difficulty

// 2D physics (written by movement strategy each frame)
context.IsGrounded     // true when on ground
context.IsOnWall       // true when touching wall / at ledge
context.IsCrouching    // true when crouching

// Status effects
context.StatusEffects  // IStatusEffectSystem
context.SpeedMultiplier // 1.0 = normal, 0.5 = half speed
context.IsStunned      // true when stunned

// Debug
context.CurrentStateName  // name of active state (written by StateMachine)
```

### Writing a new state

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
        // Called once when state activates
        context.AnimationRequest =
            new AnimationRequest(AnimationType.Idle);
    }

    public void Update(StateContext context)
    {
        // Called every frame
        // Read context, enqueue commands, never touch Unity directly
        if (context.Input.Move.sqrMagnitude > 0.01f)
            context.Commands.Enqueue(
                new MoveCommand(context.Self,
                    context.Input.Move.normalized,
                    5f,
                    context.Movement));
    }
}
```

### Writing a new condition

```csharp
public class MyCondition : ITransitionCondition
{
    public bool Evaluate(StateContext context)
    {
        // Return true to fire the transition
        return context.HealthData?.Value < 30;
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
        var attack = new AttackState();
        var dash   = new DashState();

        // idle → attack when attack pressed
        idle.AddTransition(new Transition(
            new AttackPressedCondition(), attack));

        // idle → dash when dash pressed and not on cooldown
        idle.AddTransition(new Transition(
            new DashPressedCondition(() => dash.OnCooldown), dash));

        // dash → idle when dash finished
        dash.AddTransition(new Transition(
            new DashFinishedCondition(
                () => dash.IsFinished,
                ctx => ctx.HealthComp?.SetInvincible(false) is var _ ? false : false),
            idle));

        return idle; // starting state
    }
}
```

### All built-in conditions

| Condition | When it fires |
|---|---|
| `CanSeeTargetCondition` | Agent has visible targets |
| `TargetLostCondition` | Had target, now no visible targets |
| `IsInAttackRangeCondition` | Target within attack range |
| `WasHitCondition` | Agent took damage this frame |
| `StaggerFinishedCondition` | Stagger timer expired |
| `AttackPressedCondition` | Player pressed attack |
| `AttackFinishedCondition` | Attack animation/timer done |
| `MovePressedCondition` | Player is moving (input magnitude > 0) |
| `MoveReleasedCondition` | Player stopped moving |
| `IsGroundedCondition` | Agent is on ground |
| `IsAirborneCondition` | Agent is not on ground |
| `JumpPressedCondition` | Player pressed jump |
| `DashPressedCondition` | Player pressed dash and not on cooldown |
| `DashFinishedCondition` | Dash duration expired |
| `DoubleJumpCondition` | Jump pressed while airborne, extra jumps available |
| `LandedCondition` | Just landed (IsGrounded became true) |
| `LandFinishedCondition` | Land animation timer expired |

### All built-in states

**Framework states (pure C#, no Gameplay imports):**
- `IdleState` — does nothing, waits for transitions
- `StaggerState` — freezes movement, plays stagger animation, timed

**Gameplay states (in Gameplay.States):**
- `MoveState` — camera-relative movement using input
- `AttackState` — triggers ability or deals direct damage to target
- `PatrolState` — waypoint patrol or random wander, respects ledges
- `SearchState` — looks for target at last known position
- `StrafeState` — circle-strafes around target
- `JumpState` — triggers jump via PlatformerMovementStrategy
- `FallState` — airborne falling, maintains horizontal control
- `LandState` — brief landing state, resets to idle
- `WallSlideState` — slow fall while pressing against wall
- `DashState` — fast burst, invincibility frames, cooldown
- `DoubleJumpState` — second jump, configurable extra jump count

**Gameplay AI states (in Gameplay.AI.States):**
- `ChaseState` — NavMesh or transform pathfinding to target
- `CombatState` — attacks target, strafes, uses abilities
- `TurretCombatState` — rotates to face target, fires only
- `PlatformerChaseState` — horizontal chase with gap jumping

---

## 6. Framework Layer — Interfaces

These are the contracts every system talks through.

### IMovementStrategy

```csharp
// Auto-detected by AIController based on components:
// NavMeshAgent     → NavMeshMovementStrategy
// Rigidbody2D (gravity=0) → TopDown2DMovementStrategy
// Rigidbody2D (gravity>0) → PlatformerMovementStrategy
// none             → TransformMovementStrategy

// Override manually:
_context.Movement = new NavMeshMovementStrategy(navAgent);

// Write a custom one:
public class MyMovement : IMovementStrategy
{
    public void MoveTo(Transform self, Vector3 dest, float speed) { }
    public void MoveInDirection(Transform self, Vector3 dir, float speed) { }
    public void Stop(Transform self) { }
    public bool HasArrived(Transform self, Vector3 dest, float threshold) => false;
    public void Tick(StateContext context) { } // writes IsGrounded etc.
}
```

### IHealth and IHealthComponent

```csharp
// Reading health values
IHealth health = context.HealthData;
health.Value;       // current HP
health.MaxValue;    // maximum HP
health.IsDead;      // true when Value <= 0

// Subscribing to changes
health.OnChanged += newValue => UpdateUI(newValue);
health.OnDeath   += HandleDeath;

// Dealing damage (go through IHealthComponent not IHealth directly)
IHealthComponent comp = context.HealthComp;
comp.Damage(25, hitPoint);  // amount + world position of hit
comp.Heal(10);
comp.Reset();               // restore to full (for respawn/pool)
comp.SetInvincible(true);   // for dash frames, cutscenes
```

### IAbilitySystem

```csharp
// Register an ability
context.Abilities.Register(myAbility);

// Use by id
context.Abilities.Use("Attack", abilityContext);

// Use best available (highest priority that is ready)
var system = context.Abilities as AbilitySystem;
system?.UseBestAvailable(abilityContext);
```

### IStatusEffectSystem

```csharp
// Apply an effect
context.StatusEffects?.Apply(new BurnEffect(5, 3f), context);
context.StatusEffects?.Apply(new SlowEffect(0.5f, 2f), context);

// Check if affected
bool burning = context.StatusEffects?.Has("burn") ?? false;

// Remove specific effect
context.StatusEffects?.Remove("freeze", context);

// Remove all
context.StatusEffects?.Clear(context);
```

---

## 7. Framework Layer — Events

### EventBus

**File:** `Framework/Events/EventBus.cs`

```csharp
// Subscribe (OnEnable or Start)
EventBus.Subscribe<HealthChangedEvent>(OnHealthChanged);

// Unsubscribe (ALWAYS in OnDisable or OnDestroy)
EventBus.Unsubscribe<HealthChangedEvent>(OnHealthChanged);

// Publish from anywhere
EventBus.Publish(new HealthChangedEvent(gameObject, 70, -30, 100, hitPoint));

// Clear all listeners (called automatically on scene unload)
EventBus.Clear();
```

### All built-in events

**Health:**
- `HealthChangedEvent` — `Source`, `NewValue`, `Delta`, `MaxValue`, `HitPoint`, `IsDead`

**Game state:**
- `GamePausedEvent` — `IsPaused`
- `GameSavedEvent` — `IsSaving`
- `SceneLoadingEvent` — `SceneName`, `IsLoading`

**Inventory:**
- `InventoryChangedEvent` — `OwnerId`, `ItemId`, `Delta`

**Projectiles:**
- `ProjectileHitEvent` — `Source`, `Target`, `HitPoint`, `HitNormal`, `Damage`

**Dialogue:**
- `DialogueStartedEvent` — `TreeId`, `Title`
- `DialogueLineEvent` — `SpeakerName`, `Text`, `PortraitKey`, `AudioEventId`
- `DialogueChoicesEvent` — `Choices[]`
- `DialogueEndedEvent` — `TreeId`

**Quests:**
- `QuestStartedEvent` — `QuestId`, `Title`, `Description`
- `QuestObjectiveUpdatedEvent` — `QuestId`, `ObjectiveId`, `Current`, `Required`, `IsComplete`
- `QuestCompletedEvent` — `QuestId`, `Title`
- `QuestFailedEvent` — `QuestId`, `Title`

**Progression:**
- `ExperienceGainedEvent` — `Amount`, `Total`, `NextLevel`
- `LevelUpEvent` — `OldLevel`, `NewLevel`
- `ClassSelectedEvent` — `ClassId`, `DisplayName`, `Tier`
- `ClassEvolvedEvent` — `OldClassId`, `NewClassId`, `NewTier`

**Status effects:**
- `StatusEffectAppliedEvent` — `EffectId`, `DisplayName`, `Duration`
- `StatusEffectExpiredEvent` — `EffectId`, `DisplayName`

**Stage / combat:**
- `StageStartedEvent` — `StageNumber`, `WaveCount`
- `WaveStartedEvent` — `WaveNumber`, `EnemyCount`
- `WaveCompletedEvent` — `WaveNumber`
- `BossSpawnedEvent` — `Boss`
- `StageCompletedEvent` — `StageNumber`
- `RespawnEvent` — `Player`, `Position`, `LivesRemaining`
- `GameOverEvent`
- `CheckpointReachedEvent` — `Checkpoint`
- `ComboUpdatedEvent` — `Combo`, `Multiplier`
- `ComboResetEvent`
- `CurrencyChangedEvent` — `CurrencyId`, `Delta`, `NewTotal`

**Interaction:**
- `InteractionFocusedEvent` — `PromptText`, `Target`
- `InteractionUnfocusedEvent`
- `InteractedEvent` — `InteractableId`, `Interactor`, `Target`

**Audio:**
- `StatusEffectAppliedEvent` — also used by AudioEventListener

### Adding a new event

```csharp
// 1. Create struct in Framework/Events/Events/Gameplay/
public struct MyNewEvent
{
    public string Data;
    public MyNewEvent(string data) { Data = data; }
}

// 2. Publish from anywhere
EventBus.Publish(new MyNewEvent("hello"));

// 3. Subscribe anywhere
EventBus.Subscribe<MyNewEvent>(e => Debug.Log(e.Data));
```

---

## 8. Gameplay Layer — AI

### AIController

**File:** `Gameplay/AI/AIController.cs`

The main MonoBehaviour for all AI agents. Add to any enemy or NPC.

**Inspector fields:**
- `Perception` — PerceptionSystem component on same GameObject
- `HealthComponent` — HealthComponent on same GameObject
- `Rb` — Rigidbody (optional, for stagger physics)
- `Player Tag` — tag used to find player at runtime (default "Player")
- `Ground Layer` — layer mask for ledge detection (2D only)
- `Team` — Enemy / Ally / Player / Neutral

**Lifecycle (important for pooling):**
- `Awake` → Initialize, creates context and systems, sets `_initialized = true`
- `Start` → Registers with SquadSystem
- `OnEnable` → Re-registers with SquadSystem (for pool reactivation)
- `OnDisable` → Unregisters from SquadSystem

**Adding a factory to an enemy:**
Add ONE of these components alongside AIController:
- `CombatAIStateFactory` — fights when it sees the player
- `PatrolAIStateFactory` — patrols/wanders, fights when it sees the player
- `TurretAIStateFactory` — stationary, rotates and shoots

**No factory component** → uses `DefaultCombatFactory` (idle until target spotted, then chases and attacks).

### AI Factory pattern

Every factory implements `IAIStateFactory` and its `Build()` method returns the starting state with all transitions wired:

```csharp
public class MyFactory : MonoBehaviour, IAIStateFactory
{
    public IState Build(StateContext context)
    {
        // Register abilities
        (context.Abilities as AbilitySystem)?.Register(
            BasicAttackAbility.Create(damage: 15, cooldown: 1f));

        // Create states
        var idle    = new IdleState();
        var chase   = new ChaseState(combat, idle);
        var combat  = new CombatState(new CombatStateConfig());
        var stagger = new StaggerState(combat);

        // Wire transitions
        idle.AddTransition(new Transition(
            new CanSeeTargetCondition(), chase));
        chase.AddTransition(new Transition(
            new IsInAttackRangeCondition(), combat));

        return idle; // starting state
    }
}
```

### AI Systems

All AI systems implement `IAISystem` and run each frame via `AISystemManager`.

| System | Category | What it writes to context |
|---|---|---|
| `AILODSystem` | Utility | `context.Execution.LOD` based on distance |
| `DirectorSystem` | Utility | `context.DirectorIntensity` from AIDirector |
| `SuppressionSystem` | Emotion | `context.Suppression` from nearby fire |
| `MoralSystem` | Emotion | `context.Morale` based on squad losses |
| `AlertSystem` | Emotion | `context.AlertLevel` transitions |
| `ThreatSystem` | Combat | `context.Target` best target selection |
| `SquadAISystem` | Squad | `context.SquadStrategy` from squad state |
| `FormationAISystem` | Squad | moves agents toward formation slots |
| `StatusEffectsAISystem` | Utility | ticks `context.StatusEffects` |
| `LedgeDetectionSystem` | Utility | `context.IsOnWall` at ledge edges (2D) |

**Adding a custom AI system:**
```csharp
public class MyAISystem : IAISystem
{
    public AISystemCategory Category => AISystemCategory.Utility;

    public bool ShouldRun(StateContext context)
        => context.AlertLevel == AlertLevel.Combat;

    public void Update(StateContext context)
    {
        // Read/write context — never call Unity APIs
        context.Fear += Time.deltaTime * 0.1f;
    }
}

// Register in AISystemsBootstrap.RegisterDefaults():
manager.Register(new MyAISystem());
```

### Perception System

**File:** `Gameplay/AI/Perception/PerceptionSystem.cs`

Detects targets using configurable sensors.

**Sensor types (set in Inspector via dropdown):**
- `OverlapSphere` — detects everything in radius
- `Cone` — FOV cone in front of agent
- `Raycast` — line of sight through walls
- `Trigger` — Unity trigger collider

**Swapping sensor at runtime:**
```csharp
var perception = GetComponent<PerceptionSystem>();
perception.SetSensor(new ConeSensor(transform, 12f, 90f, targetLayer));
```

### Squad System

**File:** `Gameplay/AI/Squad/SquadSystem.cs`

Three separate squads auto-sorted by team:

```csharp
var squad = ServiceLocator.Get<SquadSystem>();

squad.EnemySquad.Members.Count;  // how many enemies
squad.AllySquad.CurrentStrategy; // what allies are doing
squad.PlayerSquad.Members;       // all player-team agents

// Get target position for a team
Vector3 target = squad.GetTargetPosition(Team.Enemy);
```

**Registration is automatic.** AIController.Start() registers. OnDisable unregisters. PlayerStateController.Start() registers the player.

**Squad strategies (updated automatically by SquadAISystem):**
- `Idle` — no target, calm
- `Search` — lost target, looking
- `Chase` — target spotted, moving toward
- `Engage` — in combat range
- `Retreat` — morale < 0.3 or fear > 0.7

### Formation System

**File:** `Gameplay/AI/Formation/`

Enable formations by adding `SquadFormationSetup` to `_GameSystems`:
```
Enemy Formation: enabled, type=Wedge, spacing=2.5
Ally Formation:  enabled, type=Line,  spacing=2.0
```

Formation types: `None`, `Line`, `Wedge`, `Circle`

**FormationSystem** (`Gameplay/AI/Formation/FormationSystem.cs`) calculates each slot offset from the leader. Called by `FormationAISystem` each frame. You can query slot positions directly:

```csharp
var offset = FormationSystem.GetOffset(slotIndex, formationData);
Vector3 slotWorldPos = leader.position + offset;
```

Formation is tight during patrol/search, loose during combat.

### AIDirector

**File:** `Gameplay/AI/Director/AIDirector.cs`

Controls global game difficulty. `Intensity` (0-1) rises as more enemies spawn and engage. Feeds into music system and suppression system.

```csharp
var director = ServiceLocator.Get<AIDirector>();
director.State.Intensity;      // current difficulty 0-1
director.State.ActiveEnemies;  // enemies currently alive
director.State.MaxEnemies;     // configured maximum
```

---

## 9. Gameplay Layer — Health System

### Health types

| Type | Use case | Key fields |
|---|---|---|
| `Health` | Standard HP | Value, MaxValue |
| `ShieldedHealth` | Shield absorbs first | Shield, MaxShield |
| `RegenHealth` | HP regenerates over time | regenRate, regenDelay |
| `ArmouredHealth` | Flat + % damage reduction | Armour, ArmourPct |
| `SegmentedHealth` | HP in breakable segments | CurrentSegment, TotalSegments |
| `OvershieldHealth` | Bonus HP that decays | Overshield |
| `InvincibleHealth` | Takes no damage | — |
| `ElementalHealth` | Per-damage-type resistance | SetResistance() |
| `CompositeHealth` | Chains layers together | Add(), GetLayer<T>() |

**Composite presets in HealthComponent Inspector:**
- `ShieldedArmoured` → Shield → Armour → HP
- `ShieldedRegen` → Shield → RegenHP
- `SegmentedArmoured` → Armour → SegmentedHP
- `FullBoss` → Shield → Armour → SegmentedHP

### HealthComponent

**File:** `Gameplay/Systems/Health/HealthComponent.cs`

Add to any agent. Configure type and values in Inspector.

```csharp
var hc = GetComponent<HealthComponent>();

// Events — subscribe in OnEnable
hc.OnHit   += OnHit;    // Vector3 hitPoint
hc.OnDeath += OnDeath;  // no args

// Methods
hc.Damage(25, hitPoint);
hc.Heal(10);
hc.Reset();              // full HP (for respawn)
hc.SetInvincible(true);  // dash frames, cutscenes

// Read values
hc.GetHealth().Value;    // current HP
hc.GetHealth().IsDead;
```

### DamageType enum

**File:** `Framework/Systems/Damage/DamageType.cs`

```csharp
DamageType.Physical, Slash, Pierce, Blunt
DamageType.Fire, Ice, Lightning, Poison, Acid
DamageType.Holy, Dark, Arcane, Nature
DamageType.True   // bypasses all resistances and armour
DamageType.Heal   // negative damage
DamageType.Pure   // bypasses shields only
```

### CriticalHitSystem

```csharp
var result = CriticalHitSystem.Calculate(
    baseDamage:     20,
    critChance:     0.25f,  // 25% chance
    critMultiplier: 2f);    // double damage on crit

// result.FinalDamage — damage after crit calculation
// result.IsCritical  — whether it was a crit

// Or apply directly to a DamageInfo:
var info = new DamageInfo(20, DamageType.Slash);
info = CriticalHitSystem.Apply(info, 0.25f, 2f);
health.Damage(info.Amount, info.HitPoint);
```

### WeakpointComponent

Add to a child GameObject (e.g. head, eye):
```
Add SphereCollider → Is Trigger ON
Add WeakpointComponent
Set Multiplier: 2.0
Set VulnerableTo: Pierce, Fire (or leave empty = all types)
```

When hit, calls `Hit(DamageInfo)` which applies the multiplier and fires effects.

---

## 10. Gameplay Layer — Camera System

### How it works

All camera modes write into `CameraSnapshot`. `CameraModeController.LateUpdate` applies the snapshot to the camera each frame. Modes never write to `cam.transform` directly.

### Setup

Add to Main Camera:
- `CameraModeController` — Cam field = Main Camera
- `CameraBootstrap` — configure mode, player target, input handler

### 3D Camera modes

| Mode | Use case |
|---|---|
| `FreeLookCameraMode` | Orbits around player with mouse input |
| `ThirdPersonCameraMode` | Follows behind and above, full mouse look |
| `OverShoulderCameraMode` | Over-shoulder aim, rotates player body |
| `FPSCameraMode` | First person, head-mounted |
| `TopDownCameraMode` | Directly above looking straight down |
| `IsometricCameraMode` | Fixed angle, diagonal view (Diablo style) |
| `OrbitCameraMode` | Auto-rotates around a point, for menus |
| `FixedCameraMode` | Static position looking at a point |
| `CinematicCameraMode` | Moves between two points |

### 2D Camera modes

| Mode | Use case |
|---|---|
| `Camera2DFollowMode` | Smooth follow on XY |
| `Camera2DPlatformerMode` | Look-ahead, dead zones, loose vertical |
| `Camera2DTopDownMode` | Top-down 2D follow |
| `Camera2DConfinerMode` | Follow but stays within level bounds |

### Switching camera at runtime

```csharp
var controller = Camera.main.GetComponent<CameraModeController>();

controller.Request(new CameraRequest(
    new FreeLookCameraMode(playerTarget, inputState, 5f, 3f),
    CameraPriority.Normal,
    blendTime: 0.5f));
```

### Writing a new camera mode

```csharp
public class MyCamera : ICameraMode
{
    public void Activate(UnityEngine.Camera cam) { }
    public void Deactivate(UnityEngine.Camera cam) { }

    public void Tick(UnityEngine.Camera cam, float dt,
        ref CameraSnapshot snapshot)
    {
        // Write desired position and rotation
        // NEVER write to cam.transform directly
        snapshot.Position = desiredPosition;
        snapshot.Rotation = desiredRotation;
    }
}
```

---

## 11. Gameplay Layer — Audio System

### How it works

`IAudioSystem` in Framework has zero Unity/Wwise dependency. Two implementations exist — swap by changing ONE component on `_GameSystems`.

### UnityAudioSystem

Add to `_GameSystems`. Assign `AudioMixer` (optional). Add `AudioClipEntry` items to the `Clips` array mapping event id strings to AudioClips.

```csharp
// Usage (same API for both systems)
var audio = ServiceLocator.Get<IAudioSystem>();

audio.Play("explosion", worldPosition);
audio.PlayOnObject("footstep", playerGameObject);
audio.Stop("ambient_loop", sourceGameObject);
audio.SetParameter("MusicIntensity", 0.8f);
audio.SetState("Music", "Combat");
audio.SetMusicIntensity(0.8f);
```

### WwiseAudioSystem

Add to `_GameSystems` instead of UnityAudioSystem. In the Inspector configure:

**RTPC Mappings** — maps game values to Wwise RTPC parameters:
```
Element 0:
  Wwise RTPC Name: Intensity
  Source: DirectorIntensity
  Scale: 100
  Update Rate: 0

Element 1:
  Wwise RTPC Name: PlayerFear
  Source: PlayerFear
  Scale: 100
  Update Rate: 0.1
```

Available sources: `DirectorIntensity`, `PlayerFear`, `PlayerMorale`, `EnemyCount`, `MusicIntensity`

**State Groups** — default Wwise states on game start:
```
Element 0: Group=Music, Default=Explore
Element 1: Group=Ambience, Default=Forest
```

**Startup Events** — Wwise events posted immediately:
```
Element 0: Play_Music_Main
```

When Wwise package is installed, uncomment the `AkSoundEngine` calls in `WwiseAudioSystem.cs`.

### AudioEventListener

Add to `_GameSystems`. All fields are optional — leave empty to skip.

```
Health Sounds:
  On Damage Sound Id:   hit_flesh
  On Death Sound Id:    death_enemy
  Low Health Threshold: 0.25
  On Low Health Id:     heartbeat

Projectile Sounds:
  On Hit Sound Id:  bullet_impact

Inventory Sounds:
  On Item Added Id: item_pickup
```

### MusicSystem

Add to `_GameSystems`. Reads `AIDirector.Intensity` and `SquadStrategy` to auto-transition music states:
- Intensity < 0.2 → `Explore`
- Intensity 0.2-0.4 → `Stealth`
- Intensity > 0.4 or squad Engaging → `Combat`

```
Combat Threshold:  0.4
Tense Threshold:   0.2
Smooth Speed:      2
```

---

## 12. Gameplay Layer — 2D Support

### Movement strategies

**PlatformerMovementStrategy** — for side-scrolling platformers:
```csharp
// Auto-assigned by AIController when Rigidbody2D is present
// with gravityScale > 0

// Manual setup:
_context.Movement = new PlatformerMovementStrategy(
    rb2d,
    jumpForce: 10f,
    groundLayer: LayerMask.GetMask("Ground"));
```

**TopDown2DMovementStrategy** — for top-down RPGs, twin-stick shooters:
```csharp
// Auto-assigned when Rigidbody2D.gravityScale == 0

// Manual setup:
_context.Movement = new TopDown2DMovementStrategy(
    rb2d,
    rotateToFace: false);
```

### 2D Platformer player setup

```
Player GameObject:
  Rigidbody2D — Gravity Scale: 2, Freeze Z rotation
  CapsuleCollider2D
  PlayerStateController
  InputHandler
  HealthComponent

State graph in factory:
  idle → jump (JumpPressed + IsGrounded)
  jump → doubleJump (DoubleJumpCondition)
  jump → fall (IsAirborne)
  fall → land (LandedCondition)
  land → idle (LandFinishedCondition)
  any → dash (DashPressedCondition)
  dash → idle/move (DashFinishedCondition)
  any → wallSlide (IsOnWall + IsAirborne)
  wallSlide → jump (JumpPressed — wall jump)
```

### 2D AI setup

```
2D Enemy:
  Rigidbody2D — appropriate gravity
  AIController
  PatrolAIStateFactory — is2D = true, groundLayer set
```

`AIController` auto-detects Rigidbody2D, assigns `PlatformerMovementStrategy`, and calls `AISystemsBootstrap.Register2D()` which adds `LedgeDetectionSystem`.

### 2D Camera setup

```
Main Camera:
  Projection: Orthographic
  CameraModeController2D
  CameraBootstrap2D — Mode: Platformer
                       Player Target: Player transform
                       Smooth Speed: 6
                       Orthographic Size: 5
```

---

## 13. Gameplay Layer — Abilities & Combat

### Ability System

**AbilityDefinition ScriptableObject:**
Right click → Create → Gameplay → Ability Definition

Fields:
- `Ability Id` — unique string ("Attack", "RangedShot")
- `Ability Type` — MeleeAttack or RangedAttack
- `Damage` — damage per use
- `Cooldown` — seconds between uses
- `Priority` — higher = preferred by `UseBestAvailable`
- `Pool Key` — ranged only, must match pool entry

**In factory:**
```csharp
var system = context.Abilities as AbilitySystem;
system.Register(myAbilityDefinition.Build());
```

### Projectile

**Setup:**
1. Create bullet prefab with `Projectile` component
2. Set `Pool Key`, `Hit Layers`, `Speed`, `Lifetime`, `Damage`
3. Register pool in `GameBootstrap`

The `Projectile` component has a collider grace period (0.05s) — prevents hitting the spawner on the same frame as activation.

### StaggerSystem

Add alongside `HealthComponent`. Call `staggerSystem.Register(healthComp, rb)` in agent's `BindSystems`. Agent automatically staggers (freezes movement, plays stagger animation) when hit.

### KnockbackSystem

Add to any agent that should be knocked back on hit:
```
Force:         8        (knockback strength)
Upward Force:  3        (vertical component)
Duration:      0.2      (how long knockback lasts)
Is 2D:         true     (for Rigidbody2D agents)
```

### ComboSystem

Add to Player:
```
Combo Window:   1.5     (seconds before reset)
Max Combo:      10
Thresholds:
  Combo 3  → x1.2
  Combo 5  → x1.5
  Combo 10 → x2.0
```

Read `comboSystem.CurrentCombo` and `comboSystem.Multiplier` in `AttackState` to choose which animation to play and multiply damage.

---

## 14. Gameplay Layer — Progression

### Level System

Add `LevelSystem` to Player. Check `Register As Service` for global access.

```csharp
var levels = ServiceLocator.Get<ILevelSystem>();
levels.Level;      // current level
levels.Experience; // current XP
levels.NextLevel;  // XP needed for next level
levels.Progress;   // 0-1 progress bar value
levels.AddExperience(100);
levels.IsMaxLevel; // true at maxLevel (default 99)
```

**ExperienceCurve** (`Gameplay/Progression/ExperienceCurve.cs`) is a serializable class embedded in `LevelSystem`. You can also create standalone instances:

```csharp
var curve = new ExperienceCurve
{
    type       = CurveType.Exponential,
    baseXP     = 100,
    multiplier = 1.5f
};
int xpForLevel5 = curve.GetRequired(5);    // XP to go from 4→5
int totalXPFor5 = curve.GetCumulative(5);  // total XP from level 1 to 5
```

**XP Curve types (set in Inspector):**
- `Exponential` — steeper at high levels (default, D&D style)
- `Linear` — flat XP increase per level
- `Custom` — AnimationCurve from Inspector

**XPSource** — add to enemies. They grant XP on death automatically:
```
XP Amount:       50
Scale With Level: true  (scales XP by enemy's own level)
```

### Stat System

Add to Player alongside `LevelSystem`. Stats update automatically on `LevelUpEvent`.

```csharp
var stats = GetComponent<StatSystem>();
stats.Get("Strength");     // base + all bonuses
stats.GetBase("Agility");  // base value only
stats.AddBonus("Defence",  5f);   // equipment bonus
stats.RemoveBonus("Defence", 5f); // unequip
```

Default stats: `Strength`, `Agility`, `Vitality`, `Intelligence`, `Defence`

Add custom stats in Inspector by expanding the `Stats` list.

### Class System

Add `ClassSystem` to Player alongside `StatSystem`.

**ClassAsset** (`Gameplay/Progression/ClassAsset.cs`) is a ScriptableObject for authoring classes without code. Each asset has:
- `classId`, `displayName`, `description`
- `tier` (1=base, 2=evolved, 3=ascended)
- `requiredLevel` — minimum level to select/evolve to
- `statBonuses` — list of stat name + amount + isPercentage
- `abilityIds` — string list of abilities this class unlocks
- `evolutions` — list of ClassAsset references for next tier

**Setup:**
1. Create class assets: Right click → Create → Gameplay → Class
2. Fill in: id, displayName, tier, requiredLevel, statBonuses, abilityIds, evolutions
3. Drag all class assets into `ClassSystem.allClasses`
4. Drag Tier 1 classes into `ClassSystem.startingClasses`

```csharp
var classes = ServiceLocator.Get<IClassSystem>();
classes.SelectClass("warrior");
classes.CanEvolve("knight");      // true if level requirement met
classes.EvolveClass("knight");
classes.Current.DisplayName;      // "Knight"
classes.Available;                // evolution options
```

**To integrate a custom class system:**
```csharp
public class MyClassAdapter : MonoBehaviour, IClassSystem
{
    private MyFriendClassSystem _friend;

    private void Awake()
    {
        _friend = GetComponent<MyFriendClassSystem>();
        ServiceLocator.Register<IClassSystem>(this);
    }

    public void SelectClass(string id) => _friend.SetClass(id);
    public void EvolveClass(string id) => _friend.Ascend(id);
    public bool CanEvolve(string id)   => _friend.CanAscend(id);
    // ...
}
```

---

## 15. Gameplay Layer — Inventory & Items

### ItemDefinition ScriptableObject

Right click → Create → Gameplay → Item Definition

Fields: `Id`, `DisplayName`, `StackSize`, `Icon`, `Description`

### InventoryComponent

Add to Player, chests, enemies (for loot).

```csharp
var inv = GetComponent<InventoryComponent>();

inv.Add(healthPotionDef, 3);
inv.Remove("health_potion", 1);
inv.Has("quest_key");
inv.GetCount("gold_coin");

// Iterate
foreach (var slot in inv.Inventory.Slots)
    Debug.Log($"{slot.Item.DisplayName} x{slot.Count}");
```

Inventory saves and loads automatically via `ISaveable` — requires `ItemDatabase` to be set up.

### ItemDatabase

**Setup:**
1. Right click → Create → Gameplay → Item Database
2. Drag every `ItemDefinition` asset into the `Items` list
3. Add `ItemDatabaseLoader` to `_GameSystems`
4. Drag the database asset into `ItemDatabaseLoader`

```csharp
var db = ServiceLocator.Get<IItemDatabase>();
IItem item = db.Get("health_potion");
bool exists = db.Contains("sword");
IItem[] all = db.GetAll();
```

### LootSystem

Add to enemies. Create a `LootTable` asset (Right click → Create → Gameplay → Loot Table).

```
LootTable asset:
  Entries:
    Element 0: item=HealthPotion, dropChance=0.5, count=1-3
    Element 1: item=Gold,         dropChance=0.8, count=5-20

  Guaranteed:
    Element 0: item=XPOrb, dropChance=1.0, count=1

LootSystem component:
  Loot Table:    [drag LootTable asset here]
  Spawn In World: true
  Item Pickup Prefab: [prefab with InteractableItem]
  Scatter Radius: 1.5
```

### CurrencySystem

Add to `_GameSystems`.

```csharp
var currency = ServiceLocator.Get<CurrencySystem>();

currency.Get("gold");          // current gold amount
currency.Add("gold", 100);     // give 100 gold
currency.Spend("gold", 50);    // returns false if insufficient
currency.CanAfford("gold", 50);

// Multiple currency types configured in Inspector:
// Starting Currency:
//   Element 0: id=gold, amount=0
//   Element 1: id=gems,  amount=0
```

---

## 16. Gameplay Layer — Dialogue & Quests

### Dialogue System

**Setup:** Add `DialogueSystem` to `_GameSystems`.

**Creating dialogue assets:**
Right click → Create → Gameplay → Dialogue

Fill in: dialogueId, title, lines (speaker, text, portraitKey, audioEventId)

**Triggering dialogue:**
Add `DialogueTrigger` to NPC:
```
Dialogue Asset: [drag asset]
Only Once:      true
Auto Trigger:   false    (or true with radius)
Interaction Prompt: "Talk"
```

`DialogueTrigger` implements `IInteractable` — works automatically with `InteractionSystem`.

**In code:**
```csharp
// Via service
var dialogue = ServiceLocator.Get<IDialogueSystem>();
dialogue.Play(myDialogueAsset.Build());
dialogue.Advance();
dialogue.Choose(0);   // for branching
dialogue.Stop();

// React to events
EventBus.Subscribe<DialogueLineEvent>(e => {
    speakerText.text  = e.SpeakerName;
    dialogueText.text = e.Text;
});
EventBus.Subscribe<DialogueStartedEvent>(e => ShowPanel());
EventBus.Subscribe<DialogueEndedEvent>(e => HidePanel());
```

**Programmatic branching:**
```csharp
var tree = new DialogueTree("intro", "Guard Encounter")
    .Add(new DialogueLine("Guard", "Halt! Who goes there?"))
    .Add(new DialogueLine("Guard", "State your business."))
    .AddBranch(
        new[] { "I'm a traveler", "None of your business" },
        new[] { friendlyBranch, hostileBranch });
```

### Quest System

**Setup:** Add `QuestSystem` and `QuestBootstrap` to `_GameSystems`.

**Creating quest assets:**
Right click → Create → Gameplay → Quest

Fill in: questId, title, description, objectives (id, description, required count), autoStart.

**Progressing objectives from any system:**
```csharp
var quests = ServiceLocator.Get<IQuestSystem>();

quests.StartQuest("rescue_villager");
quests.Progress("rescue_villager", "kill_bandits", 1);
quests.IsComplete("rescue_villager");
quests.IsActive("find_sword");
```

Quest auto-completes when all objectives are done and fires `QuestCompletedEvent`.

**React to quest events:**
```csharp
EventBus.Subscribe<QuestStartedEvent>(e =>
    ShowBanner($"New Quest: {e.Title}"));

EventBus.Subscribe<QuestObjectiveUpdatedEvent>(e =>
    UpdateTracker(e.Description, e.Current, e.Required));

EventBus.Subscribe<QuestCompletedEvent>(e =>
    ShowCompletion(e.Title));
```

---

## 17. Gameplay Layer — Interaction

### InteractionSystem

Add to Player.

```
Interact Range:  2
Interact Layer:  [layer mask with interactables]
Input Handler:   [player InputHandler]
```

Player walks near any `IInteractable` object → `InteractionFocusedEvent` fires (UI shows prompt). Player presses Interact (`E` key or `InputState.InteractPressed`) → `Interact()` called.

### Making anything interactable

```csharp
public class MyInteractable : MonoBehaviour, IInteractable
{
    public string PromptText => "Examine";

    public bool CanInteract(Transform interactor) => true;

    public void Interact(Transform interactor)
    {
        // Do whatever this object does
        Debug.Log("Examined!");
    }
}
```

Built-in interactables:
- `InteractableDoor` — open/close/lock, optional Animator
- `InteractableItem` — pickup → adds to inventory, returns to pool
- `DialogueTrigger` — starts dialogue tree

### CheckpointTrigger

```
Add empty GameObject
Add BoxCollider — Is Trigger ON
Add CheckpointTrigger
Player Tag: Player
Activate Once: true
```

When player walks through, sets respawn point and fires `CheckpointReachedEvent`.

---

## 18. Gameplay Layer — Status Effects

### Built-in effects

| Effect | Id | What it does |
|---|---|---|
| `BurnEffect` | "burn" | Fire damage per second, refreshes |
| `PoisonEffect` | "poison" | Poison damage per second, stackable |
| `FreezeEffect` | "freeze" | Stops movement each frame |
| `StunEffect` | "stun" | Stops movement, clears commands |
| `SlowEffect` | "slow" | Reduces `context.SpeedMultiplier` |

### Applying effects

```csharp
// From any state or system
context.StatusEffects?.Apply(
    new BurnEffect(damage: 5, duration: 3f), context);

context.StatusEffects?.Apply(
    new SlowEffect(slowAmount: 0.5f, duration: 2f), context);
```

### Writing a custom effect

```csharp
public class BleedEffect : StatusEffectBase
{
    public override string Id          => "bleed";
    public override string DisplayName => "Bleeding";
    public override bool   CanStack    => true; // multiple sources stack

    private readonly int   _damage;
    private float          _tick;

    public BleedEffect(int damage = 3, float duration = 5f)
        : base(duration)
    {
        _damage = damage;
    }

    protected override void OnTick(StateContext ctx, float dt)
    {
        _tick += dt;
        if (_tick < 0.5f) return;  // tick every 0.5 seconds
        _tick = 0f;
        ctx.HealthComp?.Damage(_damage, ctx.Self.position);
    }
}
```

---

## 19. Gameplay Layer — Stage & Respawn

### StageSystem

Add to `_GameSystems`.

**WaveData** (`Gameplay/Systems/WaveData.cs`) defines one wave inside a stage. Each wave has:
- `enemies` — list of `EnemySpawnEntry` (prefab, count, spawnPoints, spawnDelay)
- `startDelay` — seconds before wave begins
- `isBossWave` — if true, boss spawns after all enemies die
- `bossPrefab` + `bossSpawnPoint`

**StageData** (`Gameplay/Systems/WaveData.cs`) is a ScriptableObject containing a list of WaveData plus XP reward and completion loot.

**Create a StageData asset:**
Right click → Create → Gameplay → Stage

```
Stage Number: 1
Stage Name:   Forest Stage

Waves:
  Wave 0:
    Start Delay: 1
    Enemies:
      Entry 0: prefab=Goblin, count=5, spawnPoints=[sp1,sp2,sp3]
      Entry 1: prefab=Archer, count=2, spawnDelay=3, spawnPoints=[sp4]
    Is Boss Wave: false

  Wave 1:
    Start Delay: 2
    Enemies:
      Entry 0: prefab=EliteGoblin, count=3
    Is Boss Wave: true
    Boss Prefab: GoblinKing
    Boss Spawn Point: [bossSpawn]

XP Reward: 500
Completion Loot: [LootTable asset]
```

**Starting a stage:**
```csharp
var stage = ServiceLocator.Get<StageSystem>();
stage.StartStage(myStageData);       // kicks off wave 1
stage.ForceCompleteStage();          // skip to completion
```

### RespawnSystem

Add to Player.

```
Mode:          Checkpoint   (or StageStart, Instant)
Respawn Delay: 3
Max Lives:     3
Infinite Lives: false
Start Position: [Transform]
```

```csharp
// React to game over
EventBus.Subscribe<GameOverEvent>(() => LoadMainMenu());

// React to respawn
EventBus.Subscribe<RespawnEvent>(e =>
    ShowMessage($"{e.LivesRemaining} lives remaining"));
```

---

## 20. Gameplay Layer — Network Foundation

The framework is multiplayer-ready. Currently running in single-player mode with stub implementations. No code changes needed until you add a networking library.

### Current state (single player)

Add `OfflineTransport` to `_GameSystems` — always reports IsServer=true, IsOwner=true.

Add `LocalNetworkEntity` to Player and enemies — always IsOwner=true.

### To add multiplayer (NGO example)

```csharp
// 1. Create adapter — one file, zero changes to existing code
public class NGONetworkEntity : NetworkBehaviour, INetworkEntity
{
    public int  NetworkId     => (int)NetworkObjectId;
    public bool IsOwner       => base.IsOwner;
    public bool IsServer      => base.IsServer;
    public bool IsLocalPlayer => base.IsLocalPlayer;

    public void OnNetworkSpawn()   { }
    public void OnNetworkDespawn() { }
}

// 2. Gate AI on server only
// In AIController.Update():
var net = GetComponent<INetworkEntity>();
if (net != null && !net.IsServer) return;

// 3. Gate input on owner only
// In PlayerStateController.Update():
var net = GetComponent<INetworkEntity>();
if (net != null && !net.IsOwner) return;
```

See `MULTIPLAYER_GUIDE.md` for full step-by-step NGO integration.

---

## 21. Behaviour Tree

An alternative to the state machine for complex AI. Both use `StateContext` — fully compatible.

### When to use behaviour tree vs state machine

Use state machine for: simple enemies, turrets, player controllers, few states

Use behaviour tree for: complex guards, companions, boss AI, many conditions and fallbacks

### Node types

| Type | Behaviour |
|---|---|
| `SequenceNode` | AND — all children must succeed left to right |
| `SelectorNode` | OR — tries each child, returns first success |
| `ParallelNode` | Runs ALL children simultaneously |
| `InverterNode` | Flips Success ↔ Failure |
| `RepeaterNode` | Repeats child N times or forever |
| `CooldownNode` | Prevents child running more than once per cooldown |
| `ConditionNode` | Evaluates a lambda predicate |
| `ActionNode` | Executes a lambda action |

### Building a tree

```csharp
// SelectorNode = try first option, else try second, else third
var root = new SelectorNode("Root")
    .Add(new SequenceNode("Attack")
        .Add(new ConditionNode("CanSee",
            ctx => ctx.VisibleTargets?.Count > 0))
        .Add(new CooldownNode(
            new ActionNode("Strike",
                ctx => ctx.HealthComp?.Damage(10, ctx.Self.position)),
            cooldown: 1.5f)))
    .Add(new SequenceNode("Chase")
        .Add(new ConditionNode("CanSee",
            ctx => ctx.VisibleTargets?.Count > 0))
        .Add(new ActionNode("Move",
            ctx => ctx.Commands.Enqueue(
                new MoveCommand(ctx.Self,
                    (ctx.Target.position - ctx.Self.position).normalized,
                    3f, ctx.Movement)))))
    .Add(new ActionNode("Idle",
        ctx => ctx.Movement?.Stop(ctx.Self)));
```

### Using BehaviourTreeController

```csharp
public class MyEnemyController : BehaviourTreeController
{
    protected override IBehaviourNode BuildTree()
    {
        // Return your root node
        return new SelectorNode("Root")
            .Add(/* attack branch */)
            .Add(/* patrol branch */);
    }
}
```

Add `MyEnemyController` to enemy instead of `AIController`. Everything else (perception, squad, status effects) works identically.

---

## 22. Editor Tools & Debug

### Custom Inspectors (Editor only)

All visible during Play Mode when agent is selected in Hierarchy.

**AIControllerEditor** — shows live bars for:
- Fear, Morale, Suppression (0-1 bars)
- LOD level, Alert level
- Current target name

**HealthComponentEditor** — shows:
- Color-coded health bar (green → yellow → red)
- Shield bar (blue)
- Type info

**PerceptionSystemGizmos** — shows in Scene view:
- Vision radius disc
- Attack range disc
- FOV arc

**FormationGizmos** — shows in Scene view (select `_GameSystems`):
- Formation slot positions as colored discs
- Dotted lines from leader to each slot
- Separate colors per squad (Enemy/Ally/Player)

### AIDebugOverlay

**File:** `Gameplay/Tools/AIDebugOverlay.cs`

Only compiled in Editor and Development builds.

Add to `_GameSystems`:
```
Toggle Key:     F1
Cycle Mode Key: F2
Show On Start:  false
```

Hit Play → F1 to show overlay. F2 to cycle between:
- `Minimal` — state name floating above each agent in world
- `Full` — full panel top-left with all agent data
- `Both` — both simultaneously

Panel shows per agent: name, team, state, LOD, alert, morale, fear, suppression, HP bar, squad strategy, current target.

---

## 23. Unit Tests

**Location:** `Assets/Tests/Tests/`

Run via: Window → General → **Test Runner** → EditMode → Run All

| Suite | Count | What it verifies |
|---|---|---|
| `HealthTests` | 17 | All health types, damage, healing, death events, composite routing |
| `EventBusTests` | 6 | Subscribe, publish, unsubscribe, type isolation, clear |
| `ServiceLocatorTests` | 8 | Register, get, unregister, null safety, overwrite |
| `InventoryTests` | 10 | Add, remove, stacking, capacity limits, has |
| `CommandQueueTests` | 4 | Enqueue, execute order, clear, empty safety |
| `PerceptionSensorTests` | 3 | MockSensor contract, null safety, empty list |

**Adding a test:**
```csharp
// In Tests/Tests/MyTests.cs
public class MyTests
{
    [Test]
    public void MySystem_DoesTheRightThing()
    {
        var system = new MySystem();
        system.DoSomething(42);
        Assert.AreEqual(42, system.LastValue);
    }
}
```

---

## 24. Scene Setup Checklist

### _GameSystems GameObject

Required components:
- `SceneLifecycleManager`
- `GameBootstrap` — pool entries configured
- `SquadSystem`
- `AIAgentRegistry`
- `AIGroupManager`
- `AIDirector`
- `PauseSystem`
- `SceneLoader`

Audio (pick one or both):
- `UnityAudioSystem` or `WwiseAudioSystem`
- `AudioEventListener`
- `MusicSystem`

Optional but recommended:
- `SquadFormationSetup`
- `ItemDatabaseLoader`
- `QuestSystem` + `QuestBootstrap`
- `DialogueSystem`
- `StageSystem`
- `CurrencySystem`
- `OfflineTransport`
- `AIDebugOverlay` (Editor/Dev builds only)

### Main Camera GameObject

- `CameraModeController` — Cam = this camera
- `CameraBootstrap` — configure mode, player target, input handler

### Player GameObject

- `PlayerStateController` — input handler, health component
- `InputHandler` — PlayerInputActions asset
- `HealthComponent` — configure type and values
- `LevelSystem` — Register As Service = true
- `StatSystem`
- `ClassSystem` (optional)
- `InventoryComponent`
- `InteractionSystem`
- `RespawnSystem` (optional)
- `ComboSystem` (optional)
- `PlayerLook` — Is FPS Mode = false for most modes
- Tag: `Player`

### Enemy Prefab

- `AIController` — perception, healthComponent, rb
- `PerceptionSystem`
- `HealthComponent`
- `StaggerSystem`
- `DeathSystem` — poolKey set
- `KnockbackSystem` (optional)
- `XPSource`
- `LootSystem` + `LootTable` (optional)
- ONE factory: `CombatAIStateFactory`, `PatrolAIStateFactory`, or `TurretAIStateFactory`
- `Rigidbody` or `Rigidbody2D`
- `Collider`

---

## 25. Export & Package Guide

### Framework standalone export

`Framework/` folder exports with zero errors to any blank Unity 6 project. It has no dependencies other than `UnityEngine`.

### Gameplay dependency groups

When exporting specific Gameplay systems, bring these groups together:

| Group | Contents | Depends on |
|---|---|---|
| A — Core AI | AI/, States/, Faction/, Threat/, Group/, LOD/, Memory/ | Framework |
| B — Squad | Squad/, Formation/, Director/, Moral/, Alert/, Suppression/ | Group A |
| C — Abilities | Abilities/, Systems/Projectiles/ | Group A |
| D — Full AI | All AI + AI/States/ | A + B + C |
| E — Player | Player/, Input/, Camera/, Systems/Health/, Systems/Movement/ | Framework |
| F — UI + Systems | UI/, PauseSystem, SceneLoader, Bootstrap/ | Framework |
| G — Persistence | HealthSaveable, Items/ | Framework/Persistence/ |
| H — Progression | Progression/ | Framework/Progression/ |
| I — 2D | States/Jump,Fall,Land,Dash, Systems/Movement/Platformer, Camera/2D/ | Framework |
| J — Gameplay | Dialogue, Quests, Interaction, StatusEffects, Combat | Framework |
| K — Stage | Systems/Stage, Systems/Respawn | Framework + E |

**Minimum viable export for a new game:**
```
Framework/     (always — entire folder)
+ Group A      (if you need AI)
+ Group C      (if you need abilities)
+ Group E      (if you need a player)
+ Group F      (always recommended)
```

### Making it installable via UPM

The `package.json` in the repository root makes the framework installable in any Unity project:

```
Window → Package Manager → + → Add package from git URL
https://github.com/SeaOfFlyingCorpses/UnityGameplayArchitectureFrameworkMichael.git
```

---

## 26. Extending The Framework

### Adding a new health type

```csharp
// 1. Implement IHealth in Gameplay/Systems/Health/
public class DrainHealth : IHealth
{
    public int   Value    { get; private set; }
    public int   MaxValue { get; private set; }
    public bool  IsDead   => Value <= 0;
    public event Action<int> OnChanged;
    public event Action      OnDeath;

    public void Damage(int amount) { /* drain logic */ }
    public void Heal(int amount)   { /* restore logic */ }
    public void Reset()            { Value = MaxValue; }
}

// 2. Add to HealthType enum in HealthComponent
case HealthType.Drain: return new DrainHealth(maxHealth);
```

### Adding a new camera mode

```csharp
// In Gameplay/Camera/Modes/
public class ShakeCamera : ICameraMode
{
    public void Activate(UnityEngine.Camera cam) { }
    public void Deactivate(UnityEngine.Camera cam) { }

    public void Tick(UnityEngine.Camera cam, float dt,
        ref CameraSnapshot s)
    {
        s.Position += UnityEngine.Random.insideUnitSphere * 0.1f;
    }
}

// Add to CameraBootstrap enum and BuildMode() switch
```

### Adding a new AI system

```csharp
public class FlankerSystem : IAISystem
{
    public AISystemCategory Category => AISystemCategory.Combat;

    public bool ShouldRun(StateContext ctx)
        => ctx.AlertLevel == AlertLevel.Combat;

    public void Update(StateContext ctx)
    {
        // Flanking logic — read/write context
    }
}

// Register in AISystemsBootstrap.RegisterDefaults()
manager.Register(new FlankerSystem());
```

### Adding a new status effect

```csharp
public class BlindEffect : StatusEffectBase
{
    public override string Id          => "blind";
    public override string DisplayName => "Blinded";

    public BlindEffect(float duration = 2f) : base(duration) { }

    protected override void Apply(StateContext ctx)
    {
        // Reduce perception range
        if (ctx.PerceptionContext?.State != null)
            ctx.PerceptionContext.State.Range *= 0.1f;
    }

    protected override void Expire(StateContext ctx)
    {
        if (ctx.PerceptionContext?.State != null)
            ctx.PerceptionContext.State.Range /= 0.1f;
    }
}
```

### Adding a new ability type

```csharp
// In AbilityDefinition.cs — add to AbilityType enum:
public enum AbilityType { MeleeAttack, RangedAttack, Heal, AOESlam }

// Add fields:
public int healAmount = 20;

// Add case in Build():
case AbilityType.Heal:
    return new Ability
    {
        Id       = abilityId,
        Cooldown = cooldown,
        Priority = priority,
        Execute  = ctx => ctx.SourceHealth?.Heal(healAmount)
    };
```

### Adding a new event

```csharp
// 1. Define struct in Framework/Events/Events/Gameplay/
public struct PlayerRolledEvent
{
    public int Result;
    public PlayerRolledEvent(int r) { Result = r; }
}

// 2. Publish anywhere
EventBus.Publish(new PlayerRolledEvent(6));

// 3. Subscribe anywhere (unsubscribe in OnDisable!)
EventBus.Subscribe<PlayerRolledEvent>(e =>
    Debug.Log($"Rolled: {e.Result}"));
```

---

*Framework version 1.0.0 — Unity 6 — Last updated: 2026*
*286 files across Framework and Gameplay assemblies*
