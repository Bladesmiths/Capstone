using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    public abstract class Enemy : Character, IDamaging
    {
        // Reference to the Finite State Machine
        protected FiniteStateMachine FSM;

        // Gets a reference to the player
        // Will be used for finding the player in the world
        [SerializeField] 
        protected Player player;

        protected bool damaged = false;
        protected float timer = 0f;

        protected float moveSpeed;
        
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

        #region Enemy States
        protected EnemyFSMState_SEEK seek;
        protected EnemyFSMState_IDLE idle;
        protected EnemyFSMState_ATTACK attack;
        protected EnemyFSMState_TAKEDAMAGE takeDamage;
        protected EnemyFSMState_DEATH death;
        protected EnemyFSMState_WANDER wander;
        protected EnemyFSMState_MOVING move;
        #endregion
        
        public virtual void Awake()
        {
            // Creates the FSM
            FSM = new FiniteStateMachine();

        }

        public virtual void Start()
        {
            player = GameObject.Find("Player").GetComponent<Player>();
            
            // Creates all of the states
            seek = new EnemyFSMState_SEEK(player, this);
            idle = new EnemyFSMState_IDLE();
            death = new EnemyFSMState_DEATH(this);
            wander = new EnemyFSMState_WANDER(this);

            // Adds all of the possible transitions
            FSM.AddTransition(seek, wander, IsIdle());
            FSM.AddTransition(wander, seek, IsClose());

            FSM.AddAnyTransition(death, IsDead());

            // Sets the current state
            FSM.SetCurrentState(wander);

            // Sets the team of the enemy
            ObjectTeam = Team.Enemy;
        }

        /// <summary>
        /// Checks to see if the Enemy is dead
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsDead() => () => Health <= 0;

        /// <summary>
        /// Checks to see if the player is near the Enemy
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsClose() => () => Vector3.Distance(player.transform.position, transform.position) < 4;

        /// <summary>
        /// If the Player is far away then stop seeking
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsIdle() => () => Vector3.Distance(player.transform.position, transform.position) >= 4;

        
        public virtual void Update()
        {
            //FSM.Tick();

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
