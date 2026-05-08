using UnityEngine;
using Framework.Core;
using Framework.Dialogue;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Gameplay.Dialogue
{
    // =========================================
    // DialogueSystem
    // MonoBehaviour implementation of IDialogueSystem.
    // Place on _GameSystems.
    //
    // Zero coupling — publishes events via EventBus.
    // UI subscribes to DialogueLineEvent to display.
    // Audio subscribes to play voice lines.
    // Game code calls Play() to start a tree.
    //
    // Usage from any system:
    //   var dialogue = ServiceLocator.Get<IDialogueSystem>();
    //   dialogue?.Play(myTree);
    //
    // Or via EventBus trigger (even more decoupled):
    //   EventBus.Publish(new DialogueTriggerEvent(tree));
    // =========================================
    public class DialogueSystem : MonoBehaviour, IDialogueSystem
    {
        public bool          IsPlaying { get; private set; }
        public IDialogueTree Current   { get; private set; }

        private void Awake()
        {
            ServiceLocator.Register<IDialogueSystem>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IDialogueSystem>();
        }

        // =========================================
        // IDialogueSystem
        // =========================================
        public void Play(IDialogueTree tree)
        {
            if (tree == null) return;
            if (IsPlaying) Stop();

            Current   = tree;
            IsPlaying = true;

            Current.Start();

            EventBus.Publish(new DialogueStartedEvent(
                Current.Id, Current.Title));

            PublishCurrentLine();
        }

        public void Advance()
        {
            if (!IsPlaying || Current == null) return;

            // Check for choices before advancing
            var choices = Current.GetChoices();
            if (choices.Count > 0)
            {
                // Waiting for player choice — don't advance
                return;
            }

            Current.Advance();

            if (Current.IsDone)
            {
                EndDialogue();
                return;
            }

            PublishCurrentLine();
        }

        public void Choose(int choiceIndex)
        {
            if (!IsPlaying || Current == null) return;

            Current.Choose(choiceIndex);
            Current.Advance();

            if (Current.IsDone)
            {
                EndDialogue();
                return;
            }

            PublishCurrentLine();
        }

        public void Stop()
        {
            if (!IsPlaying) return;
            EndDialogue();
        }

        // =========================================
        // INTERNAL
        // =========================================
        private void PublishCurrentLine()
        {
            var line = Current?.Current;
            if (line == null) return;

            // Publish line for UI and audio
            EventBus.Publish(new DialogueLineEvent(
                line.SpeakerName,
                line.Text,
                line.PortraitKey,
                line.AudioEventId));

            // Publish choices if present
            var choices = Current.GetChoices();
            if (choices.Count > 0)
            {
                var choiceArray = new string[choices.Count];
                for (int i = 0; i < choices.Count; i++)
                    choiceArray[i] = choices[i];

                EventBus.Publish(new DialogueChoicesEvent(choiceArray));
            }
        }

        private void EndDialogue()
        {
            string treeId = Current?.Id ?? "";

            IsPlaying = false;
            Current   = null;

            EventBus.Publish(new DialogueEndedEvent(treeId));
        }
    }
}
