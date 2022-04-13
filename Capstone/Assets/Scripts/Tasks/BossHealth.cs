using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone
{
    public class BossHealth : Conditional
    {
        enum Operator
        {
            LessThanEqualTo,
            GreaterThanEqualTo,
            EqualTo
        }

        [SerializeField] private Operator ifOperator;
        [SerializeField] private float percentageHealth;

        public override void OnStart()
        {

        }

        public override TaskStatus OnUpdate()
        {
            switch (ifOperator)
            {
                case Operator.LessThanEqualTo:
                    if (gameObject.GetComponent<Boss>().Health / gameObject.GetComponent<Boss>().MaxHealth <= percentageHealth / 100)
                        return TaskStatus.Success;
                    break;
                case Operator.GreaterThanEqualTo:
                    if (gameObject.GetComponent<Boss>().Health / gameObject.GetComponent<Boss>().MaxHealth >= percentageHealth / 100)
                        return TaskStatus.Success;
                    break;
                case Operator.EqualTo:
                    if (gameObject.GetComponent<Boss>().Health / gameObject.GetComponent<Boss>().MaxHealth == percentageHealth / 100)
                        return TaskStatus.Success;
                    break;
            }

            return TaskStatus.Failure;
        }
    }
}
