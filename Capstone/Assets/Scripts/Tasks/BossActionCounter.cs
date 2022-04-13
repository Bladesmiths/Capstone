using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone
{
    public class BossActionCounter : Action
    {

        [SerializeField] private string variableName;
        [SerializeField] private float incrementAmount;
        [SerializeField] private float maxValue;

        public override void OnStart()
        {

        }

        public override TaskStatus OnUpdate()
        {
            if(GetComponent<Boss>().actionCounter.ContainsKey(variableName) == false)
            {
                GetComponent<Boss>().actionCounter[variableName] = incrementAmount;
            }

            if(GetComponent<Boss>().actionCounter[variableName] <= maxValue)
            {
                GetComponent<Boss>().actionCounter[variableName] += incrementAmount;
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
