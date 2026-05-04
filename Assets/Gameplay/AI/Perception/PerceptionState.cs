namespace Gameplay.AI.Perception
{
    public class PerceptionState
    {
        public bool CanSeeTarget;
        public bool IsTargetInAttackRange;
        public float DistanceToTarget;

        public float LastSeenTime;
        public bool HasMemoryOfTarget;
        
        public float VisionStrength = 1f;
    }
}