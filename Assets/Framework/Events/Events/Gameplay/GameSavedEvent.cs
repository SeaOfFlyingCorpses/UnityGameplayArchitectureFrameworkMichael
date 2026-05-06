namespace Framework.Events.Events.Gameplay
{
    // =========================================
    // GameSavedEvent
    // Fired by SaveSystem after save or load.
    // Subscribe to show save indicator UI,
    // play a save sound, etc.
    //
    // IsSaving = true  — just saved
    // IsSaving = false — just loaded
    // =========================================
    public struct GameSavedEvent
    {
        public bool IsSaving;

        public GameSavedEvent(bool isSaving)
        {
            IsSaving = isSaving;
        }
    }
}
