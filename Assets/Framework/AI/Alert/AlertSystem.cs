using Framework.StateMachine;
using Framework.AI.Systems;
using UnityEngine;

namespace Framework.AI.Alert
{
    public class AlertSystem : IAISystem
    {
        // =========================================
        // CATEGORY (REQUIRED BY NEW ARCHITECTURE)
        // =========================================
        public AISystemCategory Category => AISystemCategory.Emotion;

        // =========================================
        // UPDATE LOOP
        // =========================================
        public void Update(StateContext context)
        {
            if (context == null || context.Perception == null)
                return;

            if (context.Memory == null)
                return;

            if (context.AlertContext == null)
                return;

            float gain = 0f;
            float decay = 10f * Time.deltaTime;

            // 1. PERCEPTION INCREASES ALERT
            if (context.Perception.CanSeeTarget)
                gain += 40f * Time.deltaTime;

            if (context.Perception.IsTargetInAttackRange)
                gain += 60f * Time.deltaTime;

            // 2. MEMORY MAINTAINS ALERT
            if (context.Memory.HasTargetMemory)
                gain += 20f * Time.deltaTime;

            // 3. APPLY VALUE
            context.AlertContext.Value += gain;
            context.AlertContext.Value -= decay;

            context.AlertContext.Value =
                Mathf.Clamp(context.AlertContext.Value, 0f, 100f);

            // 4. STATE MAPPING
            if (context.AlertContext.Value < 20f)
                context.AlertContext.Level = AlertLevel.Calm;

            else if (context.AlertContext.Value < 50f)
                context.AlertContext.Level = AlertLevel.Suspicious;

            else if (context.AlertContext.Value < 80f)
                context.AlertContext.Level = AlertLevel.Alert;

            else
                context.AlertContext.Level = AlertLevel.Combat;
        }
    }
}