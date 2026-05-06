using System.Collections.Generic;
using UnityEngine;
using Framework.Audio;
using Framework.Core;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Gameplay.Audio
{
    // =========================================
    // AudioEventListener
    // Bridges EventBus events to IAudioSystem.
    // Fully configurable via Inspector —
    // no hardcoded sound ids anywhere.
    //
    // Add as many mappings as your game needs.
    // Leave any section empty — zero cost.
    //
    // Place on _GameSystems.
    // =========================================
    public class AudioEventListener : MonoBehaviour
    {
        // =========================================
        // HEALTH SOUNDS
        // =========================================
        [System.Serializable]
        public class HealthSoundEntry
        {
            [Tooltip("Leave empty to skip")]
            public string onDamageSoundId   = "";
            public string onDeathSoundId    = "";
            public string onHealSoundId     = "";
            public string onLowHealthId     = "";

            [Tooltip("HP ratio below which OnLowHealth fires (0 = disabled)")]
            [Range(0f, 1f)]
            public float  lowHealthThreshold = 0.25f;
        }

        // =========================================
        // PROJECTILE SOUNDS
        // =========================================
        [System.Serializable]
        public class ProjectileSoundEntry
        {
            public string onHitSoundId    = "";
            public string onMissSoundId   = "";
        }

        // =========================================
        // GAME STATE SOUNDS
        // =========================================
        [System.Serializable]
        public class GameStateSoundEntry
        {
            public string onPauseSoundId  = "";
            public string onResumeSoundId = "";
            public string onSaveSoundId   = "";
            public string onLoadSoundId   = "";
        }

        // =========================================
        // SCENE SOUNDS
        // =========================================
        [System.Serializable]
        public class SceneSoundEntry
        {
            public string onSceneLoadStartId = "";
        }

        // =========================================
        // INVENTORY SOUNDS
        // =========================================
        [System.Serializable]
        public class InventorySoundEntry
        {
            public string onItemAddedId   = "";
            public string onItemRemovedId = "";
        }

        // =========================================
        // CUSTOM EVENT SOUNDS
        // Map any string event id to a sound.
        // For game-specific events that don't fit
        // the categories above.
        // =========================================
        [System.Serializable]
        public class CustomSoundEntry
        {
            [Tooltip("Arbitrary label for your reference")]
            public string label      = "";

            [Tooltip("The audio event id to play")]
            public string soundId    = "";

            [Tooltip("Position offset from origin")]
            public Vector3 positionOffset = Vector3.zero;
        }

        // =========================================
        // INSPECTOR FIELDS
        // =========================================
        [Header("Health Sounds")]
        public HealthSoundEntry    healthSounds    = new HealthSoundEntry();

        [Header("Projectile Sounds")]
        public ProjectileSoundEntry projectileSounds = new ProjectileSoundEntry();

        [Header("Game State Sounds")]
        public GameStateSoundEntry  gameStateSounds  = new GameStateSoundEntry();

        [Header("Scene Sounds")]
        public SceneSoundEntry      sceneSounds      = new SceneSoundEntry();

        [Header("Inventory Sounds")]
        public InventorySoundEntry  inventorySounds  = new InventorySoundEntry();

        [Header("Custom Sounds")]
        [Tooltip("Add any number of custom event-to-sound mappings")]
        public CustomSoundEntry[]   customSounds;

        private IAudioSystem _audio;

        // =========================================
        // LIFECYCLE
        // =========================================
        private void Start()
        {
            _audio = ServiceLocator.Get<IAudioSystem>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<HealthChangedEvent>(OnHealthChanged);
            EventBus.Subscribe<GamePausedEvent>(OnGamePaused);
            EventBus.Subscribe<ProjectileHitEvent>(OnProjectileHit);
            EventBus.Subscribe<GameSavedEvent>(OnGameSaved);
            EventBus.Subscribe<SceneLoadingEvent>(OnSceneLoading);
            EventBus.Subscribe<InventoryChangedEvent>(OnInventoryChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<HealthChangedEvent>(OnHealthChanged);
            EventBus.Unsubscribe<GamePausedEvent>(OnGamePaused);
            EventBus.Unsubscribe<ProjectileHitEvent>(OnProjectileHit);
            EventBus.Unsubscribe<GameSavedEvent>(OnGameSaved);
            EventBus.Unsubscribe<SceneLoadingEvent>(OnSceneLoading);
            EventBus.Unsubscribe<InventoryChangedEvent>(OnInventoryChanged);
        }

        // =========================================
        // HANDLERS
        // =========================================
        private void OnHealthChanged(HealthChangedEvent e)
        {
            if (_audio == null) return;

            if (e.IsDead)
                Play(healthSounds.onDeathSoundId, e.HitPoint);
            else if (e.Delta < 0)
            {
                Play(healthSounds.onDamageSoundId, e.HitPoint);

                // Low health warning
                if (healthSounds.lowHealthThreshold > 0f)
                {
                    float ratio = (float)e.NewValue / e.MaxValue;
                    if (ratio <= healthSounds.lowHealthThreshold)
                        Play(healthSounds.onLowHealthId, e.HitPoint);
                }
            }
            else if (e.Delta > 0)
                Play(healthSounds.onHealSoundId, e.HitPoint);
        }

        private void OnGamePaused(GamePausedEvent e)
        {
            AudioListener.pause = e.IsPaused;

            if (e.IsPaused)
                Play(gameStateSounds.onPauseSoundId, Vector3.zero);
            else
                Play(gameStateSounds.onResumeSoundId, Vector3.zero);
        }

        private void OnProjectileHit(ProjectileHitEvent e)
        {
            Play(projectileSounds.onHitSoundId, e.HitPoint);
        }

        private void OnGameSaved(GameSavedEvent e)
        {
            if (e.IsSaving)
                Play(gameStateSounds.onSaveSoundId, Vector3.zero);
            else
                Play(gameStateSounds.onLoadSoundId, Vector3.zero);
        }

        private void OnSceneLoading(SceneLoadingEvent e)
        {
            if (e.IsLoading)
                Play(sceneSounds.onSceneLoadStartId, Vector3.zero);
        }

        private void OnInventoryChanged(InventoryChangedEvent e)
        {
            if (e.Delta > 0)
                Play(inventorySounds.onItemAddedId, Vector3.zero);
            else
                Play(inventorySounds.onItemRemovedId, Vector3.zero);
        }

        // =========================================
        // PLAY HELPER — skips empty ids silently
        // =========================================
        private void Play(string soundId, Vector3 position)
        {
            if (string.IsNullOrEmpty(soundId)) return;
            if (_audio == null) return;

            _audio.Play(soundId, position);
        }

        // =========================================
        // TRIGGER CUSTOM SOUND FROM CODE
        // For game-specific events not covered above
        // =========================================
        public void PlayCustomSound(string label)
        {
            if (customSounds == null) return;

            foreach (var entry in customSounds)
            {
                if (entry.label == label)
                {
                    Play(entry.soundId, entry.positionOffset);
                    return;
                }
            }
        }
    }
}