#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using Gameplay.AI;

namespace Gameplay.Tools
{
    // =========================================
    // AIDebugOverlay
    // Runtime HUD for all registered AI agents.
    // Place on _GameSystems.
    //
    // Toggle:   F1
    // Modes:    F2 = cycle display mode
    //
    // Modes:
    //   Minimal  — state name only above agent
    //   Full     — all data in screen panel
    //   Both     — world labels + panel
    //
    // Only compiled in Editor and Development
    // builds. Zero cost in release builds.
    // =========================================
    public class AIDebugOverlay : MonoBehaviour
    {
        public enum DisplayMode { Minimal, Full, Both }

        [Header("Settings")]
        public KeyCode     toggleKey     = KeyCode.F1;
        public KeyCode     cycleModeKey  = KeyCode.F2;
        public bool        showOnStart   = false;

        [Header("Visual")]
        public int         fontSize      = 12;
        public float       worldLabelHeight = 2.5f;

        private bool        _visible;
        private DisplayMode _mode = DisplayMode.Both;

        private AIAgentRegistry _registry;

        // Styles — built once
        private GUIStyle _panelStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _healthBarBg;
        private GUIStyle _healthBarFg;
        private bool     _stylesBuilt;

        private UnityEngine.Camera _cam;

        // =========================================
        // LIFECYCLE
        // =========================================
        private void Start()
        {
            _registry = ServiceLocator.Get<AIAgentRegistry>();
            _cam      = UnityEngine.Camera.main;
            _visible  = showOnStart;
        }

        private void Update()
        {
            // Support both old and new Input System
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current[
                    ConvertKeyCode(toggleKey)].wasPressedThisFrame)
                    _visible = !_visible;

                if (_visible && UnityEngine.InputSystem.Keyboard.current[
                    ConvertKeyCode(cycleModeKey)].wasPressedThisFrame)
                    _mode = (DisplayMode)(((int)_mode + 1) % 3);
            }
#else
            if (UnityEngine.Input.GetKeyDown(toggleKey))
                _visible = !_visible;

            if (UnityEngine.Input.GetKeyDown(cycleModeKey) && _visible)
                _mode = (DisplayMode)(((int)_mode + 1) % 3);
#endif
        }

        // =========================================
        // DRAW
        // =========================================
        private void OnGUI()
        {
            if (!_visible) return;
            if (_registry == null) return;

            BuildStyles();

            var agents = _registry.GetAll();

            // Full panel — top left
            if (_mode == DisplayMode.Full ||
                _mode == DisplayMode.Both)
                DrawPanel(agents);

            // World labels — above each agent
            if (_mode == DisplayMode.Minimal ||
                _mode == DisplayMode.Both)
                DrawWorldLabels(agents);

            // Toggle hint
            GUI.Label(
                new Rect(10, Screen.height - 24, 300, 20),
                $"[{toggleKey}] Toggle  [{cycleModeKey}] Mode: {_mode}",
                _labelStyle);
        }

        // =========================================
        // PANEL — top left corner
        // =========================================
        private void DrawPanel(
            IReadOnlyList<AIAgentData> agents)
        {
            float x = 10f;
            float y = 10f;
            float w = 320f;
            float lineH = fontSize + 4f;

            // Background
            float panelH = 30f + agents.Count * (lineH * 6 + 8f);
            GUI.Box(new Rect(x - 4, y - 4, w + 8, panelH),
                    GUIContent.none, _panelStyle);

            // Header
            GUI.Label(new Rect(x, y, w, 20),
                $"AI DEBUG  [{agents.Count} agents]", _headerStyle);
            y += 24f;

            foreach (var agent in agents)
            {
                if (agent?.Context == null) continue;

                var ctx = agent.Context;

                // Agent name
                string name = agent.Transform != null
                    ? agent.Transform.name : "Unknown";

                GUI.Label(new Rect(x, y, w, lineH),
                    $"● {name}  [{ctx.Team}]", _headerStyle);
                y += lineH;

                // State
                GUI.Label(new Rect(x + 8, y, w, lineH),
                    $"State:  {ctx.CurrentStateName ?? "—"}",
                    _labelStyle);
                y += lineH;

                // LOD
                string lod = ctx.Execution != null
                    ? ctx.Execution.LOD.ToString() : "—";
                GUI.Label(new Rect(x + 8, y, w, lineH),
                    $"LOD:    {lod}    " +
                    $"Alert: {ctx.AlertLevel}",
                    _labelStyle);
                y += lineH;

                // Morale / Fear / Suppression
                GUI.Label(new Rect(x + 8, y, w, lineH),
                    $"Morale: {ctx.Morale:F2}  " +
                    $"Fear: {ctx.Fear:F2}  " +
                    $"Sup: {ctx.Suppression:F2}",
                    _labelStyle);
                y += lineH;

                // Health bar
                if (ctx.HealthData != null)
                {
                    DrawBar(
                        x + 8, y, w - 16, lineH - 2,
                        (float)ctx.HealthData.Value /
                        ctx.HealthData.MaxValue,
                        Color.green, Color.red,
                        $"HP {ctx.HealthData.Value}/{ctx.HealthData.MaxValue}");
                }
                y += lineH;

                // Squad
                GUI.Label(new Rect(x + 8, y, w, lineH),
                    $"Squad: {ctx.SquadStrategy}  " +
                    $"Target: {(ctx.Target != null ? ctx.Target.name : "none")}",
                    _labelStyle);
                y += lineH + 4f;
            }
        }

        // =========================================
        // WORLD LABELS — above each agent
        // =========================================
        private void DrawWorldLabels(
            IReadOnlyList<AIAgentData> agents)
        {
            if (_cam == null) return;

            foreach (var agent in agents)
            {
                if (agent?.Transform == null) continue;

                Vector3 worldPos  = agent.Transform.position +
                                    Vector3.up * worldLabelHeight;
                Vector3 screenPos = _cam.WorldToScreenPoint(worldPos);

                if (screenPos.z < 0) continue; // behind camera

                float sx = screenPos.x;
                float sy = Screen.height - screenPos.y;

                string stateName = agent.Context?.CurrentStateName ?? "—";

                GUI.Label(
                    new Rect(sx - 60, sy - 10, 120, 20),
                    stateName,
                    _labelStyle);
            }
        }

        // =========================================
        // BAR HELPER
        // =========================================
        private void DrawBar(
            float x, float y, float w, float h,
            float fill,
            Color fullColor, Color emptyColor,
            string label)
        {
            GUI.Box(new Rect(x, y, w, h),
                    GUIContent.none, _healthBarBg);

            float filledW = w * Mathf.Clamp01(fill);
            Color barColor = Color.Lerp(emptyColor, fullColor, fill);
            var prevColor  = GUI.color;
            GUI.color      = barColor;
            GUI.Box(new Rect(x, y, filledW, h),
                    GUIContent.none, _healthBarFg);
            GUI.color = prevColor;

            GUI.Label(new Rect(x, y, w, h), label, _labelStyle);
        }

        // =========================================
        // STYLES
        // =========================================
        // =========================================
        // Convert legacy KeyCode to new Input System
        // =========================================
#if ENABLE_INPUT_SYSTEM
        private UnityEngine.InputSystem.Key ConvertKeyCode(KeyCode key)
        {
            return key switch
            {
                KeyCode.F1  => UnityEngine.InputSystem.Key.F1,
                KeyCode.F2  => UnityEngine.InputSystem.Key.F2,
                KeyCode.F3  => UnityEngine.InputSystem.Key.F3,
                KeyCode.F4  => UnityEngine.InputSystem.Key.F4,
                KeyCode.F5  => UnityEngine.InputSystem.Key.F5,
                KeyCode.F6  => UnityEngine.InputSystem.Key.F6,
                KeyCode.F7  => UnityEngine.InputSystem.Key.F7,
                KeyCode.F8  => UnityEngine.InputSystem.Key.F8,
                KeyCode.F9  => UnityEngine.InputSystem.Key.F9,
                KeyCode.F10 => UnityEngine.InputSystem.Key.F10,
                KeyCode.F11 => UnityEngine.InputSystem.Key.F11,
                KeyCode.F12 => UnityEngine.InputSystem.Key.F12,
                _           => UnityEngine.InputSystem.Key.F1
            };
        }
#endif

        private void BuildStyles()
        {
            if (_stylesBuilt) return;
            _stylesBuilt = true;

            _panelStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = MakeTex(1, 1,
                    new Color(0f, 0f, 0f, 0.75f)) }
            };

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fontSize,
                normal    = { textColor = Color.white }
            };

            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize   = fontSize,
                fontStyle  = FontStyle.Bold,
                normal     = { textColor = Color.cyan }
            };

            _healthBarBg = new GUIStyle(GUI.skin.box)
            {
                normal = { background = MakeTex(1, 1,
                    new Color(0.2f, 0.2f, 0.2f, 1f)) }
            };

            _healthBarFg = new GUIStyle(GUI.skin.box)
            {
                normal = { background = MakeTex(1, 1, Color.white) }
            };
        }

        private Texture2D MakeTex(int w, int h, Color col)
        {
            var tex    = new Texture2D(w, h);
            var pixels = new Color[w * h];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = col;
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }
    }
}
#endif