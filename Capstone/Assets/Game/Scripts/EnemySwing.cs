using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class EnemySwing : Character
    {
        // Gets a reference to the player
        // Will be used for finding the player in the world
        private Player player;

        private float rotate;
        private float speed = 100;

        private void Start()
        {
            rotate = Time.deltaTime * speed;

        }        
       
        public virtual void Update()
        {

            if (transform.parent.eulerAngles.x <= 330 && transform.parent.eulerAngles.x > 180)
            {
                rotate = Time.deltaTime * speed;

            }

            else if (transform.parent.eulerAngles.x >= 30 && transform.parent.eulerAngles.x < 180)
            {
                rotate = -Time.deltaTime * speed;

            }

            transform.parent.Rotate(new Vector3(rotate, 0, 0));

        }

        void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.GetComponent<Player>() == true)
            {
                // Damage Player


            }

            else if(collision.collider.tag == "PreventDmg")
            {
                // Damage Enemy

            }

        }

        protected override void Attack()
        {
            //player.TakeDamage(1);
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

            base.TakeDamage(damage);
        }
    }
}
