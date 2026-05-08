namespace Framework.Events.Events.Gameplay
{
    public struct StatusEffectAppliedEvent
    {
        public string EffectId;
        public string DisplayName;
        public float  Duration;

        public StatusEffectAppliedEvent(
            string effectId,
            string displayName,
            float  duration)
        {
            EffectId    = effectId;
            DisplayName = displayName;
            Duration    = duration;
        }
    }

    public struct StatusEffectExpiredEvent
    {
        public string EffectId;
        public string DisplayName;

        public StatusEffectExpiredEvent(string effectId, string displayName)
        {
            EffectId    = effectId;
            DisplayName = displayName;
        }
    }
}
