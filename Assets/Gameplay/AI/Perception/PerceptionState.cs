using Framework.AI.Perception;

namespace Gameplay.AI.Perception
{
    public class PerceptionState : IPerceptionState
    {
        public bool  CanSeeTarget          { get; set; }
        public bool  IsTargetInAttackRange { get; set; }
        public float DistanceToTarget      { get; set; }
        public float LastSeenTime          { get; set; }
        public bool  HasMemoryOfTarget     { get; set; }
        public float VisionStrength        { get; set; } = 1f;
    }
}