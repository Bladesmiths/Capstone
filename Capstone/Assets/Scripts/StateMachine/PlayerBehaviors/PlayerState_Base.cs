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
    public class PlayerState_Base : StateMachineBehaviour
    {
        public PlayerCondition id;

        public PlayerCondition ID { get { return id; } set { id = value; } }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<Player>().SpeedUpdate(stateInfo);
            animator.GetComponent<Player>().ResetAnimationParameters();
        }
    }








    
}
