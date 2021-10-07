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
        private float moveTimer = 0f;
        private int flip = 1;
        private int index;

        [SerializeField][Range(0, 1)]
        private float speed;
        public List<Vector3> movePoints = new List<Vector3>();

        public void Start()
        {
            transform.position = movePoints[0];
            index = 0;
            player = GameObject.Find("Player").GetComponent<Player>();
        }


        private void Update()
        {
            if (index >= movePoints.Count)
            {
                index = 0;
            }

            if (transform.position == movePoints[index])
            {
                index++;
                
            }

            

            transform.position = Vector3.MoveTowards(transform.position, movePoints[index], Vector3.Distance(transform.position, movePoints[index]));
            


            


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

        public void FindNextPoint()
        {
            for(int i = 0; i < movePoints.Count;)
            {
                if(transform.position == movePoints[i])
                {
                    i++;
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
