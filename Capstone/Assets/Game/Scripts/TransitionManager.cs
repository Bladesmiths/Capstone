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

        [SerializeField] [EnumFlagsAttribute] PlayerCondition playerCondiitons;

        public List<Func<bool>> Conditions { get; set; }
        public Dictionary<Type, List<Transition>> conditionsRef;

        public GameObject player;
        public GameObject target;

        public IState cState;

        void OnEnable()
        {
            conditionsRef = new Dictionary<Type, List<Transition>>();
            
            PlayerFSMState_MOVING move = new PlayerFSMState_MOVING(target);
            PlayerFSMState_IDLE idle = new PlayerFSMState_IDLE(target, player);

            AddTransition(move, idle, IsStopped());
            AddTransition(idle, move, IsMoving());

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


        private void AddTransition(IState from, IState to, Func<bool> predicate)
        {
            if (conditionsRef.TryGetValue(from.GetType(), out List<Transition> transitions) == false)
            {
                transitions = new List<Transition>();
                conditionsRef[from.GetType()] = transitions;
            }

            transitions.Add(new Transition(to, from, predicate));
        }


    }
}
