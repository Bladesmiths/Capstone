using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class CanSee : Conditional
    {
        private Transform player;
        public float fieldOfView;

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

            if (InSight(player, newFOV))
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }

        /// <summary>
        /// Checks to see if the Enemy can see the Player
        /// </summary>
        /// <param name="target">The Player</param>
        /// <param name="fov">The Field of View</param>
        /// <returns></returns>
        public bool InSight(Transform target, float fov)
        {
            Vector3 dir = target.position - transform.position;
            return Vector3.Angle(dir, transform.forward) < fov;
        }


    }
}
