using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

public class SweepAttack : Action
{
    [SerializeField] private GameObject sword;
    [SerializeField] private Transform originalSwordTransform;
    [SerializeField] private float nodeDuration;

    public SharedGameObject playerShared;
    private GameObject player;

    private float timer;

    private float rotationEnd;
    private float currentRotation;

    public override void OnStart()
    {
        timer = 0;
        player = playerShared.Value;
        rotationEnd = 420;
        currentRotation = 0;
    }

    public override TaskStatus OnUpdate()
    {
        // While a "full" rotation has not been completed
        if(currentRotation <= rotationEnd)
        {
            // Rotate the sword down to parallel with the ground and spin the boss
            sword.transform.DOLocalRotate(new Vector3(90, 0, 0), 0.05f);
            transform.Rotate(0, -720 * Time.deltaTime, 0);
            currentRotation += 720 * Time.deltaTime;

            return TaskStatus.Running;
        }
        sword.transform.DOComplete();
        return TaskStatus.Success;
    }
}
