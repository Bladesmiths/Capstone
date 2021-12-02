using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class BossAfterImageBug : Enemy
    {
        private float moveTimer = 0f;
        private int flip = 1;
        private int index;

        [SerializeField]
        private float speed;
        public List<Vector3> movePoints = new List<Vector3>();

        // The event to call when damaging is finished
        public new event IDamaging.OnDamagingFinishedDelegate DamagingFinished;

        public void Start()
        {
            transform.position = movePoints[0];
            index = 0;
            player = GameObject.Find("Player").GetComponent<Player>();
        }

        public override void Update()
        {
            if (index >= movePoints.Count)
            {
                index = 0;
            }

            if (transform.position == movePoints[index])
            {
                index++;
                
            }

            transform.position = Vector3.Lerp(transform.position, movePoints[index], speed);

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
            player.TakeDamage(ID, 1);
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
