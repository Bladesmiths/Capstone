using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Base class for possible states
    /// </summary>
    public class EnemyFSMState : IState
    {
        protected EnemyCondition id;

        public EnemyCondition ID { get; set; }

        public virtual void Tick()
        {

        }

        public virtual void OnEnter()
        {

        }

        public virtual void OnExit()
        {

        }


    }    
}
