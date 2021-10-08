using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class Enemy_SideToSide : Character
    {
        // Gets a reference to the player
        // Will be used for finding the player in the world
        [SerializeField] private Player player;

        private bool damaged = false;
        private float timer = 0f;
        private float attackTimer = 0f;
        private int movePointsIndex;

        [SerializeField][Range(0, 1)]
        private float speed;
        [SerializeField]
        private List<Transform> movePoints = new List<Transform>();

        private BreakableBox thisBox;

       

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
            player.TakeDamage(1);
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
