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
    public class PlayerState_PARRYSUCCESS : PlayerState_Base
    {
        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;

        // The block object that notifies if the player has blocked something
        public GameObject _playerParryBox;

        // The ID of the parry success paramater in the Player's animator controller
        private int _animIDParrySuccess;

        //public PlayerState_PARRYSUCCESS(Player player, PlayerInputsScript input, Animator animator,
        //    GameObject playerParryDetector)
        //{
        //    _playerParryBox = playerParryDetector;
        //    _player = player;
        //    _input = input;
        //    _animator = animator;
        //    id = PlayerCondition.F_ParrySuccess;
        //}

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _player = animator.GetComponent<Player>();
            _playerParryBox = _player.ParryDetectorObject;
            _input = _player.GetComponent<PlayerInputsScript>();
            _animator = animator;
            id = PlayerCondition.F_ParrySuccess;
            base.OnStateEnter(animator, stateInfo, layerIndex);



            // Start the parry cooldown timer coroutine 
            // Because, to enter this state, parry must be over
            _player.StartCoroutine(ParryCooldownTimer());

            // Turn off parry object for the same reason
            _playerParryBox.SetActive(false);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Disable everything on exit
            _input.parry = false;
            _playerParryBox.SetActive(false);
            _player.parryEnd = false;
            //_player.parrySuccessful = false;
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
