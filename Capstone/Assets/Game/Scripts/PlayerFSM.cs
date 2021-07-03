using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    public class PlayerFSM : FiniteStateMachine
    {
        List<PlayerTransition> playerTransitionObjects;
        Dictionary<CharacterState, List<PlayerTransition>> filteredPlayerTransitions;




        public PlayerFSM() : base()
        {

        }

        public override void Tick()
        {
            PlayerTransition transition = GetTransition();
            if (transition != null)
                SetCurrentState(transition.To);

            if (currentState != null)
            {
                currentState.Tick();
            }

        }


        public void AddTransition(PlayerFSMState from, PlayerFSMState to, Func<bool> predicate)
        {
            if (filteredPlayerTransitions.TryGetValue(from.ID, out List<PlayerTransition> transitions) == false)
            {
                transitions = new List<PlayerTransition>();
                filteredPlayerTransitions[from.ID] = transitions;
            }

            transitions.Add(new PlayerTransition(to, from, predicate));

        }

    

        public Dictionary<CharacterState, List<PlayerTransition>> FilteredTransitions()
        {


        }

        public PlayerTransition GetTransition()
        {
            foreach (PlayerTransition transition in playerTransitionObjects)
                if (transition.Condition())
                    return transition;

            return null;
        }

    }
}
