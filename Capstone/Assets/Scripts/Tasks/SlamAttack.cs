using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

public class SlamAttack : Action
{

    [SerializeField] private GameObject sword;
    [SerializeField] private float nodeDuration;

    [SerializeField] private GameObject shockwave;

    private float timer;

    public override void OnStart()
    {
        timer = 0;
        //DOTween.SetTweensCapacity(1250, 50);
    }

    public override TaskStatus OnUpdate()
    {
        // Run for as long as it's told to run
        if (timer <= nodeDuration)
        {
            // Tween the sword from vertical down to barely touching the ground
            //sword.transform.DOLocalRotate(new Vector3(98, 0, 0), 0.2f);
            timer += Time.deltaTime;
            return TaskStatus.Running;
        }
        // Active the shockwave box and expand it over a second
        shockwave.SetActive(true);
        shockwave.transform.DOScaleX(8, 1);
        //sword.transform.DOComplete();
        return TaskStatus.Success;
    }
}
