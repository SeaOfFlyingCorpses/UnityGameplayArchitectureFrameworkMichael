using System.Collections.Generic;
using UnityEngine;
using Framework.Audio;
using Framework.Core;
using Framework.AI.Faction;

namespace Gameplay.Audio
{
    // =========================================
    // WwiseAudioSystem
    // IAudioSystem implementation using Wwise.
    // Requires Wwise Unity Integration package.
    //
    // SETUP FOR YOUR SOUND DESIGNER:
    //   1. Install Wwise Unity Integration
    //   2. Add this component to _GameSystems
    //   3. Remove UnityAudioSystem component
    //   4. Fill RTPC Mappings, State Groups,
    //      and Switch Groups in the Inspector
    //   5. Names must match exactly what is
    //      defined in the Wwise project
    //
    // HOW EACH SECTION WORKS:
    //
    //   RTPC Mappings
    //     Continuously updated float values.
    //     Left side  = name in Wwise project
    //     Right side = which game value drives it
    //     Set Update Rate to control how often
    //     Wwise receives the value (default every frame)
    //
    //   State Groups
    //     Named state switches (e.g. Music = Combat)
    //     Triggered by MusicSystem automatically
    //     or manually via SetState()
    //
    //   Switch Groups
    //     Per-object audio variants
    //     (e.g. surface type, weapon type)
    //     Set via SetSwitch() from game code
    //
    //   Default Events
    //     Sound events to post automatically
    //     on game start, scene load, etc.
    // =========================================
    public class WwiseAudioSystem : MonoBehaviour, IAudioSystem
    {
        // =========================================
        // RTPC MAPPING
        // =========================================
        public enum RTPCSource
        {
            None,
            DirectorIntensity,  // AIDirector.State.Intensity
            PlayerFear,         // player squad avg fear
            PlayerMorale,       // player squad avg morale
            EnemyCount,         // active enemy count
            MusicIntensity,     // driven by MusicSystem
            Custom              // set manually from code
        }

        [System.Serializable]
        public class RTPCMapping
        {
            [Tooltip("Must match RTPC name in Wwise project exactly")]
            public string     wwiseRtpcName = "";

            [Tooltip("Which game value drives this RTPC")]
            public RTPCSource source        = RTPCSource.None;

            [Tooltip("Multiply game value by this before sending to Wwise")]
            public float      scale         = 100f;

            [Tooltip("Update every N seconds. 0 = every frame")]
            public float      updateRate    = 0f;

            // Runtime
            [System.NonSerialized] public float timer;
            [System.NonSerialized] public float currentValue;
        }

        // =========================================
        // STATE GROUP MAPPING
        // =========================================
        [System.Serializable]
        public class StateGroupEntry
        {
            [Tooltip("State Group name in Wwise (e.g. 'Music', 'Ambience')")]
            public string groupName  = "";

            [Tooltip("Default state on game start")]
            public string defaultState = "None";
        }

        // =========================================
        // SWITCH GROUP
        // =========================================
        [System.Serializable]
        public class SwitchGroupEntry
        {
            [Tooltip("Switch Group name in Wwise (e.g. 'Surface', 'Weapon')")]
            public string groupName    = "";

            [Tooltip("Default switch value")]
            public string defaultValue = "Default";
        }

        // =========================================
        // INSPECTOR FIELDS
        // =========================================
        [Header("RTPC Mappings")]
        [Tooltip("Each entry maps a game value to a Wwise RTPC. " +
                 "Names must match Wwise project exactly.")]
        public RTPCMapping[] rtpcMappings;

        [Header("State Groups")]
        [Tooltip("State groups used in this game. " +
                 "Default state applied on Start.")]
        public StateGroupEntry[] stateGroups;

        [Header("Switch Groups")]
        [Tooltip("Switch groups used in this game.")]
        public SwitchGroupEntry[] switchGroups;

        [Header("Startup Events")]
        [Tooltip("Wwise events to post when game starts")]
        public string[] startupEvents;

        // =========================================
        // RUNTIME
        // =========================================
        private Gameplay.AI.Director.AIDirector _director;
        private Gameplay.AI.Squad.SquadSystem   _squad;

        private void Awake()
        {
            ServiceLocator.Register<IAudioSystem>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IAudioSystem>();
        }

        private void Start()
        {
            _director = ServiceLocator.Get<Gameplay.AI.Director.AIDirector>();
            _squad    = ServiceLocator.Get<Gameplay.AI.Squad.SquadSystem>();

            ApplyDefaultStates();
            PostStartupEvents();
        }

        private void Update()
        {
            UpdateRTPCs();
        }

        // =========================================
        // AUTO-UPDATE RTPCs EACH FRAME
        // =========================================
        private void UpdateRTPCs()
        {
            if (rtpcMappings == null) return;

            foreach (var mapping in rtpcMappings)
            {
                if (string.IsNullOrEmpty(mapping.wwiseRtpcName)) continue;
                if (mapping.source == RTPCSource.None) continue;

                // Rate limiting
                if (mapping.updateRate > 0f)
                {
                    mapping.timer += Time.deltaTime;
                    if (mapping.timer < mapping.updateRate) continue;
                    mapping.timer = 0f;
                }

                float value = GetSourceValue(mapping.source) * mapping.scale;

                if (!Mathf.Approximately(value, mapping.currentValue))
                {
                    mapping.currentValue = value;
                    SetParameter(mapping.wwiseRtpcName, value);
                }
            }
        }

        private float GetSourceValue(RTPCSource source)
        {
            switch (source)
            {
                case RTPCSource.DirectorIntensity:
                    return _director?.State.Intensity ?? 0f;

                case RTPCSource.EnemyCount:
                    float maxEnemies = _director?.State.MaxEnemies ?? 1f;
                    float active     = _director?.State.ActiveEnemies ?? 0f;
                    return active / maxEnemies;

                case RTPCSource.PlayerFear:
                    return GetSquadAverage(
                        Framework.AI.Faction.Team.Player,
                        ctx => ctx.Fear);

                case RTPCSource.PlayerMorale:
                    return GetSquadAverage(
                        Framework.AI.Faction.Team.Player,
                        ctx => ctx.Morale);

                default:
                    return 0f;
            }
        }

        private float GetSquadAverage(
            Framework.AI.Faction.Team team,
            System.Func<Framework.StateMachine.StateContext, float> selector)
        {
            if (_squad == null) return 0f;

            var squad = _squad.GetSquad(team);
            if (squad.Members.Count == 0) return 0f;

            float total = 0f;
            foreach (var m in squad.Members)
                total += selector(m.Context);

            return total / squad.Members.Count;
        }

        private void ApplyDefaultStates()
        {
            if (stateGroups == null) return;

            foreach (var group in stateGroups)
                if (!string.IsNullOrEmpty(group.groupName) &&
                    !string.IsNullOrEmpty(group.defaultState))
                    SetState(group.groupName, group.defaultState);
        }

        private void PostStartupEvents()
        {
            if (startupEvents == null) return;

            foreach (var eventId in startupEvents)
                if (!string.IsNullOrEmpty(eventId))
                    Play(eventId, Vector3.zero);
        }

        // =========================================
        // IAudioSystem IMPLEMENTATION
        // Uncomment AkSoundEngine calls when
        // Wwise package is installed
        // =========================================
        public void Play(string eventId, Vector3 position)
        {
            if (string.IsNullOrEmpty(eventId)) return;
            // AkSoundEngine.PostEvent(eventId, gameObject);
            Debug.Log($"[Wwise] Post: {eventId}");
        }

        public void PlayOnObject(string eventId, GameObject source)
        {
            if (string.IsNullOrEmpty(eventId) || source == null) return;
            // AkSoundEngine.PostEvent(eventId, source);
            Debug.Log($"[Wwise] Post: {eventId} on {source.name}");
        }

        public void Stop(string eventId, GameObject source)
        {
            if (string.IsNullOrEmpty(eventId) || source == null) return;
            // AkSoundEngine.PostEvent($"Stop_{eventId}", source);
            Debug.Log($"[Wwise] Stop: {eventId}");
        }

        public void StopAll()
        {
            // AkSoundEngine.StopAll();
            Debug.Log("[Wwise] StopAll");
        }

        public void SetParameter(string parameterId, float value)
        {
            if (string.IsNullOrEmpty(parameterId)) return;
            // AkSoundEngine.SetRTPCValue(parameterId, value);
            Debug.Log($"[Wwise] RTPC: {parameterId} = {value:F2}");
        }

        public void SetParameter(string parameterId, float value, GameObject source)
        {
            if (string.IsNullOrEmpty(parameterId) || source == null) return;
            // AkSoundEngine.SetRTPCValue(parameterId, value, source);
            Debug.Log($"[Wwise] RTPC: {parameterId} = {value:F2} on {source.name}");
        }

        public void SetState(string stateGroup, string stateName)
        {
            if (string.IsNullOrEmpty(stateGroup)) return;
            // AkSoundEngine.SetState(stateGroup, stateName);
            Debug.Log($"[Wwise] State: {stateGroup}/{stateName}");
        }

        public void SetSwitch(string switchGroup, string switchValue, GameObject source)
        {
            if (string.IsNullOrEmpty(switchGroup) || source == null) return;
            // AkSoundEngine.SetSwitch(switchGroup, switchValue, source);
            Debug.Log($"[Wwise] Switch: {switchGroup}/{switchValue} on {source.name}");
        }

        public void SetMusicIntensity(float intensity)
        {
            // Handled automatically via RTPCMappings
            // Map MusicIntensity source to your RTPC
        }

        public void SetMusicState(string stateName)
        {
            SetState("Music", stateName);
        }
    }
}