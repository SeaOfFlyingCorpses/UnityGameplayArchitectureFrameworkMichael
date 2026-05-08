using Framework.AI.BehaviourTree;

namespace Gameplay.AI.BehaviourTree
{
    // =========================================
    // InverterNode
    // Flips Success ↔ Failure.
    // Running passes through unchanged.
    //
    // Use for: "Do X only if condition is FALSE"
    // Example: Inverter(CanSeeTarget) = true when
    //          target is NOT visible
    // =========================================
    public class InverterNode : IBehaviourNode
    {
        public string Name { get; }

        private readonly IBehaviourNode _child;

        public InverterNode(IBehaviourNode child,
                            string name = "Inverter")
        {
            _child = child;
            Name   = name;
        }

        public void OnEnter(Framework.StateMachine.StateContext ctx) { }

        public NodeStatus Tick(Framework.StateMachine.StateContext ctx)
        {
            var status = _child.Tick(ctx);

            return status switch
            {
                NodeStatus.Success => NodeStatus.Failure,
                NodeStatus.Failure => NodeStatus.Success,
                _                  => NodeStatus.Running
            };
        }

        public void OnExit(Framework.StateMachine.StateContext ctx,
                           NodeStatus status) { }
    }

    // =========================================
    // RepeaterNode
    // Repeats child N times or infinitely.
    // Returns Failure if child fails.
    // Returns Success after N completions.
    //
    // count = -1 → repeat forever (until failure)
    //
    // Use for: patrol loops, periodic checks
    // =========================================
    public class RepeaterNode : IBehaviourNode
    {
        public string Name { get; }

        private readonly IBehaviourNode _child;
        private readonly int            _count;
        private int                     _executed;

        public RepeaterNode(IBehaviourNode child,
                            int    count = -1,
                            string name  = "Repeater")
        {
            _child = child;
            _count = count;
            Name   = name;
        }

        public void OnEnter(Framework.StateMachine.StateContext ctx)
        {
            _executed = 0;
        }

        public NodeStatus Tick(Framework.StateMachine.StateContext ctx)
        {
            var status = _child.Tick(ctx);

            if (status == NodeStatus.Failure)
                return NodeStatus.Failure;

            if (status == NodeStatus.Success)
            {
                _executed++;

                if (_count > 0 && _executed >= _count)
                    return NodeStatus.Success;

                // Reset child for next repetition
                _child.OnEnter(ctx);
            }

            return NodeStatus.Running;
        }

        public void OnExit(Framework.StateMachine.StateContext ctx,
                           NodeStatus status)
        {
            _executed = 0;
        }
    }

    // =========================================
    // CooldownNode
    // Prevents child from running more than
    // once per cooldown period.
    // Returns Failure during cooldown.
    //
    // Use for: rate-limit attacks, ability use
    // =========================================
    public class CooldownNode : IBehaviourNode
    {
        public string Name { get; }

        private readonly IBehaviourNode _child;
        private readonly float          _cooldown;
        private float                   _lastTime;

        public CooldownNode(IBehaviourNode child,
                            float  cooldown = 1f,
                            string name     = "Cooldown")
        {
            _child    = child;
            _cooldown = cooldown;
            Name      = name;
        }

        public void OnEnter(Framework.StateMachine.StateContext ctx) { }

        public NodeStatus Tick(Framework.StateMachine.StateContext ctx)
        {
            if (UnityEngine.Time.time < _lastTime + _cooldown)
                return NodeStatus.Failure;

            var status = _child.Tick(ctx);

            if (status == NodeStatus.Success)
                _lastTime = UnityEngine.Time.time;

            return status;
        }

        public void OnExit(Framework.StateMachine.StateContext ctx,
                           NodeStatus status) { }
    }
}
