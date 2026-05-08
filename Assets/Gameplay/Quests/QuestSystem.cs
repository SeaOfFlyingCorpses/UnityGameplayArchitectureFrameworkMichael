using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using Framework.Events;
using Framework.Quests;
using Framework.Events.Events.Gameplay;

namespace Gameplay.Quests
{
    // =========================================
    // QuestSystem
    // MonoBehaviour implementation of IQuestSystem.
    // Place on _GameSystems.
    //
    // Publishes quest events via EventBus.
    // UI subscribes to show quest tracker.
    // Any system can progress objectives by
    // calling Progress() — zero coupling.
    //
    // Usage:
    //   var qs = ServiceLocator.Get<IQuestSystem>();
    //   qs.Register(myQuest);
    //   qs.StartQuest("rescue_villager");
    //   qs.Progress("rescue_villager", "kill_bandits", 1);
    // =========================================
    public class QuestSystem : MonoBehaviour, IQuestSystem
    {
        private readonly Dictionary<string, IQuest> _quests    = new();
        private readonly List<IQuest>               _active    = new();
        private readonly List<IQuest>               _completed = new();

        public IReadOnlyList<IQuest> Active    => _active;
        public IReadOnlyList<IQuest> Completed => _completed;

        private void Awake()
        {
            ServiceLocator.Register<IQuestSystem>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IQuestSystem>();
        }

        // =========================================
        // IQuestSystem
        // =========================================
        public void Register(IQuest quest)
        {
            if (quest == null) return;
            if (_quests.ContainsKey(quest.Id)) return;
            _quests[quest.Id] = quest;
        }

        public void StartQuest(string questId)
        {
            var quest = GetQuest(questId);
            if (quest == null || quest.IsActive) return;

            quest.Start();
            _active.Add(quest);

            EventBus.Publish(new QuestStartedEvent(
                quest.Id, quest.Title, quest.Description));
        }

        public void Progress(
            string questId,
            string objectiveId,
            int    amount = 1)
        {
            var quest = GetQuest(questId);
            if (quest == null || !quest.IsActive) return;

            // Find the objective
            IQuestObjective objective = null;
            foreach (var obj in quest.Objectives)
            {
                if (obj.Id == objectiveId)
                {
                    objective = obj;
                    break;
                }
            }

            if (objective == null || objective.IsComplete) return;

            objective.Progress(amount);

            EventBus.Publish(new QuestObjectiveUpdatedEvent(
                quest.Id,
                objective.Id,
                objective.Description,
                objective.Current,
                objective.Required,
                objective.IsComplete));

            // Auto-complete quest when all objectives done
            var concreteQuest = quest as Quest;
            if (concreteQuest != null &&
                concreteQuest.AllObjectivesComplete())
                Complete(questId);
        }

        public void Complete(string questId)
        {
            var quest = GetQuest(questId);
            if (quest == null || !quest.IsActive) return;

            quest.Complete();
            _active.Remove(quest);
            _completed.Add(quest);

            EventBus.Publish(new QuestCompletedEvent(
                quest.Id, quest.Title));
        }

        public void Fail(string questId)
        {
            var quest = GetQuest(questId);
            if (quest == null || !quest.IsActive) return;

            quest.Fail();
            _active.Remove(quest);

            EventBus.Publish(new QuestFailedEvent(
                quest.Id, quest.Title));
        }

        public IQuest GetQuest(string questId)
        {
            _quests.TryGetValue(questId, out var quest);
            return quest;
        }

        public bool IsComplete(string questId)
            => GetQuest(questId)?.IsComplete ?? false;

        public bool IsActive(string questId)
            => GetQuest(questId)?.IsActive ?? false;
    }
}