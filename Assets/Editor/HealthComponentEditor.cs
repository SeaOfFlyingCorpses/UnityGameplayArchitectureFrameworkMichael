using UnityEngine;
using UnityEditor;
using Gameplay.Systems.Health;

namespace FrameworkEditor
{
    [CustomEditor(typeof(HealthComponent))]
    public class HealthComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying) return;

            var comp   = (HealthComponent)target;
            var health = comp.GetHealth();

            if (health == null) return;

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField(
                "Runtime Health", EditorStyles.boldLabel);

            // Health bar
            float ratio = (float)health.Value / health.MaxValue;
            var   rect  = EditorGUILayout.GetControlRect(
                GUILayout.Height(20));

            EditorGUI.DrawRect(rect,
                new Color(0.2f, 0.2f, 0.2f));

            var fill = new Rect(
                rect.x, rect.y,
                rect.width * ratio, rect.height);

            Color barColor = ratio > 0.5f ? Color.green
                           : ratio > 0.25f ? Color.yellow
                           : Color.red;

            EditorGUI.DrawRect(fill, barColor);

            GUI.Label(rect,
                $"  {health.Value} / {health.MaxValue}  ({ratio * 100f:F0}%)",
                new GUIStyle(EditorStyles.boldLabel)
                {
                    normal = { textColor = Color.white }
                });

            EditorGUILayout.LabelField(
                $"Type: {health.GetType().Name}");
            EditorGUILayout.LabelField(
                $"Dead: {health.IsDead}");

            // Shield info
            if (health is ShieldedHealth sh)
            {
                float shieldRatio = (float)sh.Shield / sh.MaxShield;
                var   sr = EditorGUILayout.GetControlRect(
                    GUILayout.Height(14));

                EditorGUI.DrawRect(sr,
                    new Color(0.2f, 0.2f, 0.2f));
                EditorGUI.DrawRect(
                    new Rect(sr.x, sr.y,
                        sr.width * shieldRatio, sr.height),
                    Color.cyan);

                GUI.Label(sr,
                    $"  Shield: {sh.Shield} / {sh.MaxShield}",
                    new GUIStyle(EditorStyles.miniLabel)
                    {
                        normal = { textColor = Color.white }
                    });
            }

            if (Application.isPlaying)
                Repaint();
        }
    }
}
