using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone
{
    public class IsAlive : Conditional
    {
        public SharedGameObject characterShared;
        private GameObject character;

        public override void OnStart()
        {
            character = characterShared.Value;
        }

        public override TaskStatus OnUpdate()
        {
            if (character.GetComponent<Character>().Health <= 0)
                return TaskStatus.Failure;
            else
                return TaskStatus.Success;
        }
    }
}
