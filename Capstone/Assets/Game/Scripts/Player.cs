using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class Player : Character
    {
        // Reference to the Finite State Machine
        private FiniteStateMachine FSM;
        //[SerializeField] private TransitionManager playerTransitionManager;

        [SerializeField] private PlayerInputsScript inputs;

        // Gets a reference to the player
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject sword;

        private void Awake()
        {
            // Creates the FSM
            FSM = new FiniteStateMachine();

            // Creates all of the states
            var move = new PlayerFSMState_MOVING();
            var idle = new PlayerFSMState_IDLE();
            var attack = new PlayerFSMState_ATTACK(this, inputs, GetComponent<Animator>(), sword);
            var dodge = new PlayerFSMState_DODGE(this, inputs, GetComponent<Animator>());

            // Adds all of the possible transitions
            FSM.AddTransition(move, idle, IsIdle());
            FSM.AddTransition(idle, move, IsMoving());
            FSM.AddTransition(idle, attack, IsAttacking());
            FSM.AddTransition(move, attack, IsAttacking());
            FSM.AddTransition(attack, move, IsMoving());
            FSM.AddTransition(attack, idle, IsIdle());
            FSM.AddTransition(move, dodge, IsDodging());
            FSM.AddTransition(dodge, move, IsMoving());

            // Sets the current state
            FSM.SetCurrentState(idle);
            

        }

        /// <summary>
        /// The condition for going between the IDLE and MOVE states
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsMoving() => () => player.GetComponent<CharacterController>().velocity.magnitude != 0;

        /// <summary>
        /// The condition for going between the MOVE and IDLE states
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsIdle() => () => player.GetComponent<CharacterController>().velocity.magnitude == 0;

        /// <summary>
        /// The condition for going between MOVE/IDLE and the ATTACK states
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsAttacking() => () => inputs.attack;

        /// <summary>
        /// The condition for going from MOVE to DODGE state
        /// </summary>
        public Func<bool> IsDodging() => () => inputs.dodge;

        /// <summary>
        /// The condition for going from DODGE to MOVE state
        /// </summary>
        /// <returns></returns>
        // TODO: Should implement something like when dodging animation stops
        public Func<bool> IsDodgingStopped() => () => !inputs.dodge;



        private void FixedUpdate()
        {
            FSM.Tick();
        }

        protected override void Attack()
        {
            // Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.TransformDirection(Vector3.forward), 2) && 
        }
        protected override void ActivateAbility()
        {

        }
        protected override void Block()
        {

        }
        protected override void Parry()
        {

        }
        protected override void Dodge()
        {

        }
        protected override void SwitchWeapon(int weaponSelect)
        {

        }
        protected override void Die()
        {

        }

       


    }
}