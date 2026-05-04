namespace Framework.Events.Events.Gameplay
{
    public struct GamePausedEvent
    {
        public bool IsPaused;

        public GamePausedEvent(bool isPaused)
        {
            IsPaused = isPaused;
        }
    }
}
