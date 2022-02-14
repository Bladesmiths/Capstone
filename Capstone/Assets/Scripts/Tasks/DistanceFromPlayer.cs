using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class DistanceFromPlayer : Conditional
{
    enum Operator
    {
        LessThanEqualTo,
        GreaterThanEqualTo,
        EqualTo
    }

    public SharedGameObject playerShared;
    private GameObject player;

    [SerializeField] private Operator ifOperator;
    [SerializeField] private float distance;

    public override void OnStart()
    {
        player = playerShared.Value;
    }

    public override TaskStatus OnUpdate()
    {
        switch(ifOperator)
        {
            case Operator.LessThanEqualTo:
                if (Vector3.Distance(transform.position, player.transform.position) <= distance)
                    return TaskStatus.Success;
                break;
            case Operator.GreaterThanEqualTo:
                if (Vector3.Distance(transform.position, player.transform.position) >= distance)
                    return TaskStatus.Success;
                break;
            case Operator.EqualTo:
                if (Vector3.Distance(transform.position, player.transform.position) == distance)
                    return TaskStatus.Success;
                break;
        }

        return TaskStatus.Failure;
    }
}
