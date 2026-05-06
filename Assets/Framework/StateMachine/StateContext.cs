using System.Collections.Generic;
using UnityEngine;
using Framework.AI.Alert;
using Framework.AI.Faction;
using Framework.AI.Memory;
using Framework.AI.Perception;
using Framework.AI.Squad;
using Framework.AI.Systems;
using Framework.Abilities;
using Framework.Animation;
using Framework.Commands;
using Framework.Movement;
using Framework.Input;
using Framework.Systems.Health;

namespace Framework.StateMachine
{
    // =========================================
    // StateContext
    // Zero Gameplay imports — pure Framework.
    // The compiler enforces this via Framework.asmdef.
    // =========================================
    public class StateContext
    {
        // =========================================
        // CORE
        // =========================================
        public InputState        Input;
        public ICommandQueue     Commands;
        public IHealth           HealthData;
        public IHealthComponent  HealthComp;
        public Transform         Self;
        public AnimationRequest? AnimationRequest;

        // =========================================
        // MOVEMENT STRATEGY — optional
        // Null = TransformMovementStrategy (default)
        // Set to NavMeshMovementStrategy for pathfinding
        // =========================================
        public IMovementStrategy Movement;

        // =========================================
        // AI LOD
        // =========================================
        public AIExecutionContext Execution  = new AIExecutionContext();
        public ulong              SystemMask = ulong.MaxValue;

        // =========================================
        // MODULAR SUB-CONTEXTS
        // =========================================
        public IPerceptionContext PerceptionContext;
        public IMemoryContext     MemoryContext;
        public AlertContext       AlertContext;
        public ISquadContext      SquadContext;

        // =========================================
        // BACKWARD-COMPAT ACCESSORS
        // =========================================
        public IPerceptionState Perception
        {
            get => PerceptionContext?.State;
            set { if (PerceptionContext != null) PerceptionContext.State = value; }
        }

        public Transform Target
        {
            get => PerceptionContext?.Target;
            set { if (PerceptionContext != null) PerceptionContext.Target = value; }
        }

        public List<Transform> VisibleTargets
        {
            get => PerceptionContext?.VisibleTargets;
            set { if (PerceptionContext != null) PerceptionContext.VisibleTargets = value; }
        }

        public IAIMemory Memory
        {
            get => MemoryContext?.Memory;
            set { if (MemoryContext != null) MemoryContext.Memory = value; }
        }

        public AlertLevel AlertLevel
        {
            get => AlertContext?.Level ?? AlertLevel.Calm;
            set { if (AlertContext != null) AlertContext.Level = value; }
        }

        public float AlertValue
        {
            get => AlertContext?.Value ?? 0f;
            set { if (AlertContext != null) AlertContext.Value = value; }
        }

        // =========================================
        // EMOTION
        // =========================================
        public float Morale      = 1f;
        public float Fear        = 0f;
        public float Suppression;

        // =========================================
        // COMBAT
        // =========================================
        public IAbilitySystem Abilities;
        public bool           WasHit;
        public Vector3        HitDirection;

        // =========================================
        // 2D PHYSICS STATE
        // Written each frame by movement strategy
        // =========================================
        public bool IsGrounded;
        public bool IsOnWall;
        public bool IsCrouching;

        // =========================================
        // FACTION
        // =========================================
        public Team Team;

        // =========================================
        // DIRECTOR / SQUAD DATA
        // Written each frame by AI systems
        // =========================================
        public float         DirectorIntensity = 0f;
        public SquadStrategy SquadStrategy     = SquadStrategy.Idle;
    }
}