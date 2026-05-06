using UnityEngine;
using UnityEditor;
using Gameplay.AI.Squad;
using Gameplay.AI.Formation;

namespace FrameworkEditor
{
    [CustomEditor(typeof(SquadSystem))]
    public class SquadSystemEditor : Editor
    {
        private void OnSceneGUI()
        {
            var squadSystem = (SquadSystem)target;
            if (squadSystem == null) return;

            DrawSquadGizmos(squadSystem.EnemySquad,  Color.red,    "Enemy");
            DrawSquadGizmos(squadSystem.AllySquad,   Color.green,  "Ally");
            DrawSquadGizmos(squadSystem.PlayerSquad, Color.cyan,   "Player");
        }

        private void DrawSquadGizmos(
            SquadContext squad, Color color, string label)
        {
            if (squad == null || squad.Members.Count == 0) return;

            var formation = squad.TypedFormation;
            if (formation == null) return;

            // Use typed leader transform or first member as fallback
            Transform leaderTransform = formation.Leader;
            if (leaderTransform == null && squad.TypedLeader?.Context?.Self != null)
                leaderTransform = squad.TypedLeader.Context.Self;
            if (leaderTransform == null) return;

            // Update formation leader reference
            formation.Leader = leaderTransform;

            Vector3 leaderPos = leaderTransform.position;

            // Leader marker
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(leaderPos, Vector3.up, 0.4f);
            Handles.Label(leaderPos + Vector3.up * 0.6f,
                $"{label} Leader");

            // Formation slots — draw actual members + 1 preview
            int slotCount = squad.Members.Count + 2;

            for (int i = 1; i < slotCount; i++)
            {
                Vector3 offset  = FormationSystem.GetOffset(i, formation);
                Vector3 slotPos = leaderPos + offset;

                bool occupied = i <= squad.Members.Count;

                Handles.color = occupied
                    ? new Color(color.r, color.g, color.b, 0.5f)
                    : new Color(1f, 1f, 1f, 0.15f);

                Handles.DrawSolidDisc(slotPos, Vector3.up, 0.3f);

                Handles.color = occupied ? color : Color.white;
                Handles.DrawWireDisc(slotPos, Vector3.up, 0.3f);
                Handles.Label(slotPos + Vector3.up * 0.4f,
                    $"Slot {i}");

                // Dotted line
                Handles.color = new Color(
                    color.r, color.g, color.b, 0.4f);
                Handles.DrawDottedLine(leaderPos, slotPos, 4f);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var squadSystem = (SquadSystem)target;

            EditorGUILayout.Space(8);

            DrawSquadInfo("Enemy Squad",  squadSystem.EnemySquad);
            EditorGUILayout.Space(4);
            DrawSquadInfo("Ally Squad",   squadSystem.AllySquad);
            EditorGUILayout.Space(4);
            DrawSquadInfo("Player Squad", squadSystem.PlayerSquad);

            if (Application.isPlaying)
                Repaint();
        }

        private void DrawSquadInfo(string label, SquadContext squad)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            if (!Application.isPlaying)
            {
                EditorGUILayout.LabelField("(Play to see live data)");
                EditorGUI.indentLevel--;
                return;
            }

            EditorGUILayout.LabelField(
                $"Members: {squad.Members.Count}");
            EditorGUILayout.LabelField(
                $"Strategy: {squad.CurrentStrategy}");
            string leaderName = "None";
            try
            {
                if (squad.TypedLeader?.Context?.Self != null)
                    leaderName = squad.TypedLeader.Context.Self.name;
            }
            catch { leaderName = "Destroyed"; }
            EditorGUILayout.LabelField($"Leader: {leaderName}");
            EditorGUILayout.LabelField(
                $"Formation: {squad.TypedFormation?.Type.ToString() ?? "None"}");

            EditorGUI.indentLevel--;
        }
    }
}