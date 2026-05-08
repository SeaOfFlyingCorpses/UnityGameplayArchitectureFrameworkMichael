namespace Framework.Events.Events.Gameplay
{
    // =========================================
    // QuestStartedEvent
    // Fired when a quest becomes active.
    // UI subscribes to show "New Quest" banner.
    // =========================================
    public struct QuestStartedEvent
    {
        public string QuestId;
        public string Title;
        public string Description;

        public QuestStartedEvent(
            string questId,
            string title,
            string description)
        {
            QuestId     = questId;
            Title       = title;
            Description = description;
        }
    }

    // =========================================
    // QuestObjectiveUpdatedEvent
    // Fired when an objective progresses.
    // UI subscribes to update quest tracker.
    // =========================================
    public struct QuestObjectiveUpdatedEvent
    {
        public string QuestId;
        public string ObjectiveId;
        public string Description;
        public int    Current;
        public int    Required;
        public bool   IsComplete;

        public QuestObjectiveUpdatedEvent(
            string questId,
            string objectiveId,
            string description,
            int    current,
            int    required,
            bool   isComplete)
        {
            QuestId      = questId;
            ObjectiveId  = objectiveId;
            Description  = description;
            Current      = current;
            Required     = required;
            IsComplete   = isComplete;
        }
    }

    // =========================================
    // QuestCompletedEvent
    // Fired when all objectives are done.
    // UI subscribes to show completion banner.
    // Reward system subscribes to grant rewards.
    // =========================================
    public struct QuestCompletedEvent
    {
        public string QuestId;
        public string Title;

        public QuestCompletedEvent(string questId, string title)
        {
            QuestId = questId;
            Title   = title;
        }
    }

    // =========================================
    // QuestFailedEvent
    // Fired when a quest fails.
    // =========================================
    public struct QuestFailedEvent
    {
        public string QuestId;
        public string Title;

        public QuestFailedEvent(string questId, string title)
        {
            QuestId = questId;
            Title   = title;
        }
    }
}
