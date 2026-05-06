using UnityEngine;
using UnityEditor;
using Gameplay.AI.Perception;

namespace FrameworkEditor
{
    // =========================================
    // PerceptionSystemGizmos
    // Draws vision radius and FOV cone
    // in the Scene view for each agent.
    //
    // Colors:
    //   White  — vision radius (OverlapSphere)
    //   Yellow — FOV cone (Cone/Raycast sensor)
    //   Red    — attack range
    // =========================================
    [CustomEditor(typeof(PerceptionSystem))]
    public class PerceptionSystemEditor : Editor
    {
        private void OnSceneGUI()
        {
            var perception = (PerceptionSystem)target;

            if (perception == null) return;

            Transform t = perception.transform;

            // Vision radius — white
            Handles.color = new Color(1f, 1f, 1f, 0.15f);
            Handles.DrawSolidDisc(t.position,
                Vector3.up, perception.visionRadius);

            Handles.color = Color.white;
            Handles.DrawWireDisc(t.position,
                Vector3.up, perception.visionRadius);

            // Attack range — red
            Handles.color = new Color(1f, 0f, 0f, 0.1f);
            Handles.DrawSolidDisc(t.position,
                Vector3.up, perception.attackRange);

            Handles.color = Color.red;
            Handles.DrawWireDisc(t.position,
                Vector3.up, perception.attackRange);

            // FOV cone — yellow (Cone and Raycast only)
            if (perception.sensorType ==
                    PerceptionSystem.SensorType.Cone ||
                perception.sensorType ==
                    PerceptionSystem.SensorType.Raycast)
            {
                Handles.color = new Color(1f, 1f, 0f, 0.08f);

                float halfAngle = perception.fieldOfView * 0.5f;

                Vector3 leftDir  = Quaternion.Euler(
                    0, -halfAngle, 0) * t.forward;
                Vector3 rightDir = Quaternion.Euler(
                    0,  halfAngle, 0) * t.forward;

                Handles.DrawSolidArc(
                    t.position, Vector3.up,
                    leftDir,
                    perception.fieldOfView,
                    perception.visionRadius);

                Handles.color = Color.yellow;
                Handles.DrawLine(t.position,
                    t.position + leftDir  * perception.visionRadius);
                Handles.DrawLine(t.position,
                    t.position + rightDir * perception.visionRadius);
                Handles.DrawWireArc(
                    t.position, Vector3.up,
                    leftDir,
                    perception.fieldOfView,
                    perception.visionRadius);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(4);
            EditorGUILayout.HelpBox(
                "White = Vision Radius\n" +
                "Red   = Attack Range\n" +
                "Yellow = FOV Cone (Cone/Raycast only)",
                MessageType.None);
        }
    }
}
