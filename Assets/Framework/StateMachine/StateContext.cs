using System.Collections.Generic;
using UnityEngine;
using Framework.AI.Alert;
using Framework.Input;
using Framework.Commands;
using Framework.Animation;
using Gameplay.Abilities;
using Gameplay.AI.Faction;
using Gameplay.AI.Memory;
using Gameplay.AI.Perception;
using Gameplay.AI.Squad;
using Gameplay.Systems.Health;
using Framework.AI.Systems;

namespace Framework.StateMachine
{
    public class StateContext
    {
        // =========================================
        // CORE — always present, never optional
        // =========================================
        public InputState    Input;
        public ICommandQueue Commands;   // interface — not concrete CommandQueue

        public IHealth         HealthData;   // interface — swap any implementation
        public HealthComponent HealthComp;
        public Transform       Self;

        public AnimationRequest? AnimationRequest;

        // =========================================
        // AI LOD + SYSTEM CONTROL LAYER
        // =========================================
        public AIExecutionContext Execution  = new AIExecutionContext();
        public ulong              SystemMask = ulong.MaxValue;

        // =========================================
        // MODULAR SUB-CONTEXTS — all nullable / opt-in
        // =========================================
        public PerceptionContext PerceptionContext;
        public MemoryContext     MemoryContext;
        public AlertContext      AlertContext;
        public SquadContext      SquadContext;

        // =========================================
        // BACKWARD-COMPATIBILITY ACCESSORS
        // =========================================
        public PerceptionState Perception
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

        public AIMemory Memory
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
        public AbilitySystem Abilities;
        public bool          WasHit;
        public Vector3       HitDirection;

        // =========================================
        // FACTION
        // =========================================
        public Team Team;

        // =========================================
        // DIRECTOR DATA
        // Written each frame by DirectorSystem
        // =========================================
        public float DirectorIntensity = 0f;

        // =========================================
        // SQUAD STRATEGY
        // Written each frame by SquadAISystem
        // =========================================
        public SquadStrategy SquadStrategy = SquadStrategy.Idle;
    }
}