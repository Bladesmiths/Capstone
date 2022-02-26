using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class BackUp : Action
{
    public SharedGameObject playerShared;
    private GameObject player;

    // How much should they back up
    [SerializeField] private float backUpAmount;
    // How long until they give up on backing up
    [SerializeField] private float backUpMaxTime;

    private NavMeshAgent navMeshAgent;
    private Vector3 destination;
    private float timer;

    public override void OnStart()
    {
        base.OnStart();

        player = playerShared.Value;
        navMeshAgent = GetComponent<NavMeshAgent>();

        destination = transform.position + (-transform.forward * backUpAmount);
    }

    public override TaskStatus OnUpdate()
    {
        timer += Time.deltaTime;

        // Go to a position behind where the boss is
        if (Vector3.Distance(transform.position, destination) >= 0.5f || timer <= backUpMaxTime)
        {
            navMeshAgent.SetDestination(destination);
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
