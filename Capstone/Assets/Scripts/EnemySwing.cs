using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class EnemySwing : Enemy, IDamaging
    {
        private float rotate;
        private float speed = 60;
        private bool isBroken = false;
        private float fadeOutTimer = 0f;
        private float fadeOutLength = 1f;
        private float shrinkSpeed = 1.0f;

        [SerializeField]
        private GameObject enemyObject;
        private float preAttackTimer;
        private float preAttackTimerMax;
        private bool attack;
        private BoxCollider box;

        // The event to call when damaging is finished
        public new event IDamaging.OnDamagingFinishedDelegate DamagingFinished;

        private void Start()
        {
            rotate = speed;
            player = GameObject.Find("Player").GetComponent<Player>();
            attack = true;
            preAttackTimer = 0f;
            preAttackTimerMax = 0.5f;
            box = GetComponent<BoxCollider>();
            box.enabled = false;
        }

        public override void Update()
        {            
            // Checks of the Enemy has detected a block
            if (isBroken == false)
            {
                if (attack)
                {
                    StartAttack();

                    if (transform.parent.localEulerAngles.x >= 39.9f && transform.parent.localEulerAngles.x <= 40.5f)
                    {
                        attack = false;
                        box.enabled = false;
                    }
                }
                else
                {
                    StopAttack();

                    if (transform.parent.localEulerAngles.x <= 330.5f && transform.parent.localEulerAngles.x >= 329.9f)
                    {
                        attack = true;
                        preAttackTimer = 0f;
                    }
                }
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
        /// The method for the Enemy attacking the Player
        /// </summary>
        public void StartAttack()
        {
            preAttackTimer += Time.deltaTime;
            if (preAttackTimer <= preAttackTimerMax)
            {
                transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, Quaternion.Euler(-70f, transform.parent.eulerAngles.y, 0f), 0.1f);
            }
            else
            {
                box.enabled = true;
                transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, Quaternion.Euler(40f, transform.parent.eulerAngles.y, 0f), 0.3f);
            }
        }

        /// <summary>
        /// The method for resetting the Enemy's sword
        /// </summary>
        public void StopAttack()
        {
            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, Quaternion.Euler(-30f, transform.parent.eulerAngles.y, 0f), 0.1f);
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
                if (player.GetPlayerFSMState() == Enums.PlayerCondition.F_Blocking /*&& other.GetComponent<Sword>() == true*/)
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

        protected override void Die()
        {

        }
        public override void Respawn()
        {
            throw new NotImplementedException();
        }

    }
}
