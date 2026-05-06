using UnityEngine;
using Framework.Audio;
using Framework.Core;
using Framework.Events;
using Framework.Events.Events.Gameplay;
using Gameplay.AI.Director;
using Gameplay.AI.Squad;

namespace Gameplay.Audio
{
    // =========================================
    // MusicSystem
    // Reads framework state and drives music
    // automatically. Zero manual calls needed.
    //
    // What it reads:
    //   AIDirector.Intensity  → music tension
    //   SquadStrategy         → combat/explore state
    //   GamePausedEvent       → pause music
    //
    // What it calls on IAudioSystem:
    //   SetMusicIntensity()   → drives RTPC or mixer
    //   SetMusicState()       → triggers transitions
    //
    // Wwise mapping:
    //   RTPC "MusicIntensity" 0-100 drives stem mixing
    //   State "Music/Explore"  — low intensity
    //   State "Music/Combat"   — squad engaging
    //   State "Music/Stealth"  — high alert, no combat
    //   State "Music/Victory"  — all enemies dead
    // =========================================
    public class MusicSystem : MonoBehaviour
    {
        [Header("Thresholds")]
        [Tooltip("Intensity above this = Combat music")]
        public float combatThreshold   = 0.4f;

        [Tooltip("Intensity above this = Tense music")]
        public float tenseThreshold    = 0.2f;

        [Tooltip("Smoothing speed for intensity changes")]
        public float smoothSpeed       = 2f;

        private IAudioSystem _audio;
        private AIDirector   _director;
        private SquadSystem  _squad;

        private float  _smoothedIntensity;
        private string _currentMusicState = "";

        private void Start()
        {
            _audio   = ServiceLocator.Get<IAudioSystem>();
            _director = ServiceLocator.Get<AIDirector>();
            _squad    = ServiceLocator.Get<SquadSystem>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GamePausedEvent>(OnGamePaused);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GamePausedEvent>(OnGamePaused);
        }

        private void Update()
        {
            if (_audio == null) return;

            UpdateIntensity();
            UpdateMusicState();
        }

        private void UpdateIntensity()
        {
            float targetIntensity = _director != null
                ? _director.State.Intensity
                : 0f;

            // Factor in squad combat state
            bool isEngaging =
                _squad?.EnemySquad.CurrentStrategy
                    == Framework.AI.Squad.SquadStrategy.Engage;

            if (isEngaging)
                targetIntensity = Mathf.Max(targetIntensity, combatThreshold + 0.1f);

            // Smooth the intensity change
            _smoothedIntensity = Mathf.Lerp(
                _smoothedIntensity,
                targetIntensity,
                Time.deltaTime * smoothSpeed);

            _audio.SetMusicIntensity(_smoothedIntensity);
        }

        private void UpdateMusicState()
        {
            string targetState;

            bool isEngaging =
                _squad?.EnemySquad.CurrentStrategy
                    == Framework.AI.Squad.SquadStrategy.Engage;

            if (isEngaging || _smoothedIntensity >= combatThreshold)
                targetState = "Combat";
            else if (_smoothedIntensity >= tenseThreshold)
                targetState = "Stealth";
            else
                targetState = "Explore";

            // Only trigger transition when state changes
            if (targetState != _currentMusicState)
            {
                _currentMusicState = targetState;
                _audio.SetMusicState(_currentMusicState);

                Debug.Log($"[MusicSystem] → {_currentMusicState} " +
                          $"(intensity: {_smoothedIntensity:F2})");
            }
        }

        private void OnGamePaused(GamePausedEvent e)
        {
            // Pause all audio including music
            AudioListener.pause = e.IsPaused;
        }
    }
}
