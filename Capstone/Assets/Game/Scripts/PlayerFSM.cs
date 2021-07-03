using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bladesmiths.Capstone
{
    public class PlayerFSM : FiniteStateMachine
    {
        List<PlayerTransition> playerTransitionObjects;
        //Dictionary<CharacterState, List<PlayerTransition>> filteredPlayerTransitions;




        public PlayerFSM() : base()
        {

        }

        public override void AddTransition(IState from, IState to, Func<bool> predicate)
        {
            //states.Add((int)state.ID, state);

        }

        //public Dictionary<CharacterState, List<PlayerTransition>> FilteredTransitions()
        //{


        //}

    }
}
