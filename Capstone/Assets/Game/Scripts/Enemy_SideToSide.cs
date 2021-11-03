using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class Enemy_SideToSide : Character, IDamaging
    {
        // Gets a reference to the player
        // Will be used for finding the player in the world
        [SerializeField] private Player player;

        private bool damaged = false;
        private float timer = 0f;
        private float attackTimer = 0f;
        private int movePointsIndex;

        [SerializeField]
        private float damage; 

        // The event to call when damaging is finished
        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;

        // Testing for damaging system
        [Header("Damaging Timer Fields (Testing)")]
        [SerializeField]
        private float damagingTimerLimit;
        private float damagingTimer;
        private bool damaging;

        [SerializeField][Range(0, 1)]
        private float speed;
        [SerializeField]
        private List<Transform> movePoints = new List<Transform>();

        private BreakableBox thisBox;

        public float Damage { get => damage; }
        public bool Damaging { get => damaging; set => damaging = value; }

        public void Start()
        {
            transform.position = movePoints[0].position;
            movePointsIndex = 0;
            thisBox = GetComponent<BreakableBox>();
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            FindNextPoint();
        }


        private void Update()
        {
            // Checks if the enemy is equal to the position
            if (transform.position == movePoints[movePointsIndex].position)
            {
                movePointsIndex++;

                // If the index is greater than the list's length, set it back at the beginning
                if (movePointsIndex >= movePoints.Count)
                {
                    movePointsIndex = 0;
                }

                FindNextPoint();

                if (movePointsIndex >= movePoints.Count)
                {
                    movePointsIndex = 0;
                }


            }

            // Move the enemy towards the new point
            if (thisBox.isBroken == false)
            {
                transform.position = Vector3.MoveTowards(transform.position, movePoints[movePointsIndex].position, speed);
            }

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
            Attack();

        }

        public void Damaged()
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;

            damaged = true;

        }

        /// <summary>
        /// Checks to see if the next point is able to be reached
        /// </summary>
        public void FindNextPoint()
        {
            RaycastHit hit;

            if (Physics.Linecast(transform.position, movePoints[movePointsIndex].position, out hit))
            {
                if (hit.collider != null)
                {
                    movePointsIndex++;
                }
            }

        }

        protected override void Attack()
        {
            player.TakeDamage(ID, 1);
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
        protected override void Die()
        {

        }
        public override void Respawn()
        {
            throw new NotImplementedException();
        }

    }
}
