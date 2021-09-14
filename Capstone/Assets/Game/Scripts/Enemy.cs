using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class Enemy : Character
    {
        // Reference to the Finite State Machine
        private FiniteStateMachine FSM;
        //[SerializeField] private TransitionManager playerTransitionManager;

        // Gets a reference to the player
        // Will be used for finding the player in the world
        [SerializeField] private Player player;

        private bool damaged = false;
        private float timer = 0f;
        private float attackTimer = 0f;

        private void Awake()
        {
            // Creates the FSM
            FSM = new FiniteStateMachine();

            // Creates all of the states
            //PlayerFSMState_MOVING move = new PlayerFSMState_MOVING();
            //PlayerFSMState_IDLE idle = new PlayerFSMState_IDLE();

            EnemyFSMState_SEEK seek = new EnemyFSMState_SEEK(player, this);
            EnemyFSMState_IDLE idle = new EnemyFSMState_IDLE();

            // Adds all of the possible transitions
            //FSM.AddTransition(seek, idle, IsIdle());
            //FSM.AddTransition(idle, seek, IsClose());
           
            // Sets the current state
            //FSM.SetCurrentState(idle);




        }

        /// <summary>
        /// The condition for going between the IDLE and MOVE states
        /// </summary>
        /// <returns></returns>
        //public Func<bool> IsMoving() => () => player.GetComponent<CharacterController>().velocity.magnitude != 0;

        /// <summary>
        /// The condition for going between the MOVE and IDLE states
        /// </summary>
        /// <returns></returns>
        //public Func<bool> IsIdle() => () => player.GetComponent<CharacterController>().velocity.magnitude == 0;


        public Func<bool> IsClose() => () => Vector3.Distance(player.transform.position, transform.position) < 4;
        public Func<bool> IsIdle() => () => Vector3.Distance(player.transform.position, transform.position) >= 4;
       
        public virtual void Update()
        {
            FSM.Tick();

            if (damaged)
            {
                timer += Time.deltaTime;

                if (timer >= 1f)
                {
                    gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                    damaged = false;
                    timer = 0f;
                }
            }

            //if(Vector3.Distance(player.transform.position, this.transform.position) < 2)
            //{
            //    attackTimer += Time.deltaTime;

            //    if ((attackTimer >= 1))
            //    {
            //        Attack();
            //        attackTimer = 0f;
            //    }
                

            //}


        }

        void OnCollisionEnter(Collision collision)
        {
            //Attack();
        }

        protected override void Attack()
        {
            player.TakingDamage(1);
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
        public override void TakeDamage(float damage)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;

            damaged = true;

            base.TakeDamage(damage);
        }
    }
}
