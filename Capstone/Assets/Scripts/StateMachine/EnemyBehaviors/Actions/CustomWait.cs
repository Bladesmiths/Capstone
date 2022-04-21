using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class CustomWait : Action
    {
        public float timer;
        public float timerMax;
        private NavMeshAgent agent;
        public bool canHitTrigger;
        private Enemy enemy;

        public override void OnStart()
        {
            timer = 0;
            timerMax = Random.Range(0.3f, 0.8f);
            agent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<Enemy>();
        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            if(timer >= timerMax)
            {
                timer = 0;
                return TaskStatus.Success;
            }

            if (agent != null)
            {
                agent.SetDestination(transform.position);
            }

            Vector3 dist = Player.instance.transform.position - transform.position;

            Vector3 lookRotVec = new Vector3(dist.x + 0.001f, 0f, dist.z);
            if (lookRotVec.magnitude > Mathf.Epsilon)
            {
                Quaternion q = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotVec),
                    Time.deltaTime * 5f);
                transform.rotation = q;
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            if(canHitTrigger)
            {
                enemy.CanHit = false;
            }

        }

    }
}