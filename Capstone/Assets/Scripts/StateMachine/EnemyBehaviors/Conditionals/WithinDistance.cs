using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class WithinDistance : Conditional
    {
        private Transform player;
        public float fieldOfView;
        public float distance;


        public override void OnAwake()
        {
            player = Player.instance.transform;
        }

        public override TaskStatus OnUpdate()
        {
            float newFOV = fieldOfView;
            if (GetComponent<Enemy>().InCombat)
            {
                newFOV = 360;
            }
            
            if (InSight(player, newFOV) && Vector3.Distance(player.position, transform.position) <= distance)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

        public bool InSight(Transform target, float fov)
        {
            Vector3 dir = target.position - transform.position;
            return Vector3.Angle(dir, transform.forward) < fov;
        }


    }
}
