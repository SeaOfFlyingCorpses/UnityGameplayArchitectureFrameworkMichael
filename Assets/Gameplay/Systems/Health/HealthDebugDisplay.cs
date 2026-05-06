using UnityEngine;
using Framework.Systems.Health;

namespace Gameplay.Systems.Health
{
    public class HealthDebugDisplay : MonoBehaviour
    {
        private HealthComponent    _healthComp;
        private IHealth            _health;
        private string             _healthType;
        private UnityEngine.Camera _cam;

        private void Start()
        {
            _healthComp = GetComponent<HealthComponent>();

            if (_healthComp == null)
            {
                Debug.LogWarning($"[HealthDebugDisplay] No HealthComponent on {gameObject.name}");
                return;
            }

            _health     = _healthComp.GetHealth();
            _healthType = _health.GetType().Name;
        }

        private void Update()
        {
            // Cache camera lazily — safe for prefabs
            if (_cam == null)
                _cam = UnityEngine.Camera.main;
        }

        private void OnGUI()
        {
            if (_health == null || _cam == null)
                return;

            Vector3 worldPos  = transform.position + Vector3.up * 3f;
            Vector3 screenPos = _cam.WorldToScreenPoint(worldPos);

            if (screenPos.z < 0)
                return;

            float x = screenPos.x - 80f;
            float y = Screen.height - screenPos.y;

            GUILayout.BeginArea(new Rect(x, y, 220f, 160f));

            GUILayout.Label($"{gameObject.name}");
            GUILayout.Label($"Type: {_healthType}");
            GUILayout.Label($"HP: {_health.Value} / {_health.MaxValue}");
            GUILayout.Label($"Dead: {_health.IsDead}");

            if (_health is ShieldedHealth sh)
                GUILayout.Label($"Shield: {sh.Shield} / {sh.MaxShield}");

            if (_health is RegenHealth)
                GUILayout.Label("(Regenerating)");

            if (_health is ArmouredHealth ah)
                GUILayout.Label($"Armour: {ah.Armour} | {ah.ArmourPct * 100f:F0}%");

            if (_health is SegmentedHealth seg)
                GUILayout.Label($"Segment: {seg.CurrentSegment} / {seg.TotalSegments}");

            if (_health is OvershieldHealth os)
                GUILayout.Label($"Overshield: {os.Overshield}");

            if (_health is ElementalHealth)
                GUILayout.Label("(Elemental)");

            if (_health is InvincibleHealth)
                GUILayout.Label("*** INVINCIBLE ***");

            if (_health is CompositeHealth composite)
            {
                GUILayout.Label("--- Layers ---");
                var csh = composite.GetLayer<ShieldedHealth>();
                if (csh != null) GUILayout.Label($"Shield: {csh.Shield} / {csh.MaxShield}");
                var cah = composite.GetLayer<ArmouredHealth>();
                if (cah != null) GUILayout.Label($"Armour: {cah.Armour} | {cah.ArmourPct * 100f:F0}%");
                var cseg = composite.GetLayer<SegmentedHealth>();
                if (cseg != null) GUILayout.Label($"Segment: {cseg.CurrentSegment} / {cseg.TotalSegments}");
            }

            GUILayout.EndArea();
        }
    }
}