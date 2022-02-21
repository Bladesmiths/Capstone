using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The behavior for the Enemy wandering
    /// </summary>
    public class EnemyWANDER : Action
    {
        private Enemy _self;
        private CharacterController controller;
        private NavMeshAgent agent;
        
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


        public EnemyWANDER(Enemy self)
        {
            _self = self;
        }
        public EnemyWANDER()
        {
            
        }

        public override void OnStart()
        {
            //controller = _self.GetComponent<CharacterController>();
            _self = gameObject.GetComponent<Enemy>();
            agent = GetComponent<NavMeshAgent>();
            center = transform.position;
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
            _self.canMove = true;
            _self.InCombat = false;

        }

        public override TaskStatus OnUpdate()
        {
            // If the Enemy has been moving for the max time then find a new position
            if (moveTimer >= moveTimerMax)
            {
                wanderPoint = NewWanderPosition();
                

            }

            // If the Enemy is a certain distance away from their target point
            if (Vector3.Distance(transform.position, wanderPoint) >= 0.1f)
            {
                moveTimer += Time.deltaTime;

                // Moves the Enemy
                Vector3 dist = wanderPoint - _self.transform.position;

                if (agent != null)
                {
                    //transform.Translate(wanderPoint);

                    agent.SetDestination(wanderPoint);
                }
                dist.Normalize();

                Vector3 lookRotVec = new Vector3(dist.x + 0.001f, 0f, dist.z);
                if (lookRotVec.magnitude > Mathf.Epsilon)
                {
                    Quaternion q = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotVec),
                        Time.deltaTime * 5f);
                    transform.rotation = q;
                }                
            }
            else
            {
                moveTimer += Time.deltaTime;

                if (moveTimer >= moveTimerMax)
                {
                    wanderPoint = NewWanderPosition();

                }
            }
            return TaskStatus.Running;

        }

        public override void OnEnd()
        {
            _self.canMove = false;
        }

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
