using System.Collections.Generic;
using UnityEngine;

using Framework.StateMachine;
using Framework.StateMachine.States;
using Framework.Commands;
using Framework.Input;
using Framework.AI.Systems;

using Gameplay.AI.Perception;
using Gameplay.AI.Group;
using Gameplay.AI.Squad;
using Gameplay.AI.Director;
using Gameplay.AI.Memory;
using Gameplay.AI.Faction;

using Gameplay.Systems.Health;
using Gameplay.Abilities;
using Gameplay.Combat;

namespace Gameplay.AI
{
    public class AIController : MonoBehaviour
    {
        // =========================================
        // CORE STATE
        // =========================================
        private StateMachine _stateMachine;
        private StateContext _context;

        // =========================================
        // REFERENCES
        // =========================================
        [Header("References")]
        public PerceptionSystem perception;
        public HealthComponent healthComponent;
        public Transform playerTransform;
        public Rigidbody rb;

        [Header("AI Settings")]
        public Team team;

        // =========================================
        // GLOBAL REGISTRY
        // =========================================
        public static Dictionary<Transform, StateContext> Registry = new();

        // =========================================
        // UNITY LIFECYCLE
        // =========================================
        private void Awake()
        {
            CreateContext();
            RegisterSelf();
            BindSystems();
            BindEvents();
            CreateStateMachine();
            RegisterDirector();
        }

        private void Update()
        {
            TickPerception();
            TickAISystems();
            TickStateMachine();
            ResetFrameFlags();
            ExecuteCommands();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        private void OnDeath()
        {
            Cleanup();
        }

        // =========================================
        // INITIALIZATION
        // =========================================
        private void CreateContext()
        {
            _context = new StateContext
            {
                Input = new InputState(),
                Commands = new CommandQueue(),

                HealthData = healthComponent.GetHealth(),
                HealthComp = healthComponent,

                Self = transform,

                PerceptionContext = new Gameplay.AI.Perception.PerceptionContext
                {
                    State = new PerceptionState(),
                    Target = playerTransform,
                    VisibleTargets = new List<Transform>()
                },

                Memory = new AIMemory(),
                Team = team
            };
        }

        // =========================================
        // REGISTRATION LAYER (ONLY THIS LEFT)
        // =========================================
        private void RegisterSelf()
        {
            Registry[transform] = _context;
        }

        private void BindSystems()
        {
            if (perception != null)
            {
                perception.context = _context;
                perception.target = playerTransform;
            }

            // NOTE:
            // These systems are now assumed to be handled by AISystemManager
            // We only ensure membership here — NOT ownership.

            AIGroupManager.Instance?.Register(_context);
            SquadSystem.Instance?.Register(_context);

            var stagger = GetComponent<StaggerSystem>();
            if (stagger != null && rb != null)
                stagger.Register(healthComponent, rb);

            // abilities remain agent-owned (valid responsibility)
            _context.Abilities = new AbilitySystem();
            _context.Abilities.Register(
                Gameplay.Abilities.Definitions.BasicAttackAbility.Create()
            );
        }

        private void BindEvents()
        {
            if (healthComponent == null)
                return;

            healthComponent.OnHit += OnHit;
            healthComponent.OnDeath += OnDeath;
        }

        private void CreateStateMachine()
        {
            _stateMachine = new StateMachine(_context);
            _stateMachine.ChangeState(new CombatState());
        }

        private void RegisterDirector()
        {
            if (AIDirector.Instance != null)
            {
                AIDirector.Instance.State.ActiveEnemies++;
            }
        }

        // =========================================
        // UPDATE LOOP
        // =========================================
        private void TickPerception()
        {
            perception?.Tick();
        }

        private void TickAISystems()
        {
            AISystemManager.UpdateAll(_context);
        }

        private void TickStateMachine()
        {
            _stateMachine?.Update();
        }

        private void ResetFrameFlags()
        {
            _context.WasHit = false;
        }

        private void ExecuteCommands()
        {
            _context.Commands?.ExecuteAll();
        }

        // =========================================
        // CLEANUP
        // =========================================
        private void Cleanup()
        {
            Registry.Remove(transform);

            AIGroupManager.Instance?.Unregister(_context);
            SquadSystem.Instance?.Unregister(_context);

            if (AIDirector.Instance != null)
            {
                AIDirector.Instance.State.ActiveEnemies--;
            }

            if (healthComponent != null)
            {
                healthComponent.OnHit -= OnHit;
                healthComponent.OnDeath -= OnDeath;
            }

            if (_context != null)
                _context.Commands = null;

            enabled = false;
        }

        // =========================================
        // EVENTS
        // =========================================
        private void OnHit(Vector3 hitPoint)
        {
            _context.WasHit = true;
            _context.HitDirection =
                (transform.position - hitPoint).normalized;
        }
    }
}