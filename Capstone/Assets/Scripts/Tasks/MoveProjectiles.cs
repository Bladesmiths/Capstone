using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone.Testing
{
    public class MoveProjectiles : Action
    {

        public override void OnStart()
        {
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            foreach(GameObject projectile in gameObject.GetComponent<Boss>().activeProjectiles)
            {
                projectile.GetComponent<TestingProjectile>().canMove = true;
            }
            
            return TaskStatus.Success;
        }
    }
}
