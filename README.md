# Gameplay Architecture Framework

A modular, production-ready gameplay framework for Unity 6.

Built with two assembly definitions (`Framework` + `Gameplay`) enforcing zero coupling between core systems and game-specific implementations.

## Quick Start

1. Import `Framework.unitypackage` into any Unity 6 project
2. Add `_GameSystems` GameObject to your scene
3. Add components: `GameBootstrap`, `SquadSystem`, `AIAgentRegistry`, `SceneLifecycleManager`
4. Done — all systems available via `ServiceLocator`

## Documentation

See [FRAMEWORK_DOCUMENTATION.md](./FRAMEWORK_DOCUMENTATION.md) for full API reference.

## Assembly Structure

```
Framework.asmdef    — pure C#, zero Unity or Gameplay imports
Gameplay.asmdef     — references Framework, contains all implementations
Tests.asmdef        — editor only, references both
```

## Systems

| Category | Systems |
|---|---|
| Core | ServiceLocator, EventBus, CommandQueue, ObjectPool, SaveSystem |
| AI | StateMachine, BehaviourTree, SquadSystem, FormationSystem, PerceptionSystem |
| Health | 9 health types, StatusEffects, DamageTypes, WeakPoints |
| Progression | LevelSystem, StatSystem, ClassSystem, XPSource |
| Gameplay | Inventory, Quest, Dialogue, Interaction, Combo, Loot, Stage, Respawn, Currency |
| Camera | 9 3D modes + 4 2D modes, all swappable at runtime |
| Audio | IAudioSystem, UnityAudio + Wwise implementations, MusicSystem |
| Network | Abstraction layer ready for Mirror/NGO/Photon |
| 2D | Platformer movement, 2D AI, Jump/Fall/Dash/WallSlide states |

## License

MIT
