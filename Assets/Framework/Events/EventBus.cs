using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Events
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> _events = new();

        // =========================================
        // SUBSCRIBE
        // =========================================
        public static void Subscribe<T>(Action<T> listener)
        {
            if (_events.TryGetValue(typeof(T), out var existing))
                _events[typeof(T)] = Delegate.Combine(existing, listener);
            else
                _events[typeof(T)] = listener;
        }

        // =========================================
        // UNSUBSCRIBE
        // =========================================
        public static void Unsubscribe<T>(Action<T> listener)
        {
            if (!_events.TryGetValue(typeof(T), out var existing))
                return;

            var result = Delegate.Remove(existing, listener);

            if (result == null)
                _events.Remove(typeof(T));
            else
                _events[typeof(T)] = result;
        }

        // =========================================
        // PUBLISH
        // =========================================
        public static void Publish<T>(T message)
        {
            if (_events.TryGetValue(typeof(T), out var del))
                (del as Action<T>)?.Invoke(message);
        }

        // =========================================
        // CLEAR — call before scene unload
        // Removes all listeners to prevent stale
        // references across scene boundaries
        // =========================================
        public static void Clear()
        {
            _events.Clear();
        }

        // =========================================
        // AUTO-CLEAR on domain reload
        // Prevents stale listeners from a previous
        // play session bleeding into the next one
        // =========================================
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnLoad()
        {
            _events.Clear();
        }
    }
}