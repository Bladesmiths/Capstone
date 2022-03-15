using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class SeekPlayer : Action
{
    public SharedGameObject playerShared;
    private GameObject player;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float stoppingDistance;

    private NavMeshAgent navMeshAgent;

    public override void OnStart()
    {
        player = playerShared.Value;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.acceleration = acceleration;
        navMeshAgent.angularSpeed = turnSpeed;
    }

    public override TaskStatus OnUpdate()
    {
        if (Vector3.Distance(transform.position, player.transform.position) >= stoppingDistance)
        {
            navMeshAgent.SetDestination(player.transform.position);
            return TaskStatus.Running;
        }
        else
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            return TaskStatus.Success;
        }
    }
}
