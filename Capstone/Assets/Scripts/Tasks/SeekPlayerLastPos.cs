using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class SeekPlayerLastPos : Action
{
    public SharedGameObject playerShared;
    private GameObject player;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float stoppingDistance;

    private NavMeshAgent navMeshAgent;
    private Vector3 lastPlayerPos;

    public override void OnStart()
    {
        player = playerShared.Value;
        lastPlayerPos = player.transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.angularSpeed = turnSpeed;
    }

    public override TaskStatus OnUpdate()
    {
        if (Vector3.Distance(transform.position, lastPlayerPos) >= stoppingDistance)
        {
            navMeshAgent.SetDestination(lastPlayerPos);
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
