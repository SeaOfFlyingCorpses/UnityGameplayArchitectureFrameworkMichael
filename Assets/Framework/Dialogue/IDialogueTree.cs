using System.Collections.Generic;

namespace Framework.Dialogue
{
    // =========================================
    // IDialogueTree
    // A sequence of dialogue lines.
    // Can have branches (choices).
    // =========================================
    public interface IDialogueTree
    {
        string              Id       { get; }
        string              Title    { get; }
        IDialogueLine       Current  { get; }
        bool                IsActive { get; }
        bool                IsDone   { get; }

        void                Start();
        void                Advance();
        void                Choose(int choiceIndex);
        IReadOnlyList<string> GetChoices();
    }
}
