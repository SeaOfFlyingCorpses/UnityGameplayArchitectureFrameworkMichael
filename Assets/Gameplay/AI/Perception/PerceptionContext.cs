using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Perception
{
    public class PerceptionContext
    {
        public PerceptionState State;

        public Transform Target;

        public List<Transform> VisibleTargets;
    }
}