using System.Collections.Generic;
using Framework.StateMachine;

namespace Framework.AI.Systems
{
    public static class AISystemManager
    {
        private static readonly List<IAISystem> _systems = new();
        private static readonly Dictionary<AISystemCategory, List<IAISystem>> _groups = new();
        private static bool _isUpdating;
        private static readonly List<IAISystem> _pendingRemove = new();
        private static readonly Dictionary<IAISystem, bool> _runCache = new();

        // =========================================
        // REGISTER
        // =========================================
        public static void Register(IAISystem system)
        {
            if (system == null)
                return;

            if (_systems.Contains(system))
                return;

            _systems.Add(system);

            if (!_groups.TryGetValue(system.Category, out var list))
            {
                list = new List<IAISystem>();
                _groups[system.Category] = list;
            }

            list.Add(system);
        }

        // =========================================
        // UNREGISTER
        // =========================================
        public static void Unregister(IAISystem system)
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

        private static void Remove(IAISystem system)
        {
            _systems.Remove(system);

            if (_groups.TryGetValue(system.Category, out var list))
                list.Remove(system);

            _runCache.Remove(system);
        }

        private static void FlushRemovals()
        {
            if (_pendingRemove.Count == 0)
                return;

            for (int i = 0; i < _pendingRemove.Count; i++)
                Remove(_pendingRemove[i]);

            _pendingRemove.Clear();
        }

        // =========================================
        // UPDATE PIPELINE
        // =========================================
        public static void UpdateAll(StateContext context)
        {
            if (context == null)
                return;

            _isUpdating = true;

            // Process all AI systems by category
            UpdateCategory(AISystemCategory.Perception, context);
            UpdateCategory(AISystemCategory.Memory, context);
            UpdateCategory(AISystemCategory.Emotion, context);
            UpdateCategory(AISystemCategory.Squad, context);
            UpdateCategory(AISystemCategory.Combat, context);
            UpdateCategory(AISystemCategory.Utility, context);

            _isUpdating = false;

            FlushRemovals();
        }

        // =========================================
        // CATEGORY EXECUTION (LOD + CONDITION LAYER)
        // =========================================
        private static void UpdateCategory(AISystemCategory category, StateContext context)
        {
            if (!_groups.TryGetValue(category, out var list))
                return;

            for (int i = 0; i < list.Count; i++)
            {
                var system = list[i];

                if (system == null)
                    continue;

                // =========================================
                // STEP 5: LOD FILTER (USING Execution.LOD)
                // =========================================
                if (context.Execution.LOD < system.MinLOD)
                    continue;

                // =========================================
                // STEP 6: SYSTEM MASK FILTER (optional)
                // =========================================
                if ((context.SystemMask & system.Mask) == 0)
                    continue;

                // =========================================
                // STEP 7: CONDITIONAL EXECUTION (USING ShouldRun)
                // =========================================
                if (!ShouldExecute(system, context))
                    continue;

                // =========================================
                // STEP 8: SYSTEM UPDATE (FINAL)
                // =========================================
                system.Update(context);
            }
        }

        // =========================================
        // SYSTEM EXECUTION CHECK (ShouldRun CACHE)
        // =========================================
        private static bool ShouldExecute(IAISystem system, StateContext context)
        {
            if (_runCache.TryGetValue(system, out var cached))
                return cached;

            bool result = system.ShouldRun(context);
            _runCache[system] = result;

            return result;
        }
    }
}