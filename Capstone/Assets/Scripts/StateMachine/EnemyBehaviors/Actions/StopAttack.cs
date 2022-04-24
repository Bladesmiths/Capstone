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
            //base.OnStart();
            GetComponent<Enemy>().CanHit = false;
            GetComponent<Enemy>().canMove = false;
            GetComponent<Enemy>().isAttacking = false;
        }
               
        public override void OnEnd()
        {
            //base.OnEnd();
            GetComponent<Enemy>().isAttacking = false;

        }

    }
}