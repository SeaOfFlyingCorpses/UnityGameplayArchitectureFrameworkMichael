using UnityEngine;
using Framework.Core;
using Framework.Quests;

namespace Gameplay.Quests
{
    // =========================================
    // QuestBootstrap
    // Registers quest assets with QuestSystem
    // at game start.
    // Place on _GameSystems.
    //
    // Drag QuestAssets into the array.
    // Assets with autoStart = true start
    // immediately when the game loads.
    // =========================================
    public class QuestBootstrap : MonoBehaviour
    {
        [Header("Quests")]
        [Tooltip("All quests available in this scene")]
        public QuestAsset[] quests;

        private void Start()
        {
            var system = ServiceLocator.Get<IQuestSystem>();
            if (system == null)
            {
                Debug.LogWarning(
                    "[QuestBootstrap] No IQuestSystem found. " +
                    "Add QuestSystem to _GameSystems.");
                return;
            }

            if (quests == null) return;

            foreach (var asset in quests)
            {
                if (asset == null) continue;

                var quest = asset.Build();
                system.Register(quest);

                if (asset.autoStart)
                    system.StartQuest(quest.Id);
            }
        }
    }
}