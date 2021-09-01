using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;

using StarterAssets;

namespace Bladesmiths.Capstone
{
    public class Player : Character
    {
        // Reference to the Finite State Machine
        private FiniteStateMachine FSM;
        //[SerializeField] private TransitionManager playerTransitionManager;

        // Gets a reference to the player
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject parryDetector;

        private StarterAssetsInputs inputs;
        private PlayerFSMState_MOVING move;
        private PlayerFSMState_PARRY parry;
        private PlayerFSMState_IDLE idle;

        private void Awake()
        {
            inputs = gameObject.GetComponent<StarterAssetsInputs>();

            // Creates the FSM
            FSM = new FiniteStateMachine();

            // Creates all of the states
            move = new PlayerFSMState_MOVING(inputs);
            idle = new PlayerFSMState_IDLE();
            parry = new PlayerFSMState_PARRY(parryDetector);

            // Adds all of the possible transitions
            FSM.AddTransition(move, idle, IsIdle());
            FSM.AddTransition(idle, move, IsMoving());

            FSM.AddTransition(idle, parry, IsBlockReleased());
            FSM.AddTransition(move, parry, IsBlockReleased());

            FSM.AddTransition(parry, idle, IsReleased());

            // Sets the current state
            FSM.SetCurrentState(idle);

        }

        /// <summary>
        /// The condition for going between the IDLE and MOVE states
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsMoving() => () => inputs.move != Vector2.zero;

        /// <summary>
        /// The condition for going between the MOVE and IDLE states
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsIdle() => () => move.timer >= 0.5f;

        /// <summary>
        /// The condition fro going between the BLOCK and PARRY state
        /// Should be replaced with the input system call later on and not hard coded to right click
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsBlockReleased() => () => inputs.parry == true;


        public Func<bool> IsReleased() => () => parry.timer >= 0.5;

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