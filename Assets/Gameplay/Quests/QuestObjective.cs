using Framework.Quests;

namespace Gameplay.Quests
{
    // =========================================
    // QuestObjective
    // Plain C# — no MonoBehaviour needed.
    //
    // Usage:
    //   new QuestObjective("kill_wolves", "Kill 5 wolves", required: 5)
    //   new QuestObjective("find_sword",  "Find the sword", required: 1)
    // =========================================
    public class QuestObjective : IQuestObjective
    {
        public string Id          { get; }
        public string Description { get; }
        public int    Current     { get; private set; }
        public int    Required    { get; }
        public bool   IsComplete  => Current >= Required;

        public QuestObjective(
            string id,
            string description,
            int    required = 1)
        {
            Id          = id;
            Description = description;
            Required    = required;
            Current     = 0;
        }

        public void Progress(int amount = 1)
        {
            if (IsComplete) return;
            Current += amount;
            if (Current > Required)
                Current = Required;
        }

        public void Complete() => Current = Required;
        public void Reset()    => Current = 0;
    }
}
