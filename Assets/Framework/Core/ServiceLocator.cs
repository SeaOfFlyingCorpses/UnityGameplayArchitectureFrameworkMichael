using System;
using System.Collections.Generic;

namespace Framework.Core
{
    // =========================================
    // ServiceLocator
    // A type-keyed registry for global services.
    // Replaces all "public static Instance" patterns.
    //
    // REGISTER (from MonoBehaviour.Awake):
    //   ServiceLocator.Register<ISquadSystem>(this);
    //
    // RETRIEVE (from anywhere):
    //   var squad = ServiceLocator.Get<ISquadSystem>();
    //   if (squad != null) squad.Register(_context);
    //
    // UNREGISTER (from MonoBehaviour.OnDestroy):
    //   ServiceLocator.Unregister<ISquadSystem>();
    //
    // Rules:
    //   - One instance per type at a time.
    //   - Get<T>() returns null if not registered — always null-check.
    //   - Register/Unregister are safe to call from any thread order.
    //   - No exceptions thrown — silent null returns keep agents safe.
    // =========================================
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        // =========================================
        // REGISTER
        // =========================================
        public static void Register<T>(T service) where T : class
        {
            if (service == null)
                return;

            _services[typeof(T)] = service;
        }

        // =========================================
        // UNREGISTER
        // =========================================
        public static void Unregister<T>() where T : class
        {
            _services.Remove(typeof(T));
        }

        // =========================================
        // GET — returns null if not registered
        // =========================================
        public static T Get<T>() where T : class
        {
            _services.TryGetValue(typeof(T), out var service);
            return service as T;
        }

        // =========================================
        // HAS — check before get when needed
        // =========================================
        public static bool Has<T>() where T : class
        {
            return _services.ContainsKey(typeof(T));
        }

        // =========================================
        // CLEAR — call on scene unload / teardown
        // =========================================
        public static void Clear()
        {
            _services.Clear();
        }
    }
}