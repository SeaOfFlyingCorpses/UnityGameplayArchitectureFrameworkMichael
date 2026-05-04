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
        public Rigidbody        rb;

        [Header("AI Settings")]
        public Team team;

        // =========================================
        // TARGET SETTINGS
        // Set enemyLayer to the layer your enemies
        // are on. Allies will auto-target enemies,
        // enemies will auto-target the player.
        // =========================================
        [Header("Targeting")]
        public Transform playerTransform;
        public LayerMask enemyLayer;

        // =========================================
        // STATE FACTORY
        // =========================================
        protected IAIStateFactory StateFactory;

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
            // =========================================
            // TARGET SELECTION
            // Enemies target the player directly.
            // Allies scan for the nearest enemy
            // using the enemyLayer mask.
            // =========================================
            Transform initialTarget = ResolveInitialTarget();

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
                    Target         = initialTarget,
                    VisibleTargets = new List<Transform>()
                },

                MemoryContext = new Gameplay.AI.Memory.MemoryContext(),
                AlertContext  = new Framework.AI.Alert.AlertContext(),
                SquadContext  = new Gameplay.AI.Squad.SquadContext(),

                Team = team
            };
        }

        private Transform ResolveInitialTarget()
        {
            // Enemies always target the player
            if (team == Team.Enemy)
                return playerTransform;

            // Allies scan for the nearest enemy on the enemy layer
            if (team == Team.Ally && enemyLayer != 0)
            {
                var hits = Physics.OverlapSphere(transform.position, 100f, enemyLayer);

                float     bestDist   = float.MaxValue;
                Transform bestTarget = null;

                foreach (var hit in hits)
                {
                    float dist = Vector3.Distance(transform.position, hit.transform.position);
                    if (dist < bestDist)
                    {
                        bestDist   = dist;
                        bestTarget = hit.transform;
                    }
                }

                // Fall back to player if no enemies found yet
                return bestTarget != null ? bestTarget : playerTransform;
            }

            return playerTransform;
        }

        private void RegisterSelf()
        {
            Registry[transform] = _context;
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

                // Perception target matches initial target
                perception.target = _context.PerceptionContext.Target;
            }
        }

        private void BindEvents()
        {
            if (healthComponent == null)
                return;

            healthComponent.OnHit   += OnHit;
            healthComponent.OnDeath += OnDeath;
        }

        // =========================================
        // STATE MACHINE
        // =========================================
        private void CreateStateMachine()
        {
            if (StateFactory == null)
                StateFactory = CreateFactory();

            _stateMachine = new StateMachine(_context);
            _stateMachine.ChangeState(StateFactory.Build(_context));
        }

        protected virtual IAIStateFactory CreateFactory()
        {
            // Use component on this GameObject if present —
            // lets you tune values in the Inspector per agent
            var factory = GetComponent<CombatAIStateFactory>();
            if (factory != null)
                return factory;

            // Fallback — CombatAIStateFactory is a MonoBehaviour so
            // we cannot use new. Return a default inline factory instead.
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
            Registry.Remove(transform);

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
        // Pure C# fallback — used when no
        // CombatAIStateFactory component is present.
        // Add CombatAIStateFactory as a component
        // to get Inspector-tunable values instead.
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