using System.Collections.Generic;
using UnityEngine;

namespace Framework.AI.Squad
{
    public interface ISquadContext
    {
        SquadStrategy    CurrentStrategy { get; set; }
        Transform        CurrentTarget   { get; set; }
        List<ISquadMember> Members       { get; }
        ISquadMember     Leader          { get; set; }
        IFormationData   Formation       { get; set; }
    }

    // =========================================
    // Thin interfaces so Framework can read
    // squad member and formation data without
    // importing Gameplay types
    // =========================================
    public interface ISquadMember
    {
        int   Index   { get; set; }
        Framework.StateMachine.StateContext Context { get; }
    }

    public interface IFormationData
    {
        Transform Leader  { get; set; }
        float     Spacing { get; set; }
    }
}