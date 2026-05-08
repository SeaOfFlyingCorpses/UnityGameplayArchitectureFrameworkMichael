# Changelog

## [1.0.0] — 2026

### Added — Phase 0: Core Framework
- ServiceLocator — replaces all static singletons
- EventBus — zero-coupling event system with scene lifetime management
- CommandQueue — decouples intent from execution, prediction-ready
- StateMachine — modular IState/ITransitionCondition pattern
- StateContext — shared data container, zero Gameplay imports
- ObjectPool — prefab deactivation prevents Awake during pre-warm
- SaveSystem — ISaveable pattern, JSON persistence
- SceneLoader — safe scene transitions via EventBus
- PauseSystem — subscribes to GamePausedEvent

### Added — Health System
- Health, ShieldedHealth, RegenHealth, ArmouredHealth
- SegmentedHealth, OvershieldHealth, InvincibleHealth
- ElementalHealth with DamageType resistances
- CompositeHealth — chain any health types together
- HealthComponent with IsInvincible, Reset, Heal

### Added — AI Systems
- AIController with pool-safe squad registration
- Per-agent AISystemManager with LOD filtering
- AILODSystem, DirectorSystem, SuppressionSystem
- MoralSystem, AlertSystem, ThreatSystem
- SquadAISystem, FormationAISystem, StatusEffectsAISystem
- PerceptionSystem with 5 sensor types
- ChaseState, CombatState, PatrolState, SearchState, StrafeState
- CombatAIStateFactory, PatrolAIStateFactory, TurretAIStateFactory
- AIAgentRegistry with GetAll for editor tools

### Added — Ability System
- AbilitySystem with UseBestAvailable priority selection
- AbilityDefinition ScriptableObject
- BasicAttackAbility (melee), ProjectileAbility (ranged)
- Projectile with collider grace period

### Added — Squad System
- SquadSystem with Enemy/Ally/Player squads
- FormationSystem with Wedge/Line/Circle/None types
- SquadFormationSetup for Inspector configuration
- PlayerStateController registers with PlayerSquad

### Added — Phase 1: Audio
- IAudioSystem — zero Unity/Wwise dependency in interface
- UnityAudioSystem — pooled AudioSources, AudioMixer support
- WwiseAudioSystem — expandable RTPC mappings, state groups
- AudioEventListener — expandable per-category sound mappings
- MusicSystem — intensity-driven automatic state transitions

### Added — Phase 2: 2D Support
- CameraSnapshot2D, ICameraMode2D
- Camera2DFollowMode, Camera2DPlatformerMode (look-ahead, dead zones)
- Camera2DTopDownMode, Camera2DConfinerMode (bounds clamping)
- CameraModeController2D, CameraBootstrap2D
- PlatformerMovementStrategy — jump, wall check, IsGrounded
- TopDown2DMovementStrategy — XY movement, optional rotation
- JumpState, FallState, LandState, WallSlideState
- DashState with invincibility frames and cooldown
- DoubleJumpState with configurable extra jump count
- IsGroundedCondition, IsAirborneCondition, JumpPressedCondition
- DashPressedCondition, DashFinishedCondition, DoubleJumpCondition
- LandedCondition, LandFinishedCondition
- PlatformerChaseState — horizontal chase with gap jumping
- LedgeDetectionSystem — writes IsOnWall to StateContext

### Added — Phase 3: Expanded Gameplay
- Dialogue: IDialogueSystem, DialogueTree, DialogueAsset, DialogueTrigger
- Quests: IQuestSystem, Quest, QuestObjective, QuestAsset, QuestBootstrap
- Damage: DamageType enum, DamageInfo, IDamageSource, CriticalHitSystem
- WeakpointComponent — body part damage multiplier
- Status Effects: IStatusEffect, Burn, Poison, Freeze, Stun, Slow
- Interaction: IInteractable, InteractionSystem, InteractableItem, InteractableDoor
- Level System: LevelSystem, StatSystem, XPSource, ExperienceCurve
- Class System: ClassSystem, ClassAsset, CharacterClass with branching evolution
- Loot: LootTable ScriptableObject, LootSystem
- Stage: StageSystem, WaveData, StageData with boss waves
- Respawn: RespawnSystem with checkpoints and lives
- Combo: ComboSystem with damage multiplier thresholds
- Currency: CurrencySystem with multiple currency types

### Added — Phase 4: Multiplayer Foundation
- INetworkEntity, INetworkTransport, INetworkStateSync
- NetworkStateSnapshot with delta compression
- OfflineTransport — single player stub
- LocalNetworkEntity, LocalNetworkStateSync

### Added — Phase 5: Tools and Polish
- Behaviour Tree: SequenceNode, SelectorNode, ParallelNode
- InverterNode, RepeaterNode, CooldownNode
- ConditionNode, ActionNode, BehaviourNodeBase
- BehaviourTree runner, BehaviourTreeController
- AIDebugOverlay — F1 toggle, F2 mode cycle, world labels + panel
- Profiling markers in AISystemManager, CommandQueue, Pool, Camera, Perception
- ItemDatabase ScriptableObject with duplicate detection
- KnockbackSystem — 2D/3D force on hit

### Added — Editor Tooling
- AIControllerEditor — live StateContext bars
- PerceptionSystemGizmos — FOV arc, range discs
- FormationGizmos — squad slot visualization
- HealthComponentEditor — color-coded health bar

### Added — Unit Tests
- 48 tests across Health, EventBus, ServiceLocator, Inventory, CommandQueue, Perception
