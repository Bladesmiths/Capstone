using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

namespace Bladesmiths.Capstone
{
    public class ResetSword : Action
    {
        [SerializeField] private GameObject sword;
        [SerializeField] private GameObject swordBlade;
        [SerializeField] private Transform originalBladeTransform;
        [SerializeField] private float nodeDuration;

        [SerializeField] private GameObject shockwave;

        private float timer;


        public override void OnStart()
        {
            timer = 0;
            shockwave.GetComponent<BossSword>().ClearDamaging();
        }

        public override TaskStatus OnUpdate()
        {
            // Reset the shockwave
            shockwave.transform.localScale = new Vector3(1, shockwave.transform.localScale.y, shockwave.transform.localScale.z);
            shockwave.SetActive(false);
            // When the node isn't finished
            if (timer < nodeDuration)
            {
                // Tween the sword back to it's original position
                swordBlade.transform.DOScale(originalBladeTransform.localScale, nodeDuration);
                swordBlade.transform.DOLocalRotateQuaternion(originalBladeTransform.localRotation, nodeDuration);
                swordBlade.transform.DOLocalMove(originalBladeTransform.localPosition, nodeDuration);

                sword.transform.DOLocalRotate(new Vector3(0, 0, 0), nodeDuration);

                timer += Time.deltaTime;

                return TaskStatus.Running;
            }

            swordBlade.transform.DOComplete();
            swordBlade.GetComponent<BossSword>().ClearDamaging();
            return TaskStatus.Success;
        }
    }
}
