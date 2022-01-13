using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the Player is parrying enemy attacks
    /// </summary>
    public class PlayerState_PARRYATTEMPT : PlayerState_Base
    {
        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;

        // The block object that notifies if the player has blocked something
        public GameObject _playerParryBox;

        //public PlayerState_PARRYATTEMPT(Player player, PlayerInputsScript input, Animator animator,
        //    GameObject playerParryDetector)
        //{
        //    _playerParryBox = playerParryDetector;
        //    _player = player;
        //    _input = input;
        //    _animator = animator;
        //    id = PlayerCondition.F_ParryAttempt;
        //}

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //_playerParryBox = playerParryDetector;
            _player = animator.GetComponent<Player>();
            _input = _player.GetComponent<PlayerInputsScript>();
            _animator = animator;
            id = PlayerCondition.F_ParryAttempt;
            base.OnStateEnter(animator, stateInfo, layerIndex);


            // Start the timer to enable the parry system after a set time
            _player.StartCoroutine(ParryDelayTimer());
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Disable everything on exit
            _input.parry = false;
            _playerParryBox.SetActive(false);
            _player.parryEnd = false;
        }

        /// <summary>
        /// Coroutine to enable the parry checking system after a delay
        /// </summary>
        /// <returns>Coroutine variable</returns>
        private IEnumerator ParryDelayTimer()
        {
            yield return new WaitForSeconds(_player.ParryDelay);

            _playerParryBox.SetActive(true);

            _player.StartCoroutine(ParryLengthTimer()); 
        }

        /// <summary>
        /// Coroutine to disable the parry system after a set amount of time
        /// </summary>
        /// <returns>Coroutine variable</returns>
        private IEnumerator ParryLengthTimer()
        {
            yield return new WaitForSeconds(_player.ParryLength);

            _playerParryBox.SetActive(false);
            _player.StartCoroutine(ParryCooldownTimer());
        }

        /// <summary>
        /// Coroutine to notify the player when the parry cooldown is over and the player can do
        /// other things again
        /// </summary>
        /// <returns>Coroutine variable</returns>
        private IEnumerator ParryCooldownTimer()
        {
            yield return new WaitForSeconds(_player.ParryCooldown);

            _player.parryEnd = true;
        }
    }
}
