using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the player is not moving
    /// </summary>
    public class PlayerState_IDLE : PlayerState_Base
    {
        private Animator _animator;

        private int _animIDIdle;
        private int _animIDForward;
       
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            id = PlayerCondition.F_Idle;

            base.OnStateEnter(animator, stateInfo, layerIndex);
            _animIDIdle = Animator.StringToHash("Idle");
            _animIDForward = Animator.StringToHash("Forward");
            animator.SetFloat(_animIDForward, 0.0f);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(_animIDIdle, false);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(_animIDIdle, true);

        }       

    }

}
