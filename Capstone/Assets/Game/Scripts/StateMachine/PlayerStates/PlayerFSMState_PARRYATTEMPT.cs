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
    public class PlayerFSMState_PARRYATTEMPT : PlayerFSMState
    {
        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;

        // The block object that notifies if the player has blocked something
        private GameObject _playerParryBox;

        public PlayerFSMState_PARRYATTEMPT(Player player, PlayerInputsScript input, Animator animator,
            GameObject playerParryDetector)
        {
            _playerParryBox = playerParryDetector;
            _player = player;
            _input = input;
            _animator = animator;
            id = PlayerCondition.F_ParryAttempt;
        }

        public override void Tick() { }

        public override void OnEnter()
        {
            // Start the timer to enable the parry system after a set time
            _player.StartCoroutine(ParryDelayTimer()); 
        }

        public override void OnExit()
        {
            // Disable everything on exit
            _input.parry = false;
            _playerParryBox.SetActive(false);
            _player.parryEnd = false;

            // Chip damage is reset regardless because
            // to exit this state a parry has to be successful or
            // the parry window has to end
            _playerParryBox.GetComponent<ParryCollision>().ResetChipDamage();
        }

        /// <summary>
        /// Coroutine to enable the parry checking system after a delay
        /// </summary>
        /// <returns>Coroutine variable</returns>
        private IEnumerator ParryDelayTimer()
        {
            yield return new WaitForSeconds(_player.CurrentBalancingData.ParryDelay);

            _playerParryBox.SetActive(true);

            _player.StartCoroutine(ParryLengthTimer()); 
        }

        /// <summary>
        /// Coroutine to disable the parry system after a set amount of time
        /// </summary>
        /// <returns>Coroutine variable</returns>
        private IEnumerator ParryLengthTimer()
        {
            yield return new WaitForSeconds(_player.CurrentBalancingData.ParryLength);

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
            yield return new WaitForSeconds(_player.CurrentBalancingData.ParryCooldown);

            _player.parryEnd = true;
        }
    }
}
