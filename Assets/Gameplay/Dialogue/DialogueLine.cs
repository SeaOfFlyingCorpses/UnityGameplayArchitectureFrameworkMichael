namespace Gameplay.Dialogue
{
    // =========================================
    // DialogueLine
    // Concrete implementation of IDialogueLine.
    // Plain C# — no MonoBehaviour needed.
    //
    // Usage:
    //   new DialogueLine("Guard", "Halt! Who goes there?")
    //   new DialogueLine("Merchant", "Welcome!", "merchant_happy", "vo_welcome")
    // =========================================
    public class DialogueLine : Framework.Dialogue.IDialogueLine
    {
        public string SpeakerName  { get; }
        public string Text         { get; }
        public string PortraitKey  { get; }
        public string AudioEventId { get; }

        public DialogueLine(
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
}
