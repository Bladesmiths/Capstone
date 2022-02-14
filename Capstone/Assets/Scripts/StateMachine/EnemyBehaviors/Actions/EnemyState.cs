using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Base class for possible states
    /// </summary>
    public abstract class EnemyState : IState
    {
        protected EnemyCondition id;

        public EnemyCondition ID { get; set; }

        public abstract void Tick();        

        public abstract void OnEnter();

        public abstract void OnExit();
        

    }    
}
