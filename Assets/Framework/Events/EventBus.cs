using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Events
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> _events = new();

        public static void Subscribe<T>(Action<T> listener)
        {
            if (_events.TryGetValue(typeof(T), out var existing))
                _events[typeof(T)] = Delegate.Combine(existing, listener);
            else
                _events[typeof(T)] = listener;
        }

        public static void Unsubscribe<T>(Action<T> listener)
        {
            if (_events.TryGetValue(typeof(T), out var existing))
            {
                var result = Delegate.Remove(existing, listener);

                if (result == null)
                    _events.Remove(typeof(T));
                else
                    _events[typeof(T)] = result;
            }
        }

        public static void Publish<T>(T message)
        {
            if (_events.TryGetValue(typeof(T), out var del))
                (del as Action<T>)?.Invoke(message);
            
        }
    }
}
