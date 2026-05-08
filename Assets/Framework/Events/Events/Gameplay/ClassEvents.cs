namespace Framework.Events.Events.Gameplay
{
    // =========================================
    // ClassSelectedEvent
    // Fired when player picks a starting class.
    // UI subscribes to update class display.
    // StatSystem subscribes to apply bonuses.
    // =========================================
    public struct ClassSelectedEvent
    {
        public string ClassId;
        public string DisplayName;
        public int    Tier;

        public ClassSelectedEvent(string classId, string displayName, int tier)
        {
            ClassId     = classId;
            DisplayName = displayName;
            Tier        = tier;
        }
    }

    // =========================================
    // ClassEvolvedEvent
    // Fired when player evolves their class.
    // UI subscribes to show evolution cutscene.
    // AbilitySystem subscribes to unlock new abilities.
    // =========================================
    public struct ClassEvolvedEvent
    {
        public string OldClassId;
        public string NewClassId;
        public string NewDisplayName;
        public int    NewTier;

        public ClassEvolvedEvent(
            string oldClassId,
            string newClassId,
            string newDisplayName,
            int    newTier)
        {
            OldClassId      = oldClassId;
            NewClassId      = newClassId;
            NewDisplayName  = newDisplayName;
            NewTier         = newTier;
        }
    }
}
