using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    [CreateAssetMenu(fileName = "EnemyTransition", menuName = "ScriptableObjects/EnemyTransition")]
    public class EnemyTransition : ScriptableObject
    {
        [SerializeField] private EnemyCondition eValue;

        
        public EnemyCondition EValue { get; }

        //public EnemyTransition(IState to, IState from, Func<bool> condition)
        //{
        //    To = to;
        //    From = from;
        //    Condition = condition;

        //}

    }
}
