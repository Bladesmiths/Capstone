using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class Player : Character, IDamageable
    {
        // Reference to the Finite State Machine
        private FiniteStateMachine FSM;
        //[SerializeField] private TransitionManager playerTransitionManager;

        [SerializeField] private PlayerInputsScript inputs;

        // Gets a reference to the player
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject sword;

        PlayerFSMState_MOVING move;
        PlayerFSMState_IDLE idle;
        PlayerFSMState_ATTACK attack;
        PlayerFSMState_DEATH death;
        PlayerFSMState_TAKEDAMAGE takeDamage;

        public bool isDamaged;

        private void Awake()
        {
            MaxHealth = 3;
            Health = 3;
            isDamaged = false;

            // Creates the FSM
            FSM = new FiniteStateMachine();

            // Creates all of the states
            move = new PlayerFSMState_MOVING();
            idle = new PlayerFSMState_IDLE();
            attack = new PlayerFSMState_ATTACK(this, inputs, GetComponent<Animator>(), sword);
            death = new PlayerFSMState_DEATH();
            takeDamage = new PlayerFSMState_TAKEDAMAGE(this);

            // Adds all of the possible transitions
            FSM.AddTransition(move, idle, IsIdle());
            FSM.AddTransition(idle, move, IsMoving());
            FSM.AddTransition(idle, attack, IsAttacking());
            FSM.AddTransition(move, attack, IsAttacking());
            FSM.AddTransition(attack, move, IsMoving());
            FSM.AddTransition(attack, idle, IsIdle());

            FSM.AddTransition(takeDamage, death, Alive());
            FSM.AddTransition(move, takeDamage, IsDamaged());
            FSM.AddTransition(idle, takeDamage, IsDamaged());

            FSM.AddTransition(takeDamage, move, IsAbleToDamage());
            

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
        /// The condition for having been attacked
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsDamaged() => () => isDamaged;

        /// <summary>
        /// The condition for having been attacked
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsAbleToDamage() => () => takeDamage.timer >= 1f;

        /// <summary>
        /// The condition for having been attacked
        /// </summary>
        /// <returns></returns>
        public Func<bool> Alive() => () => Health == 0;


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

        public void TakingDamage(float dmg)
        {
            Health -= dmg;
            isDamaged = true;

        }

       


    }
}