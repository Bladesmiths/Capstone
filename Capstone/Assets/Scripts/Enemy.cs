using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using Bladesmiths.Capstone.Enums;
using BehaviorDesigner.Runtime;

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
        [SerializeField] protected Player player;

        [SerializeField] private GameObject sword;

        [SerializeField] protected int chunksRemoved;
        protected bool damaged = false;
        protected float timer = 0f;

        protected float moveSpeed;
        public Vector3 moveVector;
        public Vector3 rotateVector;
        public Quaternion swordRot;
        public Vector3 defaultSwordPos;

        [SerializeField] protected float damage;

        [SerializeField] protected float shrinkSpeed;
        protected float fadeOutTimer;
        protected float fadeOutLength;

        [SerializeField] protected float viewDistance;

        public bool surrounding;

        // The event to call when damaging is finished
        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;

        // Testing for damaging system
        [Header("Damaging Timer Fields (Testing)")] [SerializeField]
        protected float damagingTimerLimit;

        protected float damagingTimer;
        protected bool damaging;

        public float attackTimer;
        public float attackTimerMax;
        public float moveTimer;
        public float moveTimerMax;
        public bool stunned;
        public bool canMove;
        public bool parried;
        public bool blocked = false;
        public bool attackedYet;

        public int enemyGroupNumber;

        private bool inCombat;
        public bool isAttacking;

        [SerializeField]
        public GameObject geo;
        [SerializeField]
        public GameObject bodyChunks;
        [SerializeField]
        public GameObject spine;

        public float Damage
        {
            get => damage;
        }

        public bool Damaging
        {
            get => damaging;
            set => damaging = value;
        }

        public bool CanHit { get; set; }

        public GameObject Sword
        {
            get => sword;
        }

        public bool InCombat
        {
            get => inCombat;
            set => inCombat = value;
        }

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
            surrounding = false;
        }

        public virtual void Start()
        {
            // Sets the team of the enemy
            ObjectTeam = Team.Enemy;
            AIDirector.Instance.AddToEnemyGroup(this, enemyGroupNumber);
            ObjectController.Instance.AddIdentifiedObject(ObjectTeam, this);
            
            stunned = false;
            player = Player.instance;

            moveVector = Vector3.zero;
            moveSpeed = 5f;
            controller = GetComponent<CharacterController>();
            agent = GetComponent<NavMeshAgent>();
            attackTimerMax = 0.3f;
            attackTimer = 0f;
            fadeOutTimer = 0f;
            fadeOutLength = 3f;
            chunksRemoved = 3;
            canMove = false;
            damagingTimer = 0f;
            swordRot = Sword.transform.localRotation;
            defaultSwordPos = Sword.transform.localPosition;
            blocked = false;

            if (agent != null)
            {
                agent.updateRotation = false;
            }
            //CanHit = true;            
        }

        
        public virtual void Update()
        {
            //FSM.Tick();
            //Debug.Log(InCombat);

            if((player.transform.position - transform.position).magnitude <= 2.5f)
            {
                inCombat = true;
            }

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

        public override void Respawn()
        {
            throw new NotImplementedException();
        }

        public void RemoveRandomChunk()
        {
            if (bodyChunks.transform.childCount <= 10)
            {
                return;    
            }
        
            GameObject remover = bodyChunks;
            
            GameObject removedChunk = remover.transform.GetChild(UnityEngine.Random.Range(0, remover.transform.childCount)).gameObject;
            removedChunk.transform.parent = null;
            removedChunk.AddComponent<BoxCollider>();
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
                int num = NumChunks();
                for (int i = 0; i < num; i++)
                {
                    RemoveRandomChunk();
                }

                inCombat = true;
                damaged = true;
                //damaged = true;
            }

            // Return whether damage was taken or not
            return damageResult;
        }
    }
}