using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class CantMove : Action
    {
        public CantMove()
        {

        }

        public override void OnStart()
        {
            base.OnStart();
            GetComponent<Enemy>().canMove = false;
        }

        public override TaskStatus OnUpdate()
        {
            return base.OnUpdate();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            GetComponent<Enemy>().canMove = true;

        }

    }
}