using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

namespace Bladesmiths.Capstone
{
    public class SlamAttack : Action
    {

        [SerializeField] private GameObject sword;
        [SerializeField] private float nodeDuration;

        [SerializeField] private GameObject shockwave;
        [SerializeField] private GameObject slamVFX;

        private float timer;

        public override void OnStart()
        {
            timer = 0;
            slamVFX.GetComponent<VFXManager>().DisableVFX();
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
            slamVFX.transform.position = new Vector3(shockwave.transform.position.x, 0, shockwave.transform.position.z);
            slamVFX.GetComponent<VFXManager>().EnableVFX();
            // Active the shockwave box and expand it over a second
            GetComponent<Boss>().audioManager.Play3DSound("BossSlam", shockwave.transform.position);
            shockwave.SetActive(true);
            shockwave.transform.DOScaleX(4, 0.3f);
            shockwave.transform.DOScaleZ(4, 0.3f);
            //sword.transform.DOComplete();

            return TaskStatus.Success;
        }
    }
}
