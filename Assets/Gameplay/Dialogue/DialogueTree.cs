using System.Collections.Generic;
using Framework.Dialogue;

namespace Gameplay.Dialogue
{
    // =========================================
    // DialogueTree
    // A sequence of lines with optional branches.
    //
    // Usage — linear dialogue:
    //   var tree = new DialogueTree("intro_guard", "Guard")
    //       .Add(new DialogueLine("Guard", "Halt!"))
    //       .Add(new DialogueLine("Guard", "State your business."));
    //
    // Usage — branching dialogue:
    //   var tree = new DialogueTree("shop_greet", "Merchant")
    //       .Add(new DialogueLine("Merchant", "What do you need?"))
    //       .AddBranch(
    //           new[] { "Buy something", "Just looking" },
    //           new[] { buyTree, leaveTree });
    // =========================================
    public class DialogueTree : IDialogueTree
    {
        public string Id       { get; }
        public string Title    { get; }
        public bool   IsActive { get; private set; }
        public bool   IsDone   => _index >= _lines.Count;

        public IDialogueLine Current =>
            _index < _lines.Count ? _lines[_index] : null;

        private readonly List<IDialogueLine>   _lines   = new();
        private readonly List<string[]>        _choices = new();
        private readonly List<IDialogueTree[]> _branches = new();
        private readonly List<int>             _branchAt = new();

        private int _index;

        public DialogueTree(string id, string title = "")
        {
            Id    = id;
            Title = title;
        }

        // =========================================
        // BUILDER API — fluent chaining
        // =========================================
        public DialogueTree Add(IDialogueLine line)
        {
            _lines.Add(line);
            return this;
        }

        public DialogueTree AddBranch(
            string[]        choices,
            IDialogueTree[] branches)
        {
            _branchAt.Add(_lines.Count);
            _choices.Add(choices);
            _branches.Add(branches);
            return this;
        }

        // =========================================
        // IDialogueTree
        // =========================================
        public void Start()
        {
            _index   = 0;
            IsActive = true;
        }

        public void Advance()
        {
            if (IsDone) return;
            _index++;
        }

        public void Choose(int choiceIndex)
        {
            int branchIndex = _branchAt.IndexOf(_index);
            if (branchIndex < 0) return;

            var branch = _branches[branchIndex];
            if (choiceIndex < 0 || choiceIndex >= branch.Length)
                return;

            // Start the chosen branch
            branch[choiceIndex].Start();
        }

        public IReadOnlyList<string> GetChoices()
        {
            int branchIndex = _branchAt.IndexOf(_index);
            if (branchIndex < 0)
                return System.Array.Empty<string>();

            return _choices[branchIndex];
        }

        public void End()
        {
            IsActive = false;
            _index   = _lines.Count;
        }
    }
}
