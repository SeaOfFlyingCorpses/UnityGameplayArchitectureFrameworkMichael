using UnityEngine;
using Framework.AI.Squad;

namespace Gameplay.AI.Formation
{
    public class FormationData : IFormationData
    {
        public FormationType Type;
        public float         Spacing { get; set; } = 2f;
        public Transform     Leader  { get; set; }

        public FormationData(FormationType type)
        {
            Type = type;
        }
    }
}