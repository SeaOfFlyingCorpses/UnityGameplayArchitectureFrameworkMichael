using System.Collections.Generic;
using Framework.StateMachine;
using UnityEngine;

namespace Gameplay.AI.Perception
{
    public class PerceptionSystem : MonoBehaviour
    {
        public Transform target;
        public StateContext context;

        public float visionRadius = 10f;
        public LayerMask targetLayer;

        public float viewRange = 10f;
        public float attackRange = 2f;

        private readonly List<Transform> _tempTargets = new();

        // =========================================
        // 🔥 PUBLIC ENTRY POINT (USED BY AIController)
        // =========================================
        public void Tick()
        {
            if (context == null || context.PerceptionContext == null)
                return;

            UpdateVision();
            UpdatePerceptionState();
        }

        // =========================================
        // SENSOR LAYER (PHYSICS ONLY)
        // =========================================
        private void UpdateVision()
        {
            _tempTargets.Clear();

            var hits = Physics.OverlapSphere(
                context.Self.position,
                visionRadius,
                targetLayer
            );

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] != null)
                    _tempTargets.Add(hits[i].transform);
            }

            var perceptionCtx = context.PerceptionContext;

            if (perceptionCtx.VisibleTargets == null)
                perceptionCtx.VisibleTargets = new List<Transform>();

            perceptionCtx.VisibleTargets.Clear();
            perceptionCtx.VisibleTargets.AddRange(_tempTargets);
        }

        // =========================================
        // BRAIN STATE UPDATE
        // =========================================
        private void UpdatePerceptionState()
        {
            var perceptionCtx = context.PerceptionContext;
            var state = perceptionCtx.State;

            if (state == null)
                return;

            // ✅ FIX: use PerceptionContext ONLY
            state.CanSeeTarget =
                perceptionCtx.VisibleTargets != null &&
                perceptionCtx.VisibleTargets.Count > 0;

            if (context.Target != null)
            {
                float distance = Vector3.Distance(
                    context.Self.position,
                    context.Target.position
                );

                state.DistanceToTarget = distance;
                state.IsTargetInAttackRange = distance <= attackRange;

                // memory hook
                if (state.CanSeeTarget)
                    state.LastSeenTime = Time.time;
            }
            else
            {
                state.IsTargetInAttackRange = false;
                state.DistanceToTarget = float.MaxValue;
            }
        }
    }
}