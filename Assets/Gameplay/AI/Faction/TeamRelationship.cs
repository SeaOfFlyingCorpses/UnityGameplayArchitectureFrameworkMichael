using Framework.AI.Faction;

namespace Gameplay.AI.Faction
{
    public static class TeamRelationship
    {
        public static bool IsHostile(Team self, Team other)
        {
            if (self == other) return false;
            if (self == Team.Player && other == Team.Ally)   return false;
            if (self == Team.Ally   && other == Team.Player) return false;
            return true;
        }
    }
}