using System.Collections.Generic;
using UnityEngine;

using Framework.StateMachine;
using Framework.StateMachine.States;
using Framework.Commands;
using Framework.Input;
using Framework.AI.Systems;
using Framework.Core;

using Gameplay.AI.Perception;
using Gameplay.AI.Group;
using Gameplay.AI.Squad;
using Gameplay.AI.Director;
using Gameplay.AI.Memory;
using Gameplay.AI.Faction;
using Gameplay.AI.States;

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
        private StateMachine    _stateMachine;
        private StateContext    _context;
        private AISystemManager _aiSystems;

        // =========================================
        // REFERENCES
        // =========================================
        [Header("References")]
        public PerceptionSystem perception;
        public HealthComponent  healthComponent;
        public Transform        playerTransform;
        public Rigidbody        rb;

        [Header("AI Settings")]
        public Team team;

        // =========================================
        // STATE FACTORY
        // =========================================
        protected IAIStateFactory StateFactory;

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
                Input    = new InputState(),
                Commands = new CommandQueue(),

                HealthData = healthComponent.GetHealth(),
                HealthComp = healthComponent,

                Self = transform,

                PerceptionContext = new Gameplay.AI.Perception.PerceptionContext
                {
                    State          = new PerceptionState(),
                    Target         = playerTransform,
                    VisibleTargets = new List<Transform>()
                },

                MemoryContext = new Gameplay.AI.Memory.MemoryContext(),
                AlertContext  = new Framework.AI.Alert.AlertContext(),
                SquadContext  = new Gameplay.AI.Squad.SquadContext(),

                Team = team
            };
        }

        private void RegisterSelf()
        {
            // Use AIAgentRegistry service instead of static Dictionary
            ServiceLocator.Get<AIAgentRegistry>()?.Register(transform, _context);
        }

        private void BindSystems()
        {
            _aiSystems = new AISystemManager();
            AISystemsBootstrap.RegisterDefaults(_aiSystems);

            var groupManager = ServiceLocator.Get<AIGroupManager>();
            if (groupManager != null)
            {
                _aiSystems.Register(new AIGroupSystem(groupManager));
                groupManager.Register(_context);
            }

            ServiceLocator.Get<SquadSystem>()?.Register(_context);

            var stagger = GetComponent<StaggerSystem>();
            if (stagger != null && rb != null)
                stagger.Register(healthComponent, rb);

            var death = GetComponent<DeathSystem>();
            if (death != null)
                death.Register(healthComponent, rb);

            _context.Abilities = new AbilitySystem();
            _context.Abilities.Register(
                Gameplay.Abilities.Definitions.BasicAttackAbility.Create()
            );

            if (perception != null)
            {
                perception.context = _context;
                perception.target  = playerTransform;
            }
        }

        private void BindEvents()
        {
            if (healthComponent == null)
                return;

            healthComponent.OnHit   += OnHit;
            healthComponent.OnDeath += OnDeath;
        }

        private void CreateStateMachine()
        {
            if (StateFactory == null)
                StateFactory = CreateFactory();

            _stateMachine = new StateMachine(_context);
            _stateMachine.ChangeState(StateFactory.Build(_context));
        }

        protected virtual IAIStateFactory CreateFactory()
        {
            var factory = GetComponent<CombatAIStateFactory>();
            if (factory != null)
                return factory;

            return new DefaultCombatFactory();
        }

        private void RegisterDirector()
        {
            var director = ServiceLocator.Get<AIDirector>();
            if (director != null)
                director.State.ActiveEnemies++;
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
            _aiSystems?.UpdateAll(_context);
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
            // Unregister from AIAgentRegistry instead of static Dictionary
            ServiceLocator.Get<AIAgentRegistry>()?.Unregister(transform);

            ServiceLocator.Get<AIGroupManager>()?.Unregister(_context);
            ServiceLocator.Get<SquadSystem>()?.Unregister(_context);

            var director = ServiceLocator.Get<AIDirector>();
            if (director != null)
                director.State.ActiveEnemies--;

            if (healthComponent != null)
            {
                healthComponent.OnHit   -= OnHit;
                healthComponent.OnDeath -= OnDeath;
            }

            _aiSystems?.Clear();
            _aiSystems = null;

            if (_context != null)
                _context.Commands = null;

            enabled = false;
        }

        // =========================================
        // EVENTS
        // =========================================
        private void OnHit(Vector3 hitPoint)
        {
            _context.WasHit       = true;
            _context.HitDirection = (transform.position - hitPoint).normalized;
        }

        // =========================================
        // DEFAULT COMBAT FACTORY
        // Pure C# fallback when no
        // CombatAIStateFactory component is present
        // =========================================
        private class DefaultCombatFactory : IAIStateFactory
        {
            public IState Build(StateContext context)
            {
                var config  = new CombatStateConfig();
                var combat  = new CombatState(config);
                var idle    = new IdleState();
                var search  = new SearchState(idle);
                var chase   = new ChaseState(combat, search);
                var stagger = new StaggerState(combat);

                return combat;
            }
        }
    }
}