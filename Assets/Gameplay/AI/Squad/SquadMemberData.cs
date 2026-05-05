using Framework.StateMachine;
using Framework.AI.Squad;

namespace Gameplay.AI.Squad
{
    public class SquadMemberData : ISquadMember
    {
        public StateContext Context { get; set; }
        public SquadRole    Role;
        public int          Index   { get; set; }

        // Explicit ISquadMember
        Framework.StateMachine.StateContext ISquadMember.Context => Context;
    }
}