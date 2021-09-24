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
    public class PlayerFSMState_IDLE : PlayerFSMState
    {
        private Animator _animator;

        private int _animIDIdle;
        private int _animIDForward;

        public PlayerFSMState_IDLE()
        {

        }

        public PlayerFSMState_IDLE(Animator animator)
        {
            _animator = animator;
        }

        public override void Tick()
        {
            _animator.SetBool(_animIDIdle, true);
        }

        public override void OnEnter()
        {
            _animIDIdle = Animator.StringToHash("Idle");
            _animIDForward = Animator.StringToHash("Forward");
            _animator.SetFloat(_animIDForward, 0.0f);
        }

        public override void OnExit()
        {
            _animator.SetBool(_animIDIdle, false);
        }

    }

}
