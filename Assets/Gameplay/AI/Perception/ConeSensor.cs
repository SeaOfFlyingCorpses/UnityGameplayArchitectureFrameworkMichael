using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Perception
{
    // =========================================
    // ConeSensor
    // Detects targets within a cone in front
    // of the agent. Combines radius and angle.
    // No line-of-sight check — use RaycastSensor
    // if you need wall occlusion too.
    //
    // Use case: classic guard vision cones,
    //   security cameras, patrol AI
    // Examples: Metal Gear guards, Alien Isolation
    // =========================================
    public class ConeSensor : IPerceptionSensor
    {
        private readonly float     _radius;
        private readonly float     _halfAngle;    // half of total FOV in degrees
        private readonly LayerMask _targetLayer;
        private readonly Transform _self;

        private readonly List<Transform> _results = new();

        // fieldOfView = total cone angle in degrees (e.g. 90 = 45 each side)
        public ConeSensor(
            Transform self,
            float     radius,
            float     fieldOfView,
            LayerMask targetLayer)
        {
            _self        = self;
            _radius      = radius;
            _halfAngle   = fieldOfView * 0.5f;
            _targetLayer = targetLayer;
        }

        public List<Transform> Sense(Vector3 origin)
        {
            _results.Clear();

            var hits = Physics.OverlapSphere(origin, _radius, _targetLayer);

            foreach (var hit in hits)
            {
                if (hit == null) continue;

                Vector3 dir   = (hit.transform.position - origin).normalized;
                float   angle = Vector3.Angle(_self.forward, dir);

                if (angle <= _halfAngle)
                    _results.Add(hit.transform);
            }

            return _results;
        }
    }
}
