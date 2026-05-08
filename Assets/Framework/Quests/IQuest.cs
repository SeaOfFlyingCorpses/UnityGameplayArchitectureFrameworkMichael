using System.Collections.Generic;

namespace Framework.Quests
{
    // =========================================
    // IQuest
    // A named objective sequence.
    // Can be active, complete, or failed.
    // =========================================
    public interface IQuest
    {
        string                          Id          { get; }
        string                          Title       { get; }
        string                          Description { get; }
        QuestState                      State       { get; }
        IReadOnlyList<IQuestObjective>  Objectives  { get; }

        bool IsComplete { get; }
        bool IsFailed   { get; }
        bool IsActive   { get; }

        void Start();
        void Fail();
        void Complete();
    }

    public enum QuestState
    {
        NotStarted,
        Active,
        Complete,
        Failed
    }
}
