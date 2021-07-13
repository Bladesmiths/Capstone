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
        // The 'to' states the user can select
        [SerializeField] public PlayerCondition eValue;

        // The default 'from' state
        public PlayerFSMState from;

        // Contains the possible 'to' states and the conditions for getting there
        public Dictionary<PlayerFSMState, Func<bool>> toConditions = new Dictionary<PlayerFSMState, Func<bool>>();
        public List<Transition> stateTransitions;

        public PlayerCondition EValue { get; }

        /// <summary>
        /// Checks to see which enums the user selected 
        /// are in the possible transitions
        /// </summary>
        /// <returns></returns>
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
