using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class DashTowardsPlayer : Action
    {
        private Vector3 dist;
        private Vector3 distance;

        private NavMeshAgent agent;
        public Vector3 playerPos;
        public AnimationCurve curveZ;
        public AnimationCurve curveY;
        private float timer;
        private float timerMax;
        //private GameObject sword;
        private Enemy enemy;
        public float enemySpeed;
        public float enemyAccl;

        public DashTowardsPlayer()
        {

        }

        public override void OnStart()
        {
            agent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<Enemy>(); 
            agent.speed = enemySpeed;
            agent.acceleration = enemyAccl;
            //playerPos = Player.instance.transform.position;// - transform.position;
            distance = Player.instance.transform.position - transform.position;
            playerPos = (-distance.normalized * 2f) + Player.instance.transform.position;
            timerMax = 1f;
            timer = 0;
            //sword = enemy.Sword;
            //sword.GetComponent<BoxCollider>().enabled = true;
            enemy.attackTimerMax = Random.Range(0.5f, 2f);
            enemy.isAttacking = true;
            enemy.attackedYet = true;

        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            float valZ = curveZ.Evaluate(timer);
            float valY = curveY.Evaluate(timer);            
            //sword.transform.localRotation = Quaternion.Euler(0f, valY, valZ);
            agent.SetDestination(playerPos);
            dist = playerPos - transform.position;

            if (timer >= timerMax)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            agent.speed = 3.5f;
            agent.acceleration = 8f;
            //sword.GetComponent<BoxCollider>().enabled = false;
            enemy.isAttacking = false;

        }

        /// <summary>
        /// Draws a UI element around the Enemy
        /// </summary>
        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(playerPos, .3f);

        }

    }
}