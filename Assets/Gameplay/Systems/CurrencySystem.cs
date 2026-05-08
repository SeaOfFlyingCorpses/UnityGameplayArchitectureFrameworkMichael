using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using Framework.Events;
using Framework.Events.Events.Gameplay;
using Framework.Persistence;

namespace Gameplay.Systems
{
    // =========================================
    // CurrencySystem
    // Manages multiple currency types.
    // Gold, gems, tokens — all in one system.
    //
    // Saves/loads automatically via ISaveable.
    // Place on _GameSystems.
    //
    // Usage:
    //   var cs = ServiceLocator.Get<CurrencySystem>();
    //   cs.Add("gold", 100);
    //   cs.Spend("gold", 50);
    //   cs.Get("gold");
    // =========================================
    public class CurrencySystem : MonoBehaviour, ISaveable
    {
        [Header("Starting Currency")]
        public List<CurrencyEntry> startingCurrency = new()
        {
            new CurrencyEntry { id = "gold",  amount = 0 },
            new CurrencyEntry { id = "gems",  amount = 0 },
        };

        private readonly Dictionary<string, int> _wallet = new();

        public string SaveId => "CurrencySystem";

        private void Awake()
        {
            ServiceLocator.Register<CurrencySystem>(this);

            foreach (var entry in startingCurrency)
                _wallet[entry.id] = entry.amount;
        }

        private void Start()
        {
            ServiceLocator.Get<SaveSystem>()?.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<CurrencySystem>();
            ServiceLocator.Get<SaveSystem>()?.Unregister(this);
        }

        // =========================================
        // API
        // =========================================
        public int Get(string currencyId)
        {
            _wallet.TryGetValue(currencyId, out var amount);
            return amount;
        }

        public void Add(string currencyId, int amount)
        {
            if (amount <= 0) return;

            _wallet.TryGetValue(currencyId, out var current);
            _wallet[currencyId] = current + amount;

            EventBus.Publish(new CurrencyChangedEvent(
                currencyId, amount, _wallet[currencyId]));
        }

        public bool Spend(string currencyId, int amount)
        {
            if (!CanAfford(currencyId, amount)) return false;

            _wallet[currencyId] -= amount;

            EventBus.Publish(new CurrencyChangedEvent(
                currencyId, -amount, _wallet[currencyId]));

            return true;
        }

        public bool CanAfford(string currencyId, int amount)
        {
            _wallet.TryGetValue(currencyId, out var current);
            return current >= amount;
        }

        public void Set(string currencyId, int amount)
        {
            _wallet[currencyId] = Mathf.Max(0, amount);

            EventBus.Publish(new CurrencyChangedEvent(
                currencyId, 0, _wallet[currencyId]));
        }

        // =========================================
        // ISaveable
        // =========================================
        public object CaptureState()
        {
            var data = new SaveData();
            foreach (var kvp in _wallet)
                data.wallet.Add(new CurrencyEntry
                {
                    id     = kvp.Key,
                    amount = kvp.Value
                });
            return data;
        }

        public void RestoreState(object state)
        {
            if (state is not SaveData data) return;
            _wallet.Clear();
            foreach (var entry in data.wallet)
                _wallet[entry.id] = entry.amount;
        }

        [System.Serializable]
        public class SaveData
        {
            public List<CurrencyEntry> wallet = new();
        }

        [System.Serializable]
        public class CurrencyEntry
        {
            public string id;
            public int    amount;
        }
    }
}
