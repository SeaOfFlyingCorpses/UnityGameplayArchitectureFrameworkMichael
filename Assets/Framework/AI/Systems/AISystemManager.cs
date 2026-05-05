using System.Collections.Generic;
using Framework.StateMachine;

namespace Framework.AI.Systems
{
    public class AISystemManager
    {
        private readonly List<IAISystem> _systems = new();
        private readonly Dictionary<AISystemCategory, List<IAISystem>> _groups = new();
        private bool _isUpdating;
        private readonly List<IAISystem> _pendingRemove = new();

        // =========================================
        // ShouldRun CACHE
        // Cleared every frame so conditions can
        // change between frames correctly.
        // =========================================
        private readonly Dictionary<IAISystem, bool> _runCache = new();

        public void Register(IAISystem system)
        {
            if (system == null || _systems.Contains(system))
                return;

            _systems.Add(system);

            if (!_groups.TryGetValue(system.Category, out var list))
            {
                list = new List<IAISystem>();
                _groups[system.Category] = list;
            }

            list.Add(system);
        }

        public void Unregister(IAISystem system)
        {
            if (system == null)
                return;

            if (_isUpdating)
            {
                _pendingRemove.Add(system);
                return;
            }

            Remove(system);
        }

        private void Remove(IAISystem system)
        {
            _systems.Remove(system);

            if (_groups.TryGetValue(system.Category, out var list))
                list.Remove(system);

            _runCache.Remove(system);
        }

        private void FlushRemovals()
        {
            if (_pendingRemove.Count == 0)
                return;

            for (int i = 0; i < _pendingRemove.Count; i++)
                Remove(_pendingRemove[i]);

            _pendingRemove.Clear();
        }

        public void UpdateAll(StateContext context)
        {
            if (context == null)
                return;

            // =========================================
            // FLUSH ShouldRun CACHE EACH FRAME
            // so conditions are re-evaluated every update
            // =========================================
            _runCache.Clear();

            _isUpdating = true;

            UpdateCategory(AISystemCategory.Perception, context);
            UpdateCategory(AISystemCategory.Memory,     context);
            UpdateCategory(AISystemCategory.Emotion,    context);
            UpdateCategory(AISystemCategory.Squad,      context);
            UpdateCategory(AISystemCategory.Combat,     context);
            UpdateCategory(AISystemCategory.Utility,    context);

            _isUpdating = false;

            FlushRemovals();
        }

        private void UpdateCategory(AISystemCategory category, StateContext context)
        {
            if (!_groups.TryGetValue(category, out var list))
                return;

            for (int i = 0; i < list.Count; i++)
            {
                var system = list[i];

                if (system == null)
                    continue;

                // LOD FILTER
                if (context.Execution.LOD < system.MinLOD)
                    continue;

                // SYSTEM MASK FILTER
                if ((context.SystemMask & system.Mask) == 0)
                    continue;

                // CONDITIONAL EXECUTION
                if (!ShouldExecute(system, context))
                    continue;

                system.Update(context);
            }
        }

        private bool ShouldExecute(IAISystem system, StateContext context)
        {
            if (_runCache.TryGetValue(system, out var cached))
                return cached;

            bool result = system.ShouldRun(context);
            _runCache[system] = result;

            return result;
        }

        public void Clear()
        {
            _systems.Clear();
            _groups.Clear();
            _runCache.Clear();
            _pendingRemove.Clear();
        }
    }
}