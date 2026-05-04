using UnityEngine;

namespace Gameplay.AI.Formation
{
    public class FormationData
    {
        public FormationType Type;

        public float Spacing = 2f;
        public Transform Leader;

        public FormationData(FormationType type)
        {
            Type = type;
        }
    }
}