using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bladesmiths.Capstone.Enums;


namespace Bladesmiths.Capstone
{
    [CreateAssetMenu(fileName = "PlayerTransition", menuName = "ScriptableObjects/PlayerTransition")]
    public class PlayerTransition : ScriptableObject
    {
        [SerializeField] public PlayerCondition eValue;

        public PlayerFSMState from;
        public Dictionary<PlayerFSMState, Func<bool>> toConditions;
        public List<Transition> stateTransitions;


        public PlayerCondition EValue { get; }
        public Dictionary<PlayerFSMState, Func<bool>> ToConditions { get; set; }

        public Transition CheckConditions()
        {
            foreach (KeyValuePair<PlayerFSMState, Func<bool>> state in toConditions)
            {
                foreach (PlayerCondition cond in Enum.GetValues(typeof(PlayerCondition)))
                {
                    if (state.Key.ID == cond)
                    {
                        // Add to a list and then return at the end
                        return new Transition(from, state.Key, state.Value);
                    
                    }                    
                    
                }

            }

            return null;

        }



        //public PlayerTransition(IState to, IState from, Func<bool> condition)
        //{
        //    To = to;
        //    From = from;
        //    Condition = condition;

        //}

       



    }
}
