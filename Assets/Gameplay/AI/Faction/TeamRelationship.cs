namespace Gameplay.AI.Faction
{
    // =========================================
    // TeamRelationship
    // Defines hostility between teams.
    // Used by CombatState to decide whether
    // to attack a target.
    //
    // Rules:
    //   Player  vs Enemy  → hostile
    //   Ally    vs Enemy  → hostile
    //   Player  vs Ally   → friendly (no attack)
    //   Same team         → always friendly
    // =========================================
    public static class TeamRelationship
    {
        public static bool IsHostile(Team self, Team other)
        {
            if (self == other)
                return false;

            // Player and Ally are on the same side
            if (self == Team.Player && other == Team.Ally)   return false;
            if (self == Team.Ally   && other == Team.Player) return false;

            // Everything else is hostile
            return true;
        }
    }
}