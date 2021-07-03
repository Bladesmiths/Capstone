using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    [CreateAssetMenu(fileName = "EnemyTransition", menuName = "ScriptableObjects/EnemyTransition")]
    public class EnemyTransition : Transition
    {
        [EnumFlagAttribute] EnemyState enemyCondiitons;
        public List<Func<bool>> Conditions { get; set; }
        public Dictionary<EnemyState, Func<bool>> enemyConditionsRef;


        private void Awake()
        {

        }

    }
}
