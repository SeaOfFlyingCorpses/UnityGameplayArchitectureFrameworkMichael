

namespace Framework.Events.Events.Gameplay
{
    public struct HealthChangedEvent
    {
        public int Value;

        public HealthChangedEvent(int value)
        {
            Value = value;
        }
    }
}
