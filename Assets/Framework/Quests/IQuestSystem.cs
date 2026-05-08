using System.Collections.Generic;

namespace Framework.Quests
{
    // =========================================
    // IQuestSystem
    // Manages all quests.
    // Register as ServiceLocator service.
    //
    // Usage:
    //   ServiceLocator.Get<IQuestSystem>()
    //       ?.Start("rescue_villager");
    // =========================================
    public interface IQuestSystem
    {
        IReadOnlyList<IQuest> Active    { get; }
        IReadOnlyList<IQuest> Completed { get; }

        void  Register  (IQuest quest);
        void  StartQuest (string questId);
        void  Complete  (string questId);
        void  Fail      (string questId);
        void  Progress  (string questId, string objectiveId, int amount = 1);

        IQuest     GetQuest    (string questId);
        bool       IsComplete  (string questId);
        bool       IsActive    (string questId);
    }
}