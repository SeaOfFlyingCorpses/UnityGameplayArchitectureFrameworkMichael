using System.Collections.Generic;
using UnityEngine;
using Framework.StateMachine;
using Framework.AI.Squad;
using Gameplay.AI.Threat;
using Gameplay.AI.Formation;

namespace Gameplay.AI.Squad
{
    public class SquadContext : ISquadContext
    {
        // ISquadContext implementation
        public SquadStrategy CurrentStrategy { get; set; }
        public Transform     CurrentTarget   { get; set; }
        public ISquadMember  Leader          { get; set; }
        public IFormationData Formation      { get; set; }

        // Typed access for Gameplay code
        public List<SquadMemberData> Members     = new();
        public SquadMemberData       TypedLeader => Leader as SquadMemberData;
        public FormationData         TypedFormation => Formation as FormationData;

        // ISquadContext.Members
        public List<ISquadMember> Members_Interface
        {
            get
            {
                var list = new List<ISquadMember>();
                foreach (var m in Members) list.Add(m);
                return list;
            }
        }

        List<ISquadMember> ISquadContext.Members => Members_Interface;

        public void UpdateStrategy()
        {
            if (TypedLeader == null) return;

            var perception = TypedLeader.Context.Perception;

            if (perception != null && perception.CanSeeTarget)
                CurrentStrategy = SquadStrategy.Engage;
            else if (TypedLeader.Context.Memory != null && TypedLeader.Context.Memory.HasTargetMemory)
                CurrentStrategy = SquadStrategy.Chase;
            else
                CurrentStrategy = SquadStrategy.Search;
        }

        public void UpdateTarget()
        {
            if (TypedLeader == null) return;

            var ctx = TypedLeader.Context;

            if (ctx.VisibleTargets != null && ctx.VisibleTargets.Count > 0)
                CurrentTarget = ThreatSystem.GetBestTarget(ctx.VisibleTargets, ctx.Self);
            else
                CurrentTarget = ctx.Target;
        }

        public void UpdateMoralInfluence()
        {
            if (Members.Count == 0) return;

            float totalFear = 0f;
            foreach (var m in Members) totalFear += m.Context.Fear;

            float avgFear = totalFear / Members.Count;

            if (avgFear > 0.7f)
                CurrentStrategy = SquadStrategy.Retreat;

            foreach (var m in Members)
                if (m != TypedLeader)
                    m.Context.Fear = UnityEngine.Mathf.Lerp(
                        m.Context.Fear, avgFear, UnityEngine.Time.deltaTime * 0.5f);
        }

        public void AssignRoles()
        {
            if (Members.Count == 0) return;

            Leader = Members[0];

            if (TypedFormation != null)
                TypedFormation.Leader = TypedLeader.Context.Self;

            for (int i = 0; i < Members.Count; i++)
            {
                var member = Members[i];
                member.Index = i;

                if (i == 0)      member.Role = new SquadRole(SquadRoleType.Tank);
                else if (i == 1) member.Role = new SquadRole(SquadRoleType.Flanker);
                else             member.Role = new SquadRole(SquadRoleType.Ranged);
            }
        }

        public Vector3 GetTargetPosition()
        {
            if (CurrentTarget != null)
                return CurrentTarget.position;

            if (TypedLeader?.Context.Memory != null &&
                TypedLeader.Context.Memory.HasTargetMemory)
                return TypedLeader.Context.Memory.LastKnownPosition;

            return Vector3.zero;
        }
    }
}