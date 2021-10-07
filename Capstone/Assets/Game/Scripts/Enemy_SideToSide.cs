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
        private int index;

        [SerializeField][Range(0, 1)]
        private float speed;
        public List<Vector3> movePoints = new List<Vector3>();

        // Incase we want to use transforms instead of Vector3's
        // We will need to do some code changes below but it should be similar
        //public List<Transform> movePoints = new List<Transform>();

        public void Start()
        {
            transform.position = movePoints[0];
            index = 0;
            player = GameObject.Find("Player").GetComponent<Player>();
            FindNextPoint();
        }


        private void Update()
        {
            // Checks if the enemy is equal to the position
            if (transform.position == movePoints[index])
            {
                index++;
                FindNextPoint();

            }

            

            // If the index is greater than the list's length, set it back at the beginning
            if (index >= movePoints.Count)
            {
                index = 0;
            }
            // Move the enemy towards the new point
            transform.position = Vector3.MoveTowards(transform.position, movePoints[index], speed);
            

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
            if (index >= movePoints.Count)
            {
                index = 0;
            }

            RaycastHit hit;

            if (Physics.Linecast(transform.position, movePoints[index], out hit))
            {
                if (hit.collider != null)
                {
                    index++;
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
