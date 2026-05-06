using UnityEngine;
using UnityEditor;
using Gameplay.AI;

namespace FrameworkEditor
{
    // =========================================
    // AIControllerEditor
    // Custom Inspector for AIController.
    // Shows live runtime data while playing:
    //   - Current state name
    //   - LOD level
    //   - Fear, morale, suppression
    //   - Alert level
    //   - Target name
    //   - Active systems count
    // =========================================
    [CustomEditor(typeof(AIController))]
    public class AIControllerEditor : Editor
    {
        private bool _showRuntimeData = true;

        public override void OnInspectorGUI()
        {
            // Draw default inspector first
            DrawDefaultInspector();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "Runtime data available during Play Mode.",
                    MessageType.Info);
                return;
            }

            var controller = (AIController)target;
            var context    = GetContext(controller);

            if (context == null)
            {
                EditorGUILayout.HelpBox(
                    "No StateContext found.",
                    MessageType.Warning);
                return;
            }

            EditorGUILayout.Space(8);

            _showRuntimeData = EditorGUILayout.Foldout(
                _showRuntimeData, "Runtime State", true,
                EditorStyles.foldoutHeader);

            if (!_showRuntimeData) return;

            EditorGUI.indentLevel++;

            // LOD
            DrawReadOnly("LOD Level",
                context.Execution?.LOD.ToString() ?? "—");

            // Emotion
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Emotion",
                EditorStyles.boldLabel);

            DrawBar("Fear",    context.Fear,    Color.red);
            DrawBar("Morale",  context.Morale,  Color.green);
            DrawBar("Suppression", context.Suppression, Color.yellow);

            // Alert
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Perception",
                EditorStyles.boldLabel);

            DrawReadOnly("Alert Level",
                context.AlertLevel.ToString());
            DrawReadOnly("Alert Value",
                context.AlertValue.ToString("F2"));

            if (context.Target != null)
                DrawReadOnly("Target", context.Target.name);
            else
                DrawReadOnly("Target", "None");

            DrawReadOnly("Can See Target",
                context.Perception?.CanSeeTarget.ToString() ?? "—");

            DrawReadOnly("Distance To Target",
                context.Perception?.DistanceToTarget.ToString("F1") ?? "—");

            // Director
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Director",
                EditorStyles.boldLabel);

            DrawBar("Intensity",
                context.DirectorIntensity, Color.cyan);

            // Squad
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Squad",
                EditorStyles.boldLabel);

            DrawReadOnly("Strategy",
                context.SquadStrategy.ToString());

            EditorGUI.indentLevel--;

            // Force repaint while playing
            if (Application.isPlaying)
                Repaint();
        }

        private void DrawReadOnly(string label, string value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label,
                GUILayout.Width(130));
            EditorGUILayout.LabelField(value,
                EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawBar(string label, float value, Color color)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(130));

            var rect = EditorGUILayout.GetControlRect(
                GUILayout.Height(14));

            // Background
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));

            // Fill
            var fill = new Rect(rect.x, rect.y,
                rect.width * Mathf.Clamp01(value), rect.height);
            EditorGUI.DrawRect(fill, color);

            // Value text
            GUI.Label(rect,
                $"  {value:F2}",
                new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = Color.white }
                });

            EditorGUILayout.EndHorizontal();
        }

        // =========================================
        // Read context via reflection — avoids
        // making private fields public just for editor
        // =========================================
        private Framework.StateMachine.StateContext GetContext(
            AIController controller)
        {
            var field = typeof(AIController).GetField(
                "_context",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            return field?.GetValue(controller)
                as Framework.StateMachine.StateContext;
        }
    }
}
