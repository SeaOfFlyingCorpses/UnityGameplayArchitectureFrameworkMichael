using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        // =========================================
        // Fired before Clear() wipes the registry.
        // Systems can subscribe to do their own
        // teardown before references go stale.
        // =========================================
        public static event Action OnClear;

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
        // HAS
        // =========================================
        public static bool Has<T>() where T : class
        {
            return _services.ContainsKey(typeof(T));
        }

        // =========================================
        // CLEAR — call before scene load / on teardown
        // =========================================
        public static void Clear()
        {
            OnClear?.Invoke();
            _services.Clear();
        }

        // =========================================
        // AUTO-CLEAR on domain reload (enter play mode)
        // Prevents stale services from a previous
        // play session bleeding into the next one.
        // =========================================
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnLoad()
        {
            OnClear?.Invoke();
            OnClear = null;
            _services.Clear();
        }
    }
}