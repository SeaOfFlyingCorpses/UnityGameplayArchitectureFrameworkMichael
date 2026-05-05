using System.Collections.Generic;
using Framework.StateMachine;
using UnityEngine;

namespace Gameplay.AI.Perception
{
    public class PerceptionSystem : MonoBehaviour
    {
        public enum SensorType
        {
            OverlapSphere,  // default — detects everything in radius
            Cone,           // FOV cone in front of agent
            Raycast,        // line of sight through walls
            Trigger,        // Unity trigger collider based
        }

        public Transform   target;
        public StateContext context;

        [Header("Sensor Type")]
        public SensorType sensorType = SensorType.OverlapSphere;

        [Header("Base Settings")]
        public float     visionRadius = 10f;
        public LayerMask targetLayer;

        [Header("Cone / Raycast Settings")]
        [Tooltip("Total field of view angle in degrees (Cone and Raycast only)")]
        public float fieldOfView = 90f;

        [Header("Raycast Settings")]
        [Tooltip("Layers that block line of sight (walls, cover)")]
        public LayerMask occlusionLayer;

        [Header("Ranges")]
        public float viewRange   = 10f;
        public float attackRange = 2f;

        private IPerceptionSensor      _sensor;
        private TriggerSensorCollector _triggerCollector;

        private void Awake()
        {
            _sensor = BuildSensor();
        }

        // =========================================
        // BUILD SENSOR FROM INSPECTOR SELECTION
        // =========================================
        private IPerceptionSensor BuildSensor()
        {
            switch (sensorType)
            {
                case SensorType.Cone:
                    return new ConeSensor(transform, visionRadius, fieldOfView, targetLayer);

                case SensorType.Raycast:
                    return new RaycastSensor(
                        transform, visionRadius, targetLayer, occlusionLayer, fieldOfView);

                case SensorType.Trigger:
                    _triggerCollector = GetComponent<TriggerSensorCollector>();
                    if (_triggerCollector == null)
                    {
                        Debug.LogWarning(
                            $"[PerceptionSystem] {gameObject.name} — " +
                            $"SensorType.Trigger requires a TriggerSensorCollector component. " +
                            $"Falling back to OverlapSphere.");
                        return new OverlapSphereSensor(visionRadius, targetLayer);
                    }
                    return new TriggerSensor(_triggerCollector.InsideTargets);

                default:
                    return new OverlapSphereSensor(visionRadius, targetLayer);
            }
        }

        // =========================================
        // SWAP SENSOR AT RUNTIME
        // =========================================
        public void SetSensor(IPerceptionSensor sensor)
        {
            _sensor = sensor;
        }

        // =========================================
        // PUBLIC ENTRY POINT
        // =========================================
        public void Tick()
        {
            if (context == null || context.PerceptionContext == null)
                return;

            UpdateVision();
            UpdatePerceptionState();
        }

        private void UpdateVision()
        {
            if (_sensor == null)
                return;

            var detected = _sensor.Sense(context.Self.position);

            var perceptionCtx = context.PerceptionContext;

            if (perceptionCtx.VisibleTargets == null)
                perceptionCtx.VisibleTargets = new List<Transform>();

            perceptionCtx.VisibleTargets.Clear();
            perceptionCtx.VisibleTargets.AddRange(detected);
        }

        private void UpdatePerceptionState()
        {
            var perceptionCtx = context.PerceptionContext;
            var state         = perceptionCtx.State;

            if (state == null)
                return;

            state.CanSeeTarget =
                perceptionCtx.VisibleTargets != null &&
                perceptionCtx.VisibleTargets.Count > 0;

            if (context.Target != null)
            {
                float distance = Vector3.Distance(
                    context.Self.position,
                    context.Target.position
                );

                state.DistanceToTarget      = distance;
                state.IsTargetInAttackRange = distance <= attackRange;

                if (state.CanSeeTarget)
                    state.LastSeenTime = Time.time;
            }
            else
            {
                state.IsTargetInAttackRange = false;
                state.DistanceToTarget      = float.MaxValue;
            }
        }
    }
}