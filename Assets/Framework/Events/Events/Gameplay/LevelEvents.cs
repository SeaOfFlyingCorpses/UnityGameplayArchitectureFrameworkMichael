namespace Framework.Events.Events.Gameplay
{
    // =========================================
    // ExperienceGainedEvent
    // Fired when XP is added.
    // UI subscribes to show XP gain popup.
    // =========================================
    public struct ExperienceGainedEvent
    {
        public int Amount;
        public int Total;
        public int NextLevel;

        public ExperienceGainedEvent(int amount, int total, int nextLevel)
        {
            Amount    = amount;
            Total     = total;
            NextLevel = nextLevel;
        }
    }

    // =========================================
    // LevelUpEvent
    // Fired when agent reaches a new level.
    // UI subscribes to show level-up screen.
    // StatSystem subscribes to apply stat bonuses.
    // QuestSystem subscribes to unlock quests.
    // =========================================
    public struct LevelUpEvent
    {
        public int OldLevel;
        public int NewLevel;

        public LevelUpEvent(int oldLevel, int newLevel)
        {
            OldLevel = oldLevel;
            NewLevel = newLevel;
        }
    }
}
