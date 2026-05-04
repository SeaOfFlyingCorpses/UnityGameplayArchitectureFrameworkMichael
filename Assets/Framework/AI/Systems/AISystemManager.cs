using System.Collections.Generic;
using Framework.StateMachine;
 
namespace Framework.AI.Systems
{
    // =========================================
    // AISystemManager — INSTANCED (not static)
    // Each agent owns one instance and registers
    // exactly the systems it needs.
    // All existing logic is preserved:
    //   - category ordering
    //   - LOD filter
    //   - system mask filter
    //   - ShouldRun cache
    //   - deferred removal during update
    // =========================================
    public class AISystemManager
    {
        private readonly List<IAISystem> _systems = new();
        private readonly Dictionary<AISystemCategory, List<IAISystem>> _groups = new();
        private bool _isUpdating;
        private readonly List<IAISystem> _pendingRemove = new();
        private readonly Dictionary<IAISystem, bool> _runCache = new();
 
        // =========================================
        // REGISTER
        // =========================================
        public void Register(IAISystem system)
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
 
        // =========================================
        // UPDATE PIPELINE
        // =========================================
        public void UpdateAll(StateContext context)
        {
            if (context == null)
                return;
 
            _isUpdating = true;
 
            // Fixed category execution order — unchanged
            UpdateCategory(AISystemCategory.Perception, context);
            UpdateCategory(AISystemCategory.Memory,     context);
            UpdateCategory(AISystemCategory.Emotion,    context);
            UpdateCategory(AISystemCategory.Squad,      context);
            UpdateCategory(AISystemCategory.Combat,     context);
            UpdateCategory(AISystemCategory.Utility,    context);
 
            _isUpdating = false;
 
            FlushRemovals();
        }
 
        // =========================================
        // CATEGORY EXECUTION (LOD + CONDITION LAYER)
        // =========================================
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
 
        // =========================================
        // ShouldRun CACHE
        // =========================================
        private bool ShouldExecute(IAISystem system, StateContext context)
        {
            if (_runCache.TryGetValue(system, out var cached))
                return cached;
 
            bool result = system.ShouldRun(context);
            _runCache[system] = result;
 
            return result;
        }
 
        // =========================================
        // CLEAR ALL (useful on agent death / reset)
        // =========================================
        public void Clear()
        {
            _systems.Clear();
            _groups.Clear();
            _runCache.Clear();
            _pendingRemove.Clear();
        }
    }
}