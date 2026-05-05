using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Perception
{
    // =========================================
    // RaycastSensor
    // First finds candidates with OverlapSphere,
    // then raycasts to each — only returns targets
    // with a clear line of sight.
    //
    // Use case: stealth games, snipers, guards
    //   that can be blocked by walls/cover
    // Examples: Metal Gear, Splinter Cell, Hitman
    // =========================================
    public class RaycastSensor : IPerceptionSensor
    {
        private readonly float            _radius;
        private readonly LayerMask        _targetLayer;
        private readonly LayerMask        _occlusionLayer;
        private readonly float            _fieldOfView;    // degrees, 0 = no FOV limit
        private readonly Transform        _self;

        private readonly List<Transform>  _results = new();

        public RaycastSensor(
            Transform self,
            float     radius,
            LayerMask targetLayer,
            LayerMask occlusionLayer,
            float     fieldOfView = 0f)
        {
            _self           = self;
            _radius         = radius;
            _targetLayer    = targetLayer;
            _occlusionLayer = occlusionLayer;
            _fieldOfView    = fieldOfView;
        }

        public List<Transform> Sense(Vector3 origin)
        {
            _results.Clear();

            var hits = Physics.OverlapSphere(origin, _radius, _targetLayer);

            foreach (var hit in hits)
            {
                if (hit == null) continue;

                Transform target = hit.transform;
                Vector3   dir    = (target.position - origin);

                // Field of view check
                if (_fieldOfView > 0f)
                {
                    float angle = Vector3.Angle(_self.forward, dir.normalized);
                    if (angle > _fieldOfView * 0.5f)
                        continue;
                }

                // Line of sight check
                if (Physics.Raycast(origin, dir.normalized, out var rayHit, _radius, _occlusionLayer))
                {
                    // Something blocked the ray — not visible
                    if (rayHit.transform != target)
                        continue;
                }

                _results.Add(target);
            }

            return _results;
        }
    }
}
