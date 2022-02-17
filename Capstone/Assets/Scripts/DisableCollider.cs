using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class DisableCollider : Action
{
    [SerializeField] private GameObject colliderParent;

    public override void OnStart()
    {

    }

    public override TaskStatus OnUpdate()
    {
        colliderParent.GetComponent<Collider>().enabled = false;
        return TaskStatus.Success;
    }
}
