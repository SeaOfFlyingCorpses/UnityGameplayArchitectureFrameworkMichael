using System.Collections.Generic;
using Framework.Quests;

namespace Gameplay.Quests
{
    // =========================================
    // Quest
    // Plain C# — no MonoBehaviour needed.
    //
    // Usage:
    //   var quest = new Quest("rescue_villager",
    //       "Rescue the Villager",
    //       "Find and rescue the kidnapped villager.")
    //       .AddObjective(new QuestObjective(
    //           "find_cave", "Find the bandit cave"))
    //       .AddObjective(new QuestObjective(
    //           "kill_bandits", "Defeat the bandits", 3))
    //       .AddObjective(new QuestObjective(
    //           "rescue", "Rescue the villager"));
    // =========================================
    public class Quest : IQuest
    {
        public string     Id          { get; }
        public string     Title       { get; }
        public string     Description { get; }
        public QuestState State       { get; private set; }

        public bool IsComplete => State == QuestState.Complete;
        public bool IsFailed   => State == QuestState.Failed;
        public bool IsActive   => State == QuestState.Active;

        private readonly List<IQuestObjective> _objectives = new();
        public IReadOnlyList<IQuestObjective>  Objectives  => _objectives;

        public Quest(string id, string title, string description = "")
        {
            Id          = id;
            Title       = title;
            Description = description;
            State       = QuestState.NotStarted;
        }

        // =========================================
        // BUILDER API — fluent chaining
        // =========================================
        public Quest AddObjective(IQuestObjective objective)
        {
            _objectives.Add(objective);
            return this;
        }

        // =========================================
        // IQuest
        // =========================================
        public void Start()
        {
            if (State != QuestState.NotStarted) return;
            State = QuestState.Active;
        }

        public void Complete()
        {
            if (State != QuestState.Active) return;
            State = QuestState.Complete;
        }

        public void Fail()
        {
            if (State != QuestState.Active) return;
            State = QuestState.Failed;
        }

        // =========================================
        // Check if all objectives are complete
        // Called by QuestSystem after each progress
        // =========================================
        public bool AllObjectivesComplete()
        {
            foreach (var obj in _objectives)
                if (!obj.IsComplete) return false;
            return _objectives.Count > 0;
        }
    }
}
