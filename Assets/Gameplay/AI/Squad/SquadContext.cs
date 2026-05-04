using System.Collections.Generic;
using UnityEngine;
using Framework.StateMachine;
using Gameplay.AI.Threat;

namespace Gameplay.AI.Squad
{
    public class SquadContext
    {
        public List<SquadMemberData> Members = new();

        public SquadMemberData Leader;

        public SquadStrategy CurrentStrategy;
        public Transform CurrentTarget;

        public void UpdateStrategy()
        {
            if (Leader == null)
                return;

            var perception = Leader.Context.Perception;

            if (perception != null && perception.CanSeeTarget)
                CurrentStrategy = SquadStrategy.Engage;

            else if (Leader.Context.Memory != null && Leader.Context.Memory.HasTargetMemory)
                CurrentStrategy = SquadStrategy.Chase;

            else
                CurrentStrategy = SquadStrategy.Search;
        }

        public void UpdateTarget()
        {
            if (Leader == null)
                return;

            var ctx = Leader.Context;

            if (ctx.VisibleTargets != null && ctx.VisibleTargets.Count > 0)
            {
                CurrentTarget = ThreatSystem.GetBestTarget(
                    ctx.VisibleTargets,
                    ctx.Self
                );
            }
            else
            {
                CurrentTarget = ctx.Target;
            }
        }

        public void UpdateMoralInfluence()
        {
            if (Members.Count == 0)
                return;

            float totalFear = 0f;

            foreach (var m in Members)
                totalFear += m.Context.Fear;

            float avgFear = totalFear / Members.Count;

            if (avgFear > 0.7f)
                CurrentStrategy = SquadStrategy.Retreat;

            foreach (var m in Members)
            {
                if (m != Leader)
                    m.Context.Fear =
                        Mathf.Lerp(m.Context.Fear, avgFear, Time.deltaTime * 0.5f);
            }
        }

        public void AssignRoles()
        {
            if (Members.Count == 0)
                return;

            Leader = Members[0];

            for (int i = 0; i < Members.Count; i++)
            {
                var member = Members[i];

                member.Index = i;

                if (i == 0)
                    member.Role = new SquadRole(SquadRoleType.Tank);
                else if (i == 1)
                    member.Role = new SquadRole(SquadRoleType.Flanker);
                else
                    member.Role = new SquadRole(SquadRoleType.Ranged);
            }
        }

        public Vector3 GetTargetPosition()
        {
            if (CurrentTarget != null)
                return CurrentTarget.position;

            if (Leader != null &&
                Leader.Context.Memory != null &&
                Leader.Context.Memory.HasTargetMemory)
                return Leader.Context.Memory.LastKnownPosition;

            return Vector3.zero;
        }
    }
}