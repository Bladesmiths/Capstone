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

        private void Awake()
        {
            // Creates the FSM
            FSM = new FiniteStateMachine();

            // Creates all of the states
            PlayerFSMState_MOVING move = new PlayerFSMState_MOVING();
            PlayerFSMState_IDLE idle = new PlayerFSMState_IDLE();
            PlayerFSMState_ATTACK attack = new PlayerFSMState_ATTACK(this, inputs);

            // Adds all of the possible transitions
            FSM.AddTransition(move, idle, IsIdle());
            FSM.AddTransition(idle, move, IsMoving());
            FSM.AddTransition(idle, attack, IsAttacking());
            FSM.AddTransition(move, attack, IsAttacking());
            FSM.AddTransition(attack, move, IsMoving());
            FSM.AddTransition(attack, idle, IsIdle());

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