using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Perception
{
    // =========================================
    // OverlapSphereSensor
    // Default sensor — uses Physics.OverlapSphere.
    // Same behaviour as before, now swappable.
    //
    // Swap for TriggerSensor if you want trigger-based
    // detection. Swap for MockSensor in tests.
    // =========================================
    public class OverlapSphereSensor : IPerceptionSensor
    {
        private readonly float     _radius;
        private readonly LayerMask _targetLayer;
        private readonly List<Transform> _results = new();

        public OverlapSphereSensor(float radius, LayerMask targetLayer)
        {
            _radius      = radius;
            _targetLayer = targetLayer;
        }

        public List<Transform> Sense(Vector3 origin)
        {
            _results.Clear();

            var hits = Physics.OverlapSphere(origin, _radius, _targetLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] != null)
                    _results.Add(hits[i].transform);
            }

            return _results;
        }
    }
}
