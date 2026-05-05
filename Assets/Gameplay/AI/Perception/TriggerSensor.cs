using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Perception
{
    // =========================================
    // TriggerSensor
    // Detects targets via Unity trigger collider.
    // More performant than OverlapSphere for many
    // agents — physics broad phase handles it.
    //
    // Setup:
    //   Add a SphereCollider (Is Trigger = true)
    //   to the agent. Attach TriggerSensorCollector
    //   to the same GameObject and pass its list
    //   into TriggerSensor constructor.
    //
    // Use case: large numbers of agents where
    //   Physics.OverlapSphere per agent is too costly
    // =========================================
    public class TriggerSensor : IPerceptionSensor
    {
        private readonly List<Transform> _inside;

        public TriggerSensor(List<Transform> insideList)
        {
            _inside = insideList;
        }

        public List<Transform> Sense(Vector3 origin)
        {
            return _inside;
        }
    }
}
