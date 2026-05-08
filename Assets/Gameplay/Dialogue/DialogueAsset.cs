using System.Collections.Generic;
using UnityEngine;
using Framework.Dialogue;

namespace Gameplay.Dialogue
{
    // =========================================
    // DialogueAsset
    // ScriptableObject — author dialogue in the
    // Inspector without writing code.
    //
    // Create:
    //   Right click → Create → Gameplay → Dialogue
    //
    // Usage:
    //   Drag asset onto a DialogueTrigger component
    //   or call Build() to get an IDialogueTree.
    // =========================================
    [CreateAssetMenu(
        fileName = "NewDialogue",
        menuName = "Gameplay/Dialogue")]
    public class DialogueAsset : ScriptableObject
    {
        [System.Serializable]
        public class LineData
        {
            public string speakerName;
            [TextArea(2, 5)]
            public string text;
            public string portraitKey;
            public string audioEventId;
        }

        [Header("Identity")]
        public string dialogueId;
        public string title;

        [Header("Lines")]
        public List<LineData> lines = new();

        // =========================================
        // Build — converts asset data into a
        // runtime DialogueTree
        // =========================================
        public IDialogueTree Build()
        {
            var tree = new DialogueTree(
                string.IsNullOrEmpty(dialogueId)
                    ? name : dialogueId,
                title);

            foreach (var data in lines)
                tree.Add(new DialogueLine(
                    data.speakerName,
                    data.text,
                    data.portraitKey,
                    data.audioEventId));

            return tree;
        }
    }
}
