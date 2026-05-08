namespace Framework.Events.Events.Gameplay
{
    // =========================================
    // DialogueStartedEvent
    // Fired when a dialogue tree begins.
    // UI subscribes to show dialogue panel.
    // PauseSystem can subscribe to pause game.
    // =========================================
    public struct DialogueStartedEvent
    {
        public string TreeId;
        public string Title;

        public DialogueStartedEvent(string treeId, string title)
        {
            TreeId = treeId;
            Title  = title;
        }
    }

    // =========================================
    // DialogueLineEvent
    // Fired when a new line becomes active.
    // UI subscribes to update speaker/text.
    // Audio subscribes to play voice line.
    // =========================================
    public struct DialogueLineEvent
    {
        public string SpeakerName;
        public string Text;
        public string PortraitKey;
        public string AudioEventId;

        public DialogueLineEvent(
            string speakerName,
            string text,
            string portraitKey  = "",
            string audioEventId = "")
        {
            SpeakerName  = speakerName;
            Text         = text;
            PortraitKey  = portraitKey;
            AudioEventId = audioEventId;
        }
    }

    // =========================================
    // DialogueChoicesEvent
    // Fired when the player must choose.
    // UI subscribes to show choice buttons.
    // =========================================
    public struct DialogueChoicesEvent
    {
        public string[] Choices;

        public DialogueChoicesEvent(string[] choices)
        {
            Choices = choices;
        }
    }

    // =========================================
    // DialogueEndedEvent
    // Fired when dialogue tree finishes.
    // UI subscribes to hide dialogue panel.
    // =========================================
    public struct DialogueEndedEvent
    {
        public string TreeId;

        public DialogueEndedEvent(string treeId)
        {
            TreeId = treeId;
        }
    }
}
