namespace Framework.AI.BehaviourTree
{
    // =========================================
    // NodeStatus
    // Return value from every node's Tick().
    //
    // Running  — node is in progress, call again
    // Success  — node completed successfully
    // Failure  — node failed
    // =========================================
    public enum NodeStatus
    {
        Running,
        Success,
        Failure
    }

    // =========================================
    // IBehaviourNode
    // Contract for every node in the tree.
    // Composite, Decorator, and Leaf nodes
    // all implement this.
    //
    // OnEnter — called once when node activates
    // Tick    — called every frame while Running
    // OnExit  — called once when node completes
    // =========================================
    public interface IBehaviourNode
    {
        string     Name   { get; }
        NodeStatus Tick   (Framework.StateMachine.StateContext context);
        void       OnEnter(Framework.StateMachine.StateContext context);
        void       OnExit (Framework.StateMachine.StateContext context,
                           NodeStatus status);
    }
}
