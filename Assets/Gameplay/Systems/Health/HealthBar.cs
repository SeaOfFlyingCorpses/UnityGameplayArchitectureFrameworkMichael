using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Systems.Health
{
    public class HealthBar : MonoBehaviour
    {
        public HealthComponent health;
        public Image fill;

        private void Update()
        {
            if (health == null || fill == null)
                return;

            float hp = health.GetHealth().Value;
            float max = health.GetHealth().MaxValue;

            float ratio = Mathf.Clamp01(hp / max);

            fill.transform.localScale = new Vector3(ratio, 1f, 1f);

            Debug.Log($"HP: {hp}");
        }
    }
}