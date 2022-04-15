using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class BackUp : Action
    {
        public SharedGameObject playerShared;
        private GameObject player;

        // How much should they back up
        [SerializeField] private float backUpAmount;
        // How long until they give up on backing up
        [SerializeField] private float backUpMaxTime;

        [SerializeField] private Transform bossArenaCenter;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float acceleration;

        private NavMeshAgent navMeshAgent;
        private Vector3 destination;

        public override void OnStart()
        {
            base.OnStart();

            player = playerShared.Value;
            navMeshAgent = GetComponent<NavMeshAgent>();

            destination = transform.position + (-transform.forward * backUpAmount);

            navMeshAgent.speed = moveSpeed;
            navMeshAgent.acceleration = acceleration;
        }

        public override TaskStatus OnUpdate()
        {
            if(transform.position == GetComponent<Boss>().lastPosition)
            {
                GetComponent<Boss>().hasntMovedCounter++;
            }

            if (Vector3.Distance(transform.position, destination) >= 0.5f && GetComponent<Boss>().againstWallAgain)
            {
                destination = bossArenaCenter.position;
                navMeshAgent.SetDestination(destination);
                GetComponent<Boss>().lastPosition = transform.position;
                return TaskStatus.Running;
            }
            else if (Vector3.Distance(transform.position, destination) >= 0.5f && GetComponent<Boss>().hasntMovedCounter >= 2)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
                GetComponent<Boss>().againstWallAgain = true;
                return TaskStatus.Success;
            }
            else if (Vector3.Distance(transform.position, destination) >= 0.5f)
            {
                navMeshAgent.SetDestination(destination);
                GetComponent<Boss>().lastPosition = transform.position;
                return TaskStatus.Running;
            }
            else
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
                GetComponent<Boss>().againstWallAgain = false;
                GetComponent<Boss>().hasntMovedCounter = 0;
                return TaskStatus.Success;
            }

        }
    }
}
