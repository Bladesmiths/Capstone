using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;
using System.Linq;
using UnityEngine.Animations;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the Player is dead
    /// </summary>
    public class PlayerState_DEATH : PlayerState_Base
    {
        Player _player;
        private Animator _animator;
        private int _animIDDamaged;
        private int _animIDDeath;
        private float _animDuration;
        private float _timer;

        private void OnEnable()
        {
            
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // If the player is dead and just died (fadeToBlack is still occuring)
            if (id == PlayerCondition.F_Dying)
            {
                if (_timer >= _animDuration)
                {
                    _player.StartCoroutine(_player.FadeToBlack());
                }
                else
                {
                    _timer += Time.deltaTime;
                }
            }

            // Once the screen has faded to black
            if (_player.hasFadedToBlack)
            {
                // Respawn the player to the most recent respawn point
                _player.Respawn();
            }
            _player.damaged = false;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _player = animator.GetComponent<Player>();
            //id = PlayerCondition.F_Dead;
            _animator = animator;
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Player.instance.Inputs.ResetAttackNums();

            // Assign damaged & dead paramater ids
            _animIDDamaged = Animator.StringToHash("Damaged");
            _animIDDeath = Animator.StringToHash("Dead");
            _animDuration = animator.runtimeAnimatorController.animationClips.
                Where(clip => clip.name == "Dying").ToArray()[0].length - 0.75f;

            _player.inState = true;
            _player.justDied = true;
            _player.canDmg = false;
            _player.ResetChipDamage();
            _player.damaged = false;
            _player.transform.Find("TargetLockManager").GetComponent<TargetLock>().Active = false;

            //_animator.SetBool(_animIDDeath, false);
            //_animator.SetTrigger(_animIDDamaged);

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _player.inState = false;

            if (id == PlayerCondition.F_Dead)
            {
                _animator.SetBool(_animIDDeath, false);
                _player.damaged = false;
                _player.canDmg = true;
            }

            _timer = 0;
        }
    }

}
