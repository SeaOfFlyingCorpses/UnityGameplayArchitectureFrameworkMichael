namespace Framework.Dialogue
{
    // =========================================
    // IDialogueSystem
    // Framework-level interface.
    // Zero dependency on Unity UI or any
    // specific dialogue format.
    //
    // Register as service:
    //   ServiceLocator.Register<IDialogueSystem>(this);
    //
    // Trigger from anywhere:
    //   ServiceLocator.Get<IDialogueSystem>()
    //       ?.Play(tree);
    // =========================================
    public interface IDialogueSystem
    {
        bool           IsPlaying  { get; }
        IDialogueTree  Current    { get; }

        void Play    (IDialogueTree tree);
        void Advance ();
        void Choose  (int choiceIndex);
        void Stop    ();
    }
}
