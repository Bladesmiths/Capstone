using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    //[CreateAssetMenu(fileName = "EnemyTransition", menuName = "ScriptableObjects/EnemyTransition")]
    public class EnemyTransition : Transition
    {
        //[EnumFlagsAttribute] EnemyState enemyCondiitons;

        public EnemyTransition(IState to, IState from, Func<bool> condition)
            : base(to, from, condition)
        {
            To = to;
            From = from;
            Condition = condition;

        }

    }
}
