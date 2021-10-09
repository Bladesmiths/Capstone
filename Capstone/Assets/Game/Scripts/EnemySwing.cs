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
        private bool isBroken = false;
        private float fadeOutTimer = 0f;
        private float fadeOutLength = 2f;
        private float shrinkSpeed = 1.0f;

        [SerializeField]
        private GameObject enemyObject;

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

            if (isBroken == false)
            {
                transform.parent.Rotate(new Vector3(rotate, 0, 0));
            }
            else
            {
                fadeOutTimer += Time.deltaTime;
            }

            // When the object should fade out
            if (fadeOutTimer >= fadeOutLength)
            {
                // Shrink the cubes
                enemyObject.transform.localScale = new Vector3(
                    enemyObject.transform.localScale.x - (shrinkSpeed * Time.deltaTime), 
                    enemyObject.transform.localScale.y - (shrinkSpeed * Time.deltaTime), 
                    enemyObject.transform.localScale.z - (shrinkSpeed * Time.deltaTime));

                // After the cubes are shrunk, destroy it
                if (enemyObject.transform.localScale.x <= 0 && enemyObject.transform.localScale.y <= 0 && enemyObject.transform.localScale.z <= 0)
                {
                    Destroy(enemyObject);
                }

            }

        }

        void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.GetComponent<Player>() == true)
            {
                // Damage Player
                collision.collider.GetComponent<Player>().TakeDamage(1);
                Debug.Log("Player hit!!");

            }
            Debug.Log("Collision Happened!!");

            

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "PreventDmg")
            {
                // Damage Enemy
                Debug.Log("Block hit!!");
                isBroken = true;
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
