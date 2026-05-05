using UnityEngine;

namespace Framework.Events.Events.Gameplay
{
    // =========================================
    // HealthChangedEvent
    // Published via EventBus whenever any agent's
    // health changes. Carries enough context for
    // any subscriber to act on it without needing
    // a direct reference to the HealthComponent.
    //
    // Subscribers:
    //   Audio system  — play hit sound
    //   VFX system    — spawn hit effect
    //   UI system     — update global health bars
    //   Achievement   — track total damage dealt
    //   Screen effects — flash on low health
    //
    // Usage — subscribe:
    //   EventBus.Subscribe<HealthChangedEvent>(OnHealthChanged);
    //
    // Usage — unsubscribe (always do this on destroy):
    //   EventBus.Unsubscribe<HealthChangedEvent>(OnHealthChanged);
    // =========================================
    public struct HealthChangedEvent
    {
        public GameObject Source;      // who changed health
        public int        OldValue;    // health before
        public int        NewValue;    // health after
        public int        MaxValue;    // max health
        public int        Delta;       // change amount (negative = damage, positive = heal)
        public bool       IsDead;      // true if this change caused death
        public Vector3    HitPoint;    // world position of hit (Vector3.zero if healed)

        public HealthChangedEvent(
            GameObject source,
            int        oldValue,
            int        newValue,
            int        maxValue,
            Vector3    hitPoint)
        {
            Source   = source;
            OldValue = oldValue;
            NewValue = newValue;
            MaxValue = maxValue;
            Delta    = newValue - oldValue;
            IsDead   = newValue <= 0;
            HitPoint = hitPoint;
        }
    }
}