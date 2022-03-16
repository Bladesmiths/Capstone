using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class StopAttack : Action
    {
        public StopAttack()
        {

        }

        public override void OnStart()
        {
            base.OnStart();
            GetComponent<Enemy>().CanHit = false;
        }

        public override TaskStatus OnUpdate()
        {
            return base.OnUpdate();
        }

        public override void OnEnd()
        {
            base.OnEnd();            

        }

    }
}