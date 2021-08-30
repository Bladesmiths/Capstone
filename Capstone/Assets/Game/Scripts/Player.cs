using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Bladesmiths.Capstone
{
    public class Player : Character
    {
        private WalkRunInputs walkRunInputs;
        private InputAction parryAction;

        // Reference to the Finite State Machine
        private FiniteStateMachine FSM;
        //[SerializeField] private TransitionManager playerTransitionManager;

        // Gets a reference to the player
        [SerializeField] private GameObject player;

        private void Awake()
        {
            // Create the WalkRunInputs InputAction object 
            walkRunInputs = new WalkRunInputs();

            // Creates the FSM
            FSM = new FiniteStateMachine();

            // Creates all of the states
            PlayerFSMState_MOVING move = new PlayerFSMState_MOVING();
            PlayerFSMState_IDLE idle = new PlayerFSMState_IDLE();
            PlayerFSMState_PARRY parry = new PlayerFSMState_PARRY();

            // Adds all of the possible transitions
            FSM.AddTransition(move, idle, IsIdle());
            FSM.AddTransition(idle, move, IsMoving());

            //FSM.AddTransition(idle, parry, IsBlockReleased());

            // Sets the current state
            FSM.SetCurrentState(idle);

        }

        private void OnEnable()
        {
            // Enable the controls needed for parrying
            parryAction = walkRunInputs.Player.Parry;
            parryAction.Enable();

            walkRunInputs.Player.Parry.canceled += Parry;
            walkRunInputs.Player.Parry.Enable();
        }

        private void OnDisable()
        {
            // Disable the controls needed for parrying
            parryAction.Disable();
            walkRunInputs.Player.Parry.Disable();
        }

        private void Parry(InputAction.CallbackContext obj)
        {
            Debug.Log("Parry");
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
        /// The condition fro going between the BLOCK and PARRY state
        /// Should be replaced with the input system call later on and not hard coded to right click
        /// </summary>
        /// <returns></returns>
        //public Func<bool> IsBlockReleased() => () => Input.GetMouseButtonUp(1) == true;

        private void Update()
        {
            FSM.Tick();
        }

        protected override void Attack()
        {

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