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

      
        private void Update()
        {

            if(transform.position.x > 12)
            {
                flip = -1;
            }

            if (transform.position.x < 8)
            {
                flip = 1;

            }

            transform.position += new Vector3(2 * flip * Time.deltaTime, 0, 0);


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

    }
}
