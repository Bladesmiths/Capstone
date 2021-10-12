using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the Player is blocking enemy attacks
    /// </summary>
    public class PlayerFSMState_BLOCK : PlayerFSMState
    {
        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;
        private GameObject _sword;

        // The ID of the block paramater in the Player's animator controller
        private int _animIDBlock;
        private bool _hasAnimator;

        // The block rectangle that notifies if the player has blocked something
        private GameObject playerBlockBox;
        public PlayerFSMState_BLOCK(Player player, PlayerInputsScript input, Animator animator, 
            GameObject sword, GameObject playerBlockDetector)
        {
            _player = player;
            _input = input;
            _animator = animator;
            _sword = sword;
            _sword.GetComponent<Rigidbody>().detectCollisions = false;

            playerBlockBox = playerBlockDetector;
            id = PlayerCondition.F_Blocking;
        }

        public override void Tick()
        {
            // Not sure what to do here yet, if anything
        }

        public override void OnEnter()
        {
            // Turns the block detector box on
            playerBlockBox.SetActive(true);
            playerBlockBox.GetComponent<BlockCollision>().Active = true;

            // Assign block paramater id
            _animIDBlock = Animator.StringToHash("Block");
            
            // Do we need this?
            if (_animator != null)
            {
                _hasAnimator = true;
            }
            else
            {
                _hasAnimator = false;
            }

            // Set blocking id to true
            _animator.SetBool(_animIDBlock, true);

            // Set the sword to detect collisions
            //_sword.GetComponent<Rigidbody>().detectCollisions = true;
        }

        public override void OnExit()
        {
            // Turns the block detector box off
            playerBlockBox.SetActive(false);
            playerBlockBox.GetComponent<BlockCollision>().Active = false;

            // Change the color back to white
            playerBlockBox.GetComponent<MeshRenderer>().material.color = Color.white;

            // Set the sword to not detect collisions
            // and turn off blocking paramater
            //_sword.GetComponent<Rigidbody>().detectCollisions = false;
            _animator.SetBool(_animIDBlock, false);
        }

    }

}
