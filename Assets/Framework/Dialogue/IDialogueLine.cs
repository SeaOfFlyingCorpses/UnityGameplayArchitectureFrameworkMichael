namespace Framework.Dialogue
{
    // =========================================
    // IDialogueLine
    // A single line of dialogue.
    // Speaker, text, optional portrait key,
    // optional audio event id.
    // =========================================
    public interface IDialogueLine
    {
        string SpeakerName  { get; }
        string Text         { get; }
        string PortraitKey  { get; } // optional — for UI portrait
        string AudioEventId { get; } // optional — for voice acting
    }
}
