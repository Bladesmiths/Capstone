using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class ActivateCollider : Action
{
    [SerializeField] private GameObject colliderParent;

    public override void OnStart()
    {

    }

    public override TaskStatus OnUpdate()
    {
        colliderParent.GetComponent<Collider>().enabled = true;
        return TaskStatus.Success;
    }
}
