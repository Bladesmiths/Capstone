using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    //[CreateAssetMenu(fileName = "TransitionManager", menuName = "ScriptableObjects/TransitionManager")]
    public class TransitionManager : MonoBehaviour
    {

        [EnumFlagsAttribute] PlayerCondition playerCondiitons;

        
        public Dictionary<Type, List<Transition>> conditionsRef;

        [SerializeField] private PlayerTransition moveTransition;
        [SerializeField] private PlayerTransition idleTransition;

        public GameObject player;
        public GameObject target;
        //[SerializeField] public List<Transition> allTransitions;
        PlayerFSMState_MOVING move;
        PlayerFSMState_IDLE idle;

        public IState cState;

        void Start()
        {
            //conditionsRef = new Dictionary<Type, List<Transition>>();
            move = new PlayerFSMState_MOVING(target);
            idle = new PlayerFSMState_IDLE(target, player);
            move.ID = PlayerCondition.F_Moving;
            idle.ID = PlayerCondition.F_Idle;

            idleTransition.ToConditions.Add(move, IsMoving());
            moveTransition.ToConditions.Add(idle, IsStopped());

            AddTransition(move, moveTransition);
            AddTransition(idle, idleTransition);

            //AddTransition(move, idle, IsStopped());
            //AddTransition(idle, move, IsMoving());



            cState = idle;

            //Func<bool> IsFar() => () => target != null &&
            //       Vector3.Distance(player.transform.position, target.transform.position) >= 1f;
            //Func<bool> IsClose() => () => target != null &&
            //       Vector3.Distance(player.transform.position, target.transform.position) < 1f;


        }

        public Func<bool> IsStopped() => () => target != null &&
               Vector3.Distance(player.transform.position, target.transform.position) >= 2f;
        public Func<bool> IsMoving() => () => target != null &&
               Vector3.Distance(player.transform.position, target.transform.position) < 2f;

        

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
