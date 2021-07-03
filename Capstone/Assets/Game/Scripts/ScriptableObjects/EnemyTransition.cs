using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bladesmiths.Capstone
{
    public class EnemyTransition : Transition
    {
        //[EnumFlagsAttribute] EnemyCondiiton enemyCondiitons;
        public List<Func<bool>> Conditions { get; set; }
        //public Dictionary<EnemyCondition, Func<bool>> enemyConditionsRef;


        private void Awake()
        {

        }

    }
}
