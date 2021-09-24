using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;


namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Base class for possible states for the Player
    /// </summary>
    public class PlayerFSMState : IState
    {
        protected PlayerCondition id;
        
        public PlayerCondition ID { get; set; }

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
