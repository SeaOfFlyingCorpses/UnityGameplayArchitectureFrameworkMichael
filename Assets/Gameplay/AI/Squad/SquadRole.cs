using UnityEngine;

namespace Gameplay.AI.Squad
{
    public class SquadRole
    {
        public SquadRoleType Type;
        public float AggressionMultiplier = 1f;
        public float MovementBias = 1f;

        public SquadRole(SquadRoleType type)
        {
            Type = type;

            switch (type)
            {
                case SquadRoleType.Tank:
                    AggressionMultiplier = 1.5f;
                    MovementBias = 0.8f;
                    break;

                case SquadRoleType.Flanker:
                    AggressionMultiplier = 1f;
                    MovementBias = 1.3f;
                    break;

                case SquadRoleType.Ranged:
                    AggressionMultiplier = 0.8f;
                    MovementBias = 1.6f;
                    break;

                case SquadRoleType.Support:
                    AggressionMultiplier = 0.6f;
                    MovementBias = 1f;
                    break;
            }
        }
    }
}