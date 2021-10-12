using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    [CreateAssetMenu(fileName = "TransitionManager", menuName = "ScriptableObjects/TransitionManager")]
    public class TransitionManager : ScriptableObject
    {

        [EnumFlagsAttribute] PlayerCondition playerCondiitons;


        public Dictionary<Type, List<Transition>> conditionsRef = new Dictionary<Type, List<Transition>>();

        [SerializeField] private PlayerTransition moveTransition;
        [SerializeField] private PlayerTransition idleTransition;

        [NonSerialized] public GameObject player;
        public GameObject target;
        //[SerializeField] public List<Transition> allTransitions;
        PlayerFSMState_IDLE idle;

        public IState cState;

        void OnEnable()
        {
            cState = idle;
        }

        public Func<bool> IsStopped() => () => player.GetComponent<CharacterController>().velocity.magnitude == 0;
        public Func<bool> IsMoving() => () => player.GetComponent<CharacterController>().velocity.magnitude > 0;

        

        private void AddTransition(IState from, PlayerTransition transition)
        {

            if (conditionsRef.TryGetValue(from.GetType(), out List<Transition> transitions) == false)
            {
                transitions = new List<Transition>();
                conditionsRef[from.GetType()] = transitions;
            }

            transitions.Add(transition.CheckConditions());
        }

        //private void GetFlags(IState from, IState to, Func<bool> predicate)
        //{
        //    // check flags against list of transitions
        //    foreach (PlayerCondition cond in Enum.GetValues(typeof(PlayerCondition)))
        //    {
        //        foreach (Transition transition in allTransitions)
        //        {
        //            // if flag is active and transition is active then AddTransition
        //            if (cond == (PlayerCondition)transition.EValue)
        //            {
        //                AddTransition(from, to, predicate, transition);
        //            }
        //        }
        //    }
        //}
    }
}
