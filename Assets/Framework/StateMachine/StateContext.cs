using System.Collections.Generic;
using UnityEngine;
using Framework.AI.Alert;
using Framework.Input;
using Framework.Commands;
using Framework.Animation;
using Gameplay.Abilities;
using Gameplay.AI.Faction;
using Gameplay.AI.Formation;
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
        // CORE INPUT / EXECUTION
        // =========================================
        public InputState Input;
        public CommandQueue Commands;

        public Health HealthData;
        public HealthComponent HealthComp;
        public Transform Self;

        public AnimationRequest? AnimationRequest;

        // =========================================
        // CONTEXT SYSTEMS
        // =========================================
        public PerceptionContext PerceptionContext = new();
        public MemoryContext MemoryContext = new();
        public AlertContext AlertContext = new();
        public SquadContext SquadContext = new();

        // =========================================
        // BACKWARD COMPATIBILITY
        // =========================================
        public PerceptionState Perception
        {
            get => PerceptionContext.State;
            set => PerceptionContext.State = value;
        }

        public Transform Target
        {
            get => PerceptionContext.Target;
            set => PerceptionContext.Target = value;
        }

        public List<Transform> VisibleTargets
        {
            get => PerceptionContext.VisibleTargets;
            set => PerceptionContext.VisibleTargets = value;
        }

        public AIMemory Memory
        {
            get => MemoryContext.Memory;
            set => MemoryContext.Memory = value;
        }

        public AlertLevel AlertLevel
        {
            get => AlertContext.Level;
            set => AlertContext.Level = value;
        }

        public float AlertValue
        {
            get => AlertContext.Value;
            set => AlertContext.Value = value;
        }

        // =========================================
        // EMOTION
        // =========================================
        public float Morale = 1f;
        public float Fear = 0f;
        public float Suppression;

        // =========================================
        // COMBAT
        // =========================================
        public AbilitySystem Abilities;
        public bool WasHit;
        public Vector3 HitDirection;

        // =========================================
        // FACTION
        // =========================================
        public Team Team;

        // =========================================
        // STEP 5: AI LOD + SYSTEM CONTROL LAYER
        // =========================================
        public AIExecutionContext Execution = new AIExecutionContext();

        // Optional future: bitmask system activation (not used yet)
        public ulong SystemMask = ulong.MaxValue;
    }
}