using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class FacePlayerLastPosition : Action
{
    public SharedGameObject playerShared;
    private GameObject player;

    private Vector3 currentPlayerPos;

    [SerializeField] float turnSpeed;
    
    public override void OnStart()
    {
        base.OnStart();

        player = playerShared.Value;

        currentPlayerPos = player.transform.position;
    }

    public override TaskStatus OnUpdate()
    {
        // Get a quaternion facing the position the player was at when this node was first called
        Quaternion lookRotation = Quaternion.LookRotation(currentPlayerPos - transform.position);
        lookRotation.eulerAngles = new Vector3(0, lookRotation.eulerAngles.y, 0);

        // Create a temp quaternion that is rotating this object towards the target
        Quaternion q = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
        q.eulerAngles = new Vector3(0, q.eulerAngles.y, 0);

        // Update the transform
        transform.rotation = q;

        // If not facing the player, keep turning
        if (Quaternion.Angle(transform.rotation,lookRotation) >= 1)
        {
            return TaskStatus.Running;
        }
        return TaskStatus.Success;
    }
}
