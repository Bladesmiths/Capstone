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

        //protected AIDirector director;
        private CharacterController controller;
        private NavMeshAgent agent;

        // Gets a reference to the player
        // Will be used for finding the player in the world
        [SerializeField] 
        protected Player player;

        [SerializeField]
        private GameObject sword;

        [SerializeField]
        protected int chunksRemoved;
        protected bool damaged = false;
        protected float timer = 0f;

        protected float moveSpeed;
        public Vector3 moveVector;
        public Vector3 rotateVector;

        [SerializeField]
        protected float damage;

        [SerializeField]
        private float shrinkSpeed;
        private float fadeOutTimer;
        private float fadeOutLength;

        [SerializeField]
        protected float viewDistance;
        
        // The event to call when damaging is finished
        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;

        // Testing for damaging system
        [Header("Damaging Timer Fields (Testing)")]
        [SerializeField]
        protected float damagingTimerLimit;
        protected float damagingTimer;
        protected bool damaging;

        public float attackTimer;
        public float attackTimerMax;
        public bool stunned;

        private bool inCombat;

        public float Damage { get => damage; }
        public bool Damaging { get => damaging; set => damaging = value; }
        public bool CanHit { get; set; }
        public GameObject Sword { get => sword; }
        public bool InCombat { get => inCombat; set => inCombat = value; }

        #region Enemy States
        protected EnemyFSMState_SEEK seek;
        protected EnemyFSMState_IDLE idle;
        protected EnemyFSMState_ATTACK attack;
        protected EnemyFSMState_TAKEDAMAGE takeDamage;
        protected EnemyFSMState_DEATH death;
        protected EnemyFSMState_WANDER wander;
        protected EnemyFSMState_MOVING move;
        protected EnemyFSMState_STUN stun;
        #endregion

        public virtual void Awake()
        {
            // Creates the FSM
            FSM = new FiniteStateMachine();
            damage = 15f;

        }

        public virtual void Start()
        {
            AIDirector.Instance.AddToEnemyGroup(this);
            stunned = false;
            player = GameObject.Find("Player").GetComponent<Player>();

            moveVector = Vector3.zero;
            moveSpeed = 5f;
            controller = GetComponent<CharacterController>();
            agent = GetComponent<NavMeshAgent>();
            attackTimerMax = 0.5f;
            attackTimer = attackTimerMax;
            fadeOutTimer = 0f;
            fadeOutLength = 3f;
            chunksRemoved = 3;

            // Creates all of the states
            seek = new EnemyFSMState_SEEK(player, this);
            idle = new EnemyFSMState_IDLE();
            death = new EnemyFSMState_DEATH(this);
            wander = new EnemyFSMState_WANDER(this);
            attack = new EnemyFSMState_ATTACK(sword, this);
            stun = new EnemyFSMState_STUN(sword, this, player);

            // Adds all of the possible transitions
            //FSM.AddTransition(seek, wander, IsIdle());
            //FSM.AddTransition(wander, seek, IsClose());
            //FSM.AddTransition(seek, attack, CanAttack());
            //FSM.AddTransition(attack, seek, DoneAttacking());
            //FSM.AddTransition(attack, stun, Stunned());
            //FSM.AddTransition(stun, seek, KeepAttacking());
            //FSM.AddTransition(stun, wander, GoWander());


            agent.updateRotation = false;

            //CanHit = true;

            //FSM.AddAnyTransition(death, IsDead());

            // Sets the current state
            //FSM.SetCurrentState(wander);

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
        public Func<bool> IsClose() => () => Vector3.Distance(player.transform.position, transform.position) < viewDistance;

        /// <summary>
        /// If the Player is far away then stop seeking
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsIdle() => () => Vector3.Distance(player.transform.position, transform.position) >= viewDistance;

        /// <summary>
        /// If the Enemy can attack
        /// Set through the AI Director
        /// </summary>
        /// <returns></returns>
        public Func<bool> CanAttack() => () => CanHit;

        /// <summary>
        /// If the Enemy is finishing attacking
        /// 
        /// </summary>
        /// <returns></returns>
        public Func<bool> DoneAttacking() => () => !CanHit;

        /// <summary>
        /// If the Enemy has been parried 
        /// </summary>
        /// <returns></returns>
        public Func<bool> Stunned() => () => player.parrySuccessful;

        /// <summary>
        /// If the Enemy is no longer stunned 
        /// </summary>
        /// <returns></returns>
        public Func<bool> KeepAttacking() => () => stun.continueAttacking == true;

        /// <summary>
        /// If the Enemy is no longer stunned 
        /// </summary>
        /// <returns></returns>
        public Func<bool> GoWander() => () => stun.continueAttacking == true && Vector3.Distance(player.transform.position, transform.position) >= viewDistance;

        public virtual void Update()
        {
            //FSM.Tick();

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

            // Movement
            agent.SetDestination(moveVector);

            Debug.DrawLine(transform.position, rotateVector, Color.red);

            // This is dumb and it probably needs to be changed, but I need to be able to see debug messages
            //if (!float.IsNaN(rotateVector.x)) 
            //{
            //    Quaternion q = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotateVector, Vector3.up), 0.25f);
            //    q.eulerAngles = new Vector3(0, q.eulerAngles.y, 0);
            //    transform.rotation = q;
            //}
            Quaternion q = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotateVector, Vector3.up), 0.25f);
            q.eulerAngles = new Vector3(0, q.eulerAngles.y, 0);
            transform.rotation = q;
        }

        public void ClearDamaging()
        {
            // If the enemy is currently damaging an object
            if (damaging)
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
                damaging = false;
            }
        }
        public void SwordAttack(int targetID)
        {
           ((IDamageable)ObjectController[targetID].IdentifiedObject).TakeDamage(ID, Damage);
            
            // Testing
            damaging = true;
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
        public void RemoveRandomChunk()
        {
            GameObject removedChunk = transform.GetChild(1).GetChild(UnityEngine.Random.Range(0, transform.GetChild(1).childCount)).gameObject;
            removedChunk.transform.parent = null;
            //removedChunck.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            removedChunk.AddComponent<Rigidbody>();
            removedChunk.AddComponent<EnemyChunk>();          

        }

        public int NumChunks()
        {
            return chunksRemoved * (int)(player.CurrentSword.Damage / 5);            
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
                if (transform.GetChild(1).childCount > 30)
                {
                    for (int i = 0; i < NumChunks(); i++)
                    {                    
                        RemoveRandomChunk();
                    }
                }
                damaged = true;
            }

            // Return whether damage was taken or not
            return damageResult;
        }
    }

    
}
