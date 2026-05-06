namespace Framework.Audio
{
    // =========================================
    // IAudioSystem
    // Framework-level audio interface.
    // Zero dependency on Unity Audio or Wwise.
    //
    // Implementations:
    //   UnityAudioSystem  — uses AudioSource/Mixer
    //   WwiseAudioSystem  — uses AkSoundEngine
    //
    // Register as service:
    //   ServiceLocator.Register<IAudioSystem>(this);
    //
    // Use from anywhere:
    //   ServiceLocator.Get<IAudioSystem>()
    //       ?.Play("explosion", position);
    // =========================================
    public interface IAudioSystem
    {
        // =========================================
        // PLAYBACK
        // =========================================

        // Play a one-shot sound at world position
        void Play(string eventId, UnityEngine.Vector3 position);

        // Play a one-shot sound on a specific object
        void PlayOnObject(string eventId, UnityEngine.GameObject source);

        // Stop a playing event
        void Stop(string eventId, UnityEngine.GameObject source);

        // Stop all audio
        void StopAll();

        // =========================================
        // PARAMETERS — drive dynamic audio
        // Maps to Wwise RTPC or Unity mixer params
        // =========================================

        // Set a float parameter (e.g. "Intensity", "Fear")
        void SetParameter(string parameterId, float value);

        // Set a float parameter on a specific object
        void SetParameter(
            string             parameterId,
            float              value,
            UnityEngine.GameObject source);

        // =========================================
        // STATES — switch audio states
        // Maps to Wwise State or Unity mixer snapshot
        // =========================================

        // Set a named state (e.g. "Music", "Combat")
        void SetState(string stateGroup, string stateName);

        // =========================================
        // SWITCHES — per-object audio variants
        // Maps to Wwise Switch or Unity AudioClip swap
        // =========================================

        // Set a switch on an object
        // (e.g. surface type, weapon type, character)
        void SetSwitch(
            string             switchGroup,
            string             switchValue,
            UnityEngine.GameObject source);

        // =========================================
        // MUSIC
        // =========================================

        // Set music intensity 0-1
        // Drives stem mixing or Wwise RTPC
        void SetMusicIntensity(float intensity);

        // Trigger a music transition
        void SetMusicState(string stateName);
    }
}
