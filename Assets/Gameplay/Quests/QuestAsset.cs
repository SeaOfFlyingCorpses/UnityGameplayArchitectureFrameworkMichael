using System.Collections.Generic;
using UnityEngine;
using Framework.Quests;

namespace Gameplay.Quests
{
    // =========================================
    // QuestAsset
    // ScriptableObject — author quests in
    // the Inspector without writing code.
    //
    // Create:
    //   Right click → Create → Gameplay → Quest
    //
    // Usage:
    //   Drag onto QuestGiver or register at
    //   game start via QuestBootstrap.
    // =========================================
    [CreateAssetMenu(
        fileName = "NewQuest",
        menuName = "Gameplay/Quest")]
    public class QuestAsset : ScriptableObject
    {
        [System.Serializable]
        public class ObjectiveData
        {
            public string id;
            [TextArea(1, 3)]
            public string description;
            public int    required = 1;
        }

        [Header("Identity")]
        public string questId;
        public string title;
        [TextArea(2, 4)]
        public string description;

        [Header("Objectives")]
        public List<ObjectiveData> objectives = new();

        [Header("Settings")]
        [Tooltip("Register and start this quest automatically on game start")]
        public bool autoStart = false;

        public IQuest Build()
        {
            var quest = new Quest(
                string.IsNullOrEmpty(questId) ? name : questId,
                title,
                description);

            foreach (var data in objectives)
                quest.AddObjective(new QuestObjective(
                    data.id,
                    data.description,
                    data.required));

            return quest;
        }
    }
}
