using System.Collections.Generic;
using UnityEngine;

namespace Framework.AI.Perception
{
    public interface IPerceptionContext
    {
        IPerceptionState   State          { get; set; }
        Transform          Target         { get; set; }
        List<Transform>    VisibleTargets { get; set; }
    }

    public interface IPerceptionState
    {
        bool  CanSeeTarget          { get; set; }
        bool  IsTargetInAttackRange { get; set; }
        float DistanceToTarget      { get; set; }
        float LastSeenTime          { get; set; }
        bool  HasMemoryOfTarget     { get; set; }
        float VisionStrength        { get; set; }
    }
}
