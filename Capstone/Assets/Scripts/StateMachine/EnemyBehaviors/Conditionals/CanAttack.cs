using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class CanAttack : Conditional
    {
        //public SharedTransform player;
        //public float fieldOfView;
        //public float distance;

        public override void OnAwake()
        {
            //player.Value = Player.instance.transform;
        }

        public override TaskStatus OnUpdate()
        {
            if (GetComponent<Enemy>().CanHit)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

        


    }
}
