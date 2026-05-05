using System.Collections.Generic;
using UnityEngine;
using Framework.AI.Perception;

namespace Gameplay.AI.Perception
{
    public class PerceptionContext : IPerceptionContext
    {
        public IPerceptionState  State          { get; set; }
        public Transform         Target         { get; set; }
        public List<Transform>   VisibleTargets { get; set; }
    }
}