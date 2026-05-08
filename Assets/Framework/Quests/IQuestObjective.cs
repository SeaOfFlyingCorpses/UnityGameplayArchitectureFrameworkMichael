namespace Framework.Quests
{
    // =========================================
    // IQuestObjective
    // A single trackable goal within a quest.
    // Examples:
    //   Kill 5 enemies
    //   Collect 3 herbs
    //   Reach the castle
    //   Talk to the blacksmith
    // =========================================
    public interface IQuestObjective
    {
        string Id          { get; }
        string Description { get; }
        int    Current     { get; }
        int    Required    { get; }
        bool   IsComplete  { get; }

        void Progress(int amount = 1);
        void Complete();
        void Reset();
    }
}
