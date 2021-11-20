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
    /// The state for when the Player is dead
    /// </summary>
    public class PlayerFSMState_DEATH : PlayerFSMState
    {
        Player _player;
        private Animator _animator;
        private int _animIDDeath;
        private float _animDuration;
        private float _timer; 

        public PlayerFSMState_DEATH(Player player, Animator animator)
        {
            _player = player;
            id = PlayerCondition.F_Dead;
            _animator = animator;

            // Assign damaged paramater id
            _animIDDeath = Animator.StringToHash("Dead");
            _animDuration = animator.runtimeAnimatorController.animationClips.
                Where(clip => clip.name == "Dying").ToArray()[0].length;
        }

        public override void Tick()
        {
            // If the player is dead and just died (fadeToBlack is still occuring)
            if (_timer >= _animDuration)
            {
                _player.FadeToBlack();
            }
            else
            {
                _timer += Time.deltaTime;
            }

            // Once the screen has faded to black
            if (_player.hasFadedToBlack)
            {
                // Respawn the player to the most recent respawn point
                _player.Respawn();
            }
        }

        public override void OnEnter()
        {
            _player.inState = true;
            _player.justDied = true;
            _player.ParryDetector.ResetChipDamage();

            _animator.SetTrigger(_animIDDeath);
        }

        public override void OnExit()
        {
            _player.inState = false;

            _timer = 0; 
        }

    }

}
