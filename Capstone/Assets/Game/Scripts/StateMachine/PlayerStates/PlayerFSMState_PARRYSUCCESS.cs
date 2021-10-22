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
    public class PlayerFSMState_PARRYSUCCESS : PlayerFSMState
    {
        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;

        // The block object that notifies if the player has blocked something
        private GameObject _playerParryBox;

        // The ID of the parry success paramater in the Player's animator controller
        private int _animIDParrySuccess;

        public PlayerFSMState_PARRYSUCCESS(Player player, PlayerInputsScript input, Animator animator,
            GameObject playerParryDetector)
        {
            _playerParryBox = playerParryDetector;
            _player = player;
            _input = input;
            _animator = animator;
            id = PlayerCondition.F_ParrySuccess;
        }

        public override void Tick() { }

        public override void OnEnter()
        {
            //// Assign parry success paramater id
            //_animIDParrySuccess = Animator.StringToHash("Parry Successful");

            //// Set blocking id to true
            //_animator.SetTrigger(_animIDParrySuccess);

            // Start the parry cooldown timer coroutine 
            // Because, to enter this state, parry must be over
            _player.StartCoroutine(ParryCooldownTimer());

            // TUrn off parry object for the same reason
            _playerParryBox.SetActive(false);
        }

        public override void OnExit()
        {
            // Disable everything on exit
            _input.parry = false;
            _playerParryBox.SetActive(false);
            _player.parryEnd = false;
        }

        /// <summary>
        /// Coroutine to notify the player when the parry cooldown is over and the player can do
        /// other things again
        /// </summary>
        /// <returns>Coroutine variable</returns>
        private IEnumerator ParryCooldownTimer()
        {
            yield return new WaitForSeconds(_player.CurrentBalancingData.ParryCooldown);

            _player.parryEnd = true;
        }
    }
}
