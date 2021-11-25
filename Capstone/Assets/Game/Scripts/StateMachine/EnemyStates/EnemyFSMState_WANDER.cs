using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The behavior for the Enemy wandering
    /// </summary>
    public class EnemyFSMState_WANDER : EnemyFSMState
    {
        private Enemy _self;
        private CharacterController controller;
        
        // The point to head towards
        private Vector3 wanderPoint;

        // The center point from where they should wander
        private Vector3 center;
        private float moveTimer;
        private float moveTimerMax;
        private float speed;
        private float randMin;
        private float randMax;
        private float minTime;
        private float maxTime;


        public EnemyFSMState_WANDER(Enemy self)
        {
            _self = self;
        }

        public override void OnEnter()
        {
            //controller = _self.GetComponent<CharacterController>();

            center = _self.transform.position;
            moveTimer = 0;
            moveTimerMax = 1f;
            speed = 2;
            randMin = -2f;
            randMax = 2f;
            minTime = 0.2f;
            maxTime = 2f;

            float x = Random.Range(randMin, randMax) + center.x;
            float y = center.y;
            float z = Random.Range(randMin, randMax) + center.z;

            wanderPoint = new Vector3(x, y, z);
        }

        public override void Tick()
        {
            // If the Enemy has been moving for the max time then find a new position
            if (moveTimer >= moveTimerMax)
            {
                wanderPoint = NewWanderPosition();

            }

            // If the Enemy is a certain distance away from their target point
            if (Vector3.Distance(_self.transform.position, wanderPoint) >= 0.1f)
            {
                moveTimer += Time.deltaTime;

                // Moves the Enemy
                Vector3 dist = wanderPoint - _self.transform.position;

                _self.moveVector = wanderPoint;// dist.normalized;
                _self.rotateVector = dist.normalized;
                
            }
            else
            {
                moveTimer += Time.deltaTime;

                if (moveTimer >= moveTimerMax)
                {
                    wanderPoint = NewWanderPosition();

                }
            }

        }

        public override void OnExit() { }

        /// <summary>
        /// Checks to see if the enemy is heading into/through a wall
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Vector3 NewWanderPosition()
        {            
            moveTimer = 0;
            moveTimerMax = Random.Range(minTime, maxTime);

            Vector3 randDirection = Random.insideUnitSphere * randMax;
            //randDirection.y = center.y;

            randDirection += center;

            NavMeshHit navHit;

            NavMesh.SamplePosition(randDirection, out navHit, randMax, -1);

            return navHit.position;                        
        }
    }
}
