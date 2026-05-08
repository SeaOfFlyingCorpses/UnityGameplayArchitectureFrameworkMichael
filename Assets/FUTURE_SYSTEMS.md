# Future Systems — Per Game Type

This document lists systems to add to the framework
for specific game genres. Add to Framework or Gameplay
following the same patterns in FRAMEWORK_DOCUMENTATION.md.

---

## For ALL Games (Add Next)

```
SkillHotbarSystem
  Map abilities 1-9 to keyboard slots
  Player assigns abilities to slots
  Reads InputState.SlotPressed[9]
  Critical for any action RPG or MMO-like game

EquipmentSystem
  Weapon/armor slots (head, chest, legs, weapon, offhand)
  Each equipment piece has StatBonuses
  Plugs into StatSystem.AddBonus / RemoveBonus
  Works with InventoryComponent

BuffSystem
  Like StatusEffects but positive
  Haste, Strength Up, Shield Buff, etc.
  Same IStatusEffect interface — just positive effects
  Distinguish via a BuffType enum

DamageNumberSystem
  Floating text showing damage dealt
  Subscribes to HealthChangedEvent
  Critical hit shows different color/size
  Zero coupling — purely visual layer

MinimapSystem
  Renders agent positions on a RenderTexture
  Subscribes to AIAgentRegistry.GetAll()
  Player position, enemy positions, objectives
```

---

## 2D Platformer / MapleStory-style

```
AirDashState
  Horizontal dash while airborne
  Different from grounded dash
  MapleStory-style Flash Jump

ClimbState
  Ladder/rope climbing
  Vertical movement only
  AnimationType.Climb already in enum

TeleportAbility
  Blink to cursor or fixed distance
  Many MapleStory classes have this

HitStopSystem
  Brief time freeze on heavy hits
  Extremely impactful for game feel
  Set Time.timeScale to 0.1 for 3-5 frames

ScreenShakeSystem
  CameraShakeEffect already exists
  Extend with magnitude curves
  Subscribe to ProjectileHitEvent, OnDeath

AttackChainSystem
  Specific combo sequences (light-light-heavy)
  Different from ComboSystem (which just counts)
  State graph: Attack1 → Attack2 → Attack3 → Finisher

ParallaxBackground
  Layered scrolling backgrounds
  Different scroll speeds per layer
  Essential for 2D platformer feel

PlatformController
  One-way platforms (drop through)
  Moving platforms that carry the player
  Currently not in framework

SummonSystem
  Spawn allied units that follow and fight
  MapleStory has many summon classes
  Extension of existing squad system
```

---

## Turn-Based / Fire Emblem-style

```
GridSystem
  2D/3D grid of tiles
  Tile types (forest, fort, mountain) with movement cost
  A* pathfinding on grid
  Most important addition for this genre

TurnManager
  Who acts when
  Player phase / enemy phase / ally phase
  Action points per unit

GridMovementStrategy
  Implements IMovementStrategy for grid movement
  Move to tile, cost calculation
  Plugs into existing AIController

CombatForecastSystem
  Preview damage before confirming attack
  Hit rate, crit rate, damage range
  Reads StatSystem values

RelationshipSystem
  Unit bonds increase with proximity/battles
  Stat bonuses between paired units
  Fire Emblem support system

WeaponTriangleSystem
  Rock/paper/scissors weapon advantage
  Sword beats Axe beats Lance beats Sword
  Extends DamageType with advantage lookup

TerrainSystem
  Tile bonuses (defense%, avoid%)
  Affects combat calculations
  Reads from GridSystem

FogOfWarSystem
  Hide tiles outside vision range
  Reveal on unit proximity
  Subscribes to PerceptionSystem
```

---

## FPS

```
WeaponSystem
  Primary/secondary/melee slot
  Magazine, ammo reserves, reload
  Extends AbilitySystem with reload states

RecoilSystem
  Camera kick on fire
  Recovery curve
  Extends CameraShakeEffect

AimDownSights
  Toggle ADS mode
  FOV change, accuracy buff
  New camera mode or camera modifier

FootstepSystem
  Audio based on ground material
  Reads Rigidbody velocity magnitude
  Uses SwitchGroups in Wwise

BulletPenetrationSystem
  Bullets pass through thin surfaces
  LayerMask-based material thickness
  Extends Projectile.OnTriggerEnter

WeaponSwaySystem
  Weapon mesh offset based on mouse movement
  Procedural animation
  Reads InputState.Look

SprintState
  Faster movement, can't fire
  Stamina drain optional
  Transitions: Move → Sprint (SprintPressed)

ProceduralFootIK
  Feet placement on uneven terrain
  Unity Animation Rigging integration
```

---

## RTS

```
ResourceSystem
  Gold, wood, stone, food
  Tick-based income
  Extends CurrencySystem with tick rate

BuildingSystem
  Place buildings on grid
  Construction time, requirements
  Buildings as special agents

ProductionQueue
  Queue units for training at buildings
  Time-based, consumes resources
  One queue per building

FogOfWarSystem
  (Same as Fire Emblem version)
  Per-unit vision radius
  Subscribes to PerceptionSystem

GroupSelectionSystem
  Box-select multiple units
  Works with AIAgentRegistry.GetAll()
  Move group with formation

MiniatureSquadSystem
  Much larger squad counts than current system
  Current SquadSystem works for ~10-20 agents
  For 100+ need spatial hashing

TechTreeSystem
  Unlock new units/abilities/buildings
  Prerequisite graph
  Extends ClassSystem concepts

AttackMoveCommand
  Unit moves toward target, attacks anything in range
  New ICommand implementation
```

---

## Horror

```
SanitySystem
  Like MoralSystem but for player
  Drops in darkness, near monsters
  Affects vision, audio distortion
  Reads LightIntensity from scene

FearSystem
  Separate from AI fear
  Player-specific emotional state
  Triggers events at thresholds

LightingInteractionSystem
  Enemies avoid light
  Player uses light as a tool
  Reads Unity Light intensity

ProximityHorrorSystem
  Triggers audio/visual effects based on
  distance to nearest enemy
  Subscribes to AIAgentRegistry

ChaseSequenceSystem
  Scripted unstoppable pursuer
  Different from normal AI
  Triggered by StageSystem

InventoryLimitSystem
  Strict item count limit
  Resident Evil-style management tension
  Extension of InventoryComponent capacity

```

---

## Roguelike

```
ProceduralRoomSystem
  Generate connected rooms
  Spawn enemies and loot per room
  Works with existing StageSystem

DungeonGraphSystem
  Map of rooms as a graph
  Track visited/unvisited rooms
  Connects to MapSystem

RunDataSystem
  Track current run stats
  Resets on death (roguelike loop)
  Separate from SaveSystem

RelicSystem
  Passive upgrades found during run
  Each relic modifies StatSystem or game rules
  Similar to ClassSystem bonuses

CurseSystem
  Negative relics / challenge modifiers
  Inverse of BuffSystem
  Some roguelikes let you choose curse for reward

MetaProgressionSystem
  Unlocks persist between runs
  Separate SaveSystem slot
  Currencies earned per run
```

---

## Open World / Sandbox

```
TimeOfDaySystem
  Day/night cycle
  Affects lighting, enemy behavior, events
  Subscribes AIDirector to time of day

WeatherSystem
  Rain, snow, fog states
  Affects movement (SlowEffect), visibility
  Wwise State Groups map to weather

WorldPersistenceSystem
  Remember which enemies were killed
  Which items were picked up
  Per-sector save data, extends SaveSystem

DynamicSpawnSystem
  Enemies respawn over time
  Density based on player distance
  Extends AIDirector spawning logic

FastTravelSystem
  Discover locations, teleport to them
  Works with CheckpointTrigger concept
  Gated by discovery flag

ReputationSystem
  Faction-based relationships
  Extends TeamRelationship system
  Actions affect standing with factions

CraftingSystem
  Combine items to create new ones
  Recipe ScriptableObjects
  Works with InventoryComponent
```

---

## Multiplayer-specific (when adding co-op)

```
LobbySystem
  Matchmaking, room creation
  Unity Gaming Services integration
  Before NGO transport

PartySystem
  Shared XP, shared loot distribution
  Party members on PlayerSquad
  PartyLeader role assignment

SharedObjectiveSystem
  Quest objectives shared across party
  One player progresses, all benefit
  Extends QuestSystem

DropInDropOutSystem
  Seamless join mid-session
  Player spawns at party leader position
  Works with RespawnSystem

VoiceChatSystem
  Spatial audio, proximity-based
  Wwise + Vivox or Discord GameSDK
  Toggle with InputState
```

---

## UI Systems (separate concern, game-specific)

```
HUDSystem
  Health bar, mana bar, XP bar
  Subscribes to HealthChangedEvent, LevelUpEvent
  Each game has unique HUD layout

SkillHotbarUI
  Slots 1-9 showing ability icons + cooldowns
  Reads from AbilitySystem
  Subscribes to ability use events

QuestTrackerUI
  Active quest objectives on screen
  Subscribes to QuestObjectiveUpdatedEvent

DialogueUI
  Speaker portrait, text, choice buttons
  Subscribes to DialogueLineEvent, DialogueChoicesEvent

DamageNumbersUI
  Floating damage text
  Subscribes to HealthChangedEvent
  Object pooled text components

MinimapUI
  Renders RenderTexture from MinimapSystem
  Player position indicator

InventoryUI
  Grid-based or list-based display
  Drag and drop
  Subscribes to InventoryChangedEvent

ShopUI
  Buy/sell interface
  Reads from CurrencySystem and InventoryComponent
```

---

## Summary — Priority by project

For your MapleStory 2D coop game specifically:

```
HIGH PRIORITY (add before game jam / prototype)
  SkillHotbarSystem
  BuffSystem
  AirDashState
  HitStopSystem
  EquipmentSystem
  DamageNumberSystem

MEDIUM PRIORITY (add during production)
  SummonSystem
  ClimbState
  AttackChainSystem
  ParallaxBackground
  PlatformController
  PartySystem (when multiplayer ready)

LOW PRIORITY (polish / late game)
  WeaponTriangleSystem (if you want weapon variance)
  ProceduralRoomSystem (if adding roguelike mode)
  ReputationSystem (if adding factions)
```
