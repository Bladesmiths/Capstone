using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class RotateSword : Action
    {
        private Vector3 dist;
        private NavMeshAgent agent;
        public Vector3 playerPos;
        public AnimationCurve curveZ;
        public AnimationCurve curveY;
        private float timer;
        private float timerMax;
        private GameObject sword;
        private Enemy enemy;


        public RotateSword()
        {

        }

        public override void OnStart()
        {
            agent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<Enemy>(); 
            agent.speed = 18f;
            agent.acceleration = 20f;
            playerPos = Player.instance.transform.position;// - transform.position;
            timerMax = 1f;
            timer = 0;
            sword = enemy.Sword;

        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            float valZ = curveZ.Evaluate(timer);
            float valY = curveY.Evaluate(timer);            
            sword.transform.localRotation = Quaternion.Euler(0f, valY, valZ);
            dist = playerPos - transform.position; 

            if(timer >= timerMax)
            {
                return TaskStatus.Success;

            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            agent.speed = 3.5f;
            agent.acceleration = 8f;


        }

    }
}