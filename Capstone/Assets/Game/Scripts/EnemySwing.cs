using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class EnemySwing : Character, IDamaging
    {
        // Gets a reference to the player
        // Will be used for finding the player in the world
        private Player player;

        private float rotate;
        private float speed = 60;
        private bool isBroken = false;
        private float fadeOutTimer = 0f;
        private float fadeOutLength = 1f;
        private float shrinkSpeed = 1.0f;

        [SerializeField]
        private GameObject enemyObject;

        [SerializeField]
        private float damage = 10;

        // The event to call when damaging is finished
        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;

        // Testing for damaging system
        [Header("Damaging Timer Fields (Testing)")]
        [SerializeField]
        private float damagingTimerLimit;
        private float damagingTimer;
        private bool damaging;

        public float Damage { get => damage; }

        private void Start()
        {
            rotate = speed;
            player = GameObject.Find("Player").GetComponent<Player>();
        }

        public virtual void Update()
        {
            // Rotates downwards 
            if (transform.parent.eulerAngles.x <= 330 && transform.parent.eulerAngles.x > 180)
            {
                rotate = speed;
            }

            // Rotates upwards
            else if (transform.parent.eulerAngles.x >= 30 && transform.parent.eulerAngles.x < 180)
            {
                rotate = -speed;
            }

            // Checks of the Enemy has detected a block
            if (isBroken == false)
            {
                transform.parent.Rotate(new Vector3(rotate * Time.deltaTime, 0, 0));
            }
            else
            {
                // If there is a block then start fading out
                fadeOutTimer += Time.deltaTime;
                GetComponent<BoxCollider>().enabled = false;
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
                if (enemyObject.transform.localScale.x <= 0 && 
                    enemyObject.transform.localScale.y <= 0 && 
                    enemyObject.transform.localScale.z <= 0)
                {
                    Destroy(enemyObject);
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

        /// <summary>
        /// Allows for the player to hit and be hit by the Swinging Enemy
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<BlockCollision>() != null || other.GetComponent<Player>() != null)
            { 
                // Checks to see if the player is in the blocking state
                if (player.GetPlayerFSMState().ID == Enums.PlayerCondition.F_Blocking /*&& other.GetComponent<Sword>() == true*/)
                {
                    // Damage Enemy
                    isBroken = true;
                }
                // Otherwise deal damage to the player
                else if (other.GetComponent<Player>() == true)
                {
                    // Damage Player
                    ((IDamageable)ObjectController[player.ID].IdentifiedObject).TakeDamage(ID, damage);

                    damaging = true;

                    // Knockback Player
                    // Will be a method in the Player class
                    //other.GetComponent<Player>().Knockback();
                }
            }                        
        }

        protected override void Attack()
        {

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
        public override void Respawn()
        {
            throw new NotImplementedException();
        }

    }
}
