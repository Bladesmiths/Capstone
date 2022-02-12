using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class OutsideDistance : Conditional
    {
        private Transform player;
        public float fieldOfView;
        public float distance;
        public float distVec;


        public override void OnAwake()
        {
            player = Player.instance.transform;
        }

        public override TaskStatus OnUpdate()
        {            
            player = Player.instance.transform;
            float newFOV = fieldOfView;
            if (GetComponent<Enemy>().InCombat)
            {
                newFOV = 360;
            }

            distVec = Vector3.Distance(player.position, transform.position);

            if (InSight(player, newFOV) && (distVec <= distance))
            {
                return TaskStatus.Failure;
            }
            distVec = 0;
            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
            base.OnEnd();

        }

        public bool InSight(Transform target, float fov)
        {
            Vector3 dir = target.position - transform.position;
            return Vector3.Angle(dir, transform.forward) < fov;
        }


    }
}
