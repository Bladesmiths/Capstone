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
        public float distance;
        public float distVec;


        public override void OnAwake()
        {
            player = Player.instance.transform;
        }

        public override TaskStatus OnUpdate()
        {
            distVec = Vector3.Distance(player.position, transform.position);

            if (distVec <= distance)
            {
                distVec = 10000;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
