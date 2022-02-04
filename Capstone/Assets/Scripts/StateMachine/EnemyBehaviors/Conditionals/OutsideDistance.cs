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


        public override void OnAwake()
        {
            player = Player.instance.transform;
        }

        public override TaskStatus OnUpdate()
        {
            Debug.Log(Vector3.Distance(player.position, transform.position));
            //if (Vector3.Distance(player.Value.position, transform.position) <= distance)
            //{
            //    return TaskStatus.Failure;
            //}
            if(GetComponent<Enemy>().InCombat)
            {
                fieldOfView = 360;
            }

            if (InSight(player, fieldOfView) && Vector3.Distance(player.position, transform.position) <= distance)
            {
                return TaskStatus.Failure;
            }            
            return TaskStatus.Success;
        }

        public bool InSight(Transform target, float fov)
        {
            Vector3 dir = target.position - transform.position;
            return Vector3.Angle(dir, transform.forward) < fov;
        }


    }
}
