using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    public class Enemy : Character, IDamaging
    {
        // Reference to the Finite State Machine
        private FiniteStateMachine FSM;
        //[SerializeField] private TransitionManager playerTransitionManager;

        // Gets a reference to the player
        // Will be used for finding the player in the world
        [SerializeField] 
        protected Player player;

        protected bool damaged = false;
        protected float timer = 0f;
        protected float attackTimer = 0f;

        [SerializeField]
        protected float damage;
        
        // The event to call when damaging is finished
        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;

        // Testing for damaging system
        [Header("Damaging Timer Fields (Testing)")]
        [SerializeField]
        protected float damagingTimerLimit;
        protected float damagingTimer;
        protected bool damaging;

        public float Damage { get => damage; }
        public bool Damaging { get => damaging; set => damaging = value; }

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

            // Sets the team of the enemy
            ObjectTeam = Team.Enemy;
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

            // Testing
            // If the enemy is currently damaging an object
            if (damaging)
            {
                // Update the timer
                damagingTimer += Time.deltaTime;

                // If the timer is equal to or exceeds the limit
                if (damagingTimer >= damagingTimerLimit)
                {
                    // If the damaging finished event has subcribing delegates
                    // Call it, running all subscribing delegates
                    if (DamagingFinished != null)
                    {
                        DamagingFinished(ID);
                    }
                    // If the damaging finished event doesn't have any subscribing events
                    // Something has gone wrong because damaging shouldn't be true otherwise
                    else
                    {
                        Debug.Log("Damaging Finished Event was not subscribed to correctly");
                    }

                    // Reset fields
                    damagingTimer = 0.0f;
                    damaging = false;
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            //Attack();

        }

        protected virtual void Attack()
        {
            player.TakeDamage(ID, 1);
        }

        protected override void Die()
        {

        }
        public override void Respawn()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Subtract an amount of damage from the character's health
        /// </summary>
        /// <param name="damagingID">The id of the damaging object that is damaging this character</param>
        /// <param name="damage">The amount of damage to be subtracted</param>
        /// <returns>Returns a boolean indicating whether damage was taken or not</returns>
        public override float TakeDamage(int damagingID, float damage)
        {
            // The resullt of Character's Take Damage
            // Was damage taken or not
            float damageResult = base.TakeDamage(damagingID, damage);

            // If damage was taken
            // Change the object to red and set damaged to true
            if (damageResult > 0)
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Color.red;

                damaged = true;
            }

            // Return whether damage was taken or not
            return damageResult;
        }
    }
}
