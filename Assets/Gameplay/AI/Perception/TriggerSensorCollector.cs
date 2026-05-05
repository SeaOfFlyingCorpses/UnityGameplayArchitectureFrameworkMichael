using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Perception
{
    // =========================================
    // TriggerSensorCollector
    // Maintains the list of transforms inside
    // this agent's trigger collider.
    // Pass InsideTargets to TriggerSensor.
    //
    // Attach to the same GameObject as the
    // SphereCollider trigger. Set targetLayer
    // to filter which objects are collected.
    // =========================================
    public class TriggerSensorCollector : MonoBehaviour
    {
        public LayerMask targetLayer;

        public List<Transform> InsideTargets { get; } = new();

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & targetLayer) == 0)
                return;

            if (!InsideTargets.Contains(other.transform))
                InsideTargets.Add(other.transform);
        }

        private void OnTriggerExit(Collider other)
        {
            InsideTargets.Remove(other.transform);
        }
    }
}
