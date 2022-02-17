using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

public class GrowSword : Action
{
    [SerializeField] private GameObject swordBlade;
    [SerializeField] private Transform swordBladeEndTransform;
    [SerializeField] private float nodeDuration;

    private float timer;


    public override void OnStart()
    {
        timer = 0;
    }

    public override TaskStatus OnUpdate()
    {
        // While the node is set to run
        if (timer < nodeDuration)
        {
            // Tween the sword from where it's transform is to the predefined larger blade transform
            swordBlade.transform.DOScale(swordBladeEndTransform.localScale, nodeDuration);
            swordBlade.transform.DOLocalRotateQuaternion(swordBladeEndTransform.localRotation, nodeDuration);
            swordBlade.transform.DOLocalMove(swordBladeEndTransform.localPosition, nodeDuration);

            timer += Time.deltaTime;

            return TaskStatus.Running;
        }
        swordBlade.transform.DOComplete();
        return TaskStatus.Success;
    }

}
