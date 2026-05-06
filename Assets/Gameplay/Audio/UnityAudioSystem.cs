using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Framework.Audio;
using Framework.Core;

namespace Gameplay.Audio
{
    // =========================================
    // UnityAudioSystem
    // IAudioSystem implementation using Unity's
    // built-in audio — AudioSource and AudioMixer.
    //
    // Setup:
    //   1. Add to _GameSystems GameObject
    //   2. Assign AudioMixer (optional)
    //   3. Add AudioClipEntry assets to the
    //      Clips array — map event id to clip
    //   4. Done — use via IAudioSystem interface
    //
    // To switch to Wwise later:
    //   Remove this component
    //   Add WwiseAudioSystem component
    //   Zero other code changes
    // =========================================
    public class UnityAudioSystem : MonoBehaviour, IAudioSystem
    {
        [System.Serializable]
        public class AudioClipEntry
        {
            [Tooltip("Event id used in code — e.g. 'explosion', 'hit'")]
            public string    eventId;
            public AudioClip clip;
            [Range(0f, 1f)]
            public float     volume  = 1f;
            [Range(0.5f, 2f)]
            public float     pitch   = 1f;
            public bool      loop    = false;
        }

        [Header("Audio Library")]
        [Tooltip("Map event ids to AudioClips")]
        public AudioClipEntry[] clips;

        [Header("References")]
        public AudioMixer mixer;

        [Header("Settings")]
        [Tooltip("Pool size for one-shot AudioSources")]
        public int poolSize = 10;

        private readonly Dictionary<string, AudioClipEntry> _library = new();
        private readonly Queue<AudioSource>                 _pool    = new();

        private void Awake()
        {
            ServiceLocator.Register<IAudioSystem>(this);
            BuildLibrary();
            PrewarmPool();
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IAudioSystem>();
        }

        private void BuildLibrary()
        {
            if (clips == null) return;

            foreach (var entry in clips)
                if (!string.IsNullOrEmpty(entry.eventId))
                    _library[entry.eventId] = entry;
        }

        private void PrewarmPool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                var source = CreateSource();
                source.gameObject.SetActive(false);
                _pool.Enqueue(source);
            }
        }

        private AudioSource CreateSource()
        {
            var go     = new GameObject("AudioSource");
            go.transform.SetParent(transform);
            var source = go.AddComponent<AudioSource>();
            return source;
        }

        // =========================================
        // PLAYBACK
        // =========================================
        public void Play(string eventId, Vector3 position)
        {
            if (!_library.TryGetValue(eventId, out var entry))
            {
                Debug.LogWarning($"[UnityAudioSystem] Event '{eventId}' not found.");
                return;
            }

            var source = GetSource();
            source.gameObject.SetActive(true);
            source.transform.position = position;
            source.clip               = entry.clip;
            source.volume             = entry.volume;
            source.pitch              = entry.pitch;
            source.loop               = entry.loop;
            source.Play();

            if (!entry.loop)
                StartCoroutine(ReturnAfterPlay(source, entry.clip.length));
        }

        public void PlayOnObject(string eventId, GameObject obj)
        {
            if (!_library.TryGetValue(eventId, out var entry)) return;

            var source = obj.GetComponent<AudioSource>()
                      ?? obj.AddComponent<AudioSource>();

            source.clip   = entry.clip;
            source.volume = entry.volume;
            source.pitch  = entry.pitch;
            source.loop   = entry.loop;
            source.Play();
        }

        public void Stop(string eventId, GameObject obj)
        {
            var source = obj.GetComponent<AudioSource>();
            source?.Stop();
        }

        public void StopAll()
        {
            var sources = GetComponentsInChildren<AudioSource>();
            foreach (var s in sources)
                s.Stop();
        }

        // =========================================
        // PARAMETERS — maps to AudioMixer params
        // =========================================
        public void SetParameter(string parameterId, float value)
        {
            mixer?.SetFloat(parameterId, value);
        }

        public void SetParameter(string parameterId, float value, GameObject source)
        {
            // Unity doesn't support per-object params without Wwise
            // Apply globally as fallback
            SetParameter(parameterId, value);
        }

        // =========================================
        // STATES — maps to AudioMixer snapshots
        // =========================================
        public void SetState(string stateGroup, string stateName)
        {
            // Unity: transition to a mixer snapshot
            // Snapshot must be named "{stateGroup}_{stateName}"
            // e.g. "Music_Combat", "Music_Explore"
            var snapshotName = $"{stateGroup}_{stateName}";
            var snapshot     = FindSnapshot(snapshotName);
            snapshot?.TransitionTo(0.5f);
        }

        private AudioMixerSnapshot FindSnapshot(string name)
        {
            if (mixer == null) return null;

            // Unity doesn't expose snapshot search directly
            // Store snapshots in a dictionary if needed
            return null;
        }

        // =========================================
        // SWITCHES — not natively supported in Unity
        // Use AudioClipEntry per variant as workaround
        // =========================================
        public void SetSwitch(string switchGroup, string switchValue, GameObject source)
        {
            // Unity workaround: play event id = "switchGroup_switchValue"
            Play($"{switchGroup}_{switchValue}", source.transform.position);
        }

        // =========================================
        // MUSIC
        // =========================================
        public void SetMusicIntensity(float intensity)
        {
            // Map 0-1 intensity to mixer parameter
            // e.g. "MusicIntensity" exposed on AudioMixer
            mixer?.SetFloat("MusicIntensity", intensity);
        }

        public void SetMusicState(string stateName)
        {
            SetState("Music", stateName);
        }

        // =========================================
        // POOL HELPERS
        // =========================================
        private AudioSource GetSource()
        {
            if (_pool.Count > 0)
                return _pool.Dequeue();

            return CreateSource();
        }

        private System.Collections.IEnumerator ReturnAfterPlay(
            AudioSource source, float duration)
        {
            yield return new WaitForSeconds(duration + 0.1f);

            source.Stop();
            source.gameObject.SetActive(false);
            _pool.Enqueue(source);
        }
    }
}
