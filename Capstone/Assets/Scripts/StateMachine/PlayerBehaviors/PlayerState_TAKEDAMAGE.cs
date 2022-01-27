using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;
using System.Linq;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the player takes damage
    /// CURRENTLY NOT IN USE
    /// </summary>
    public class PlayerState_TAKEDAMAGE : PlayerState_Base
    {
        private Player _player;
        private Animator _animator;
        private int _animIDDamaged;

        public float Timer { get; set; }
        public float AnimDuration { get; private set; }

        override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            
        }

        private void OnEnable()
        {
            
        }

        //public PlayerState_TAKEDAMAGE(Player player, Animator animator)
        //{
        //    _player = player;
        //    id = PlayerCondition.F_TakingDamage;
        //    _animator = animator;

        //    // Assign damaged paramater id
        //    _animIDDamaged = Animator.StringToHash("Damaged");
        //    AnimDuration = animator.runtimeAnimatorController.animationClips.
        //        Where(clip => clip.name == "GettingHit").ToArray()[0].length / 1.5f;
        //}

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Timer += Time.deltaTime;
            //_player.damaged = false;
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            _player = animator.gameObject.GetComponent<Player>();
            id = PlayerCondition.F_TakingDamage;
            base.OnStateEnter(animator, stateInfo, layerIndex);

            _animator = animator;

            // Assign damaged paramater id
            _animIDDamaged = Animator.StringToHash("Damaged");
            AnimDuration = animator.runtimeAnimatorController.animationClips.
                Where(clip => clip.name == "GettingHit").ToArray()[0].length / 1.5f;

            //_player.damaged = false;
            Timer = 0;
            //_player.inState = true;
            //Debug.Log("AnimDuration: " + AnimDuration);

            //w_animator.SetTrigger(_animIDDamaged); 
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {            
            Timer = 0; 
            //_player.inState = false;
            //_player.damaged = false;

        }

    }

}
