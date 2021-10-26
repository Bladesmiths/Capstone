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

        // The block object that notifies if the player has blocked something
        private GameObject playerBlockBox;
        public PlayerFSMState_BLOCK(Player player, PlayerInputsScript input, Animator animator, 
            GameObject sword, GameObject playerBlockDetector)
        {
            _player = player;
            _input = input;
            _animator = animator;
            _sword = sword;

            playerBlockBox = playerBlockDetector;
            id = PlayerCondition.F_Blocking;
        }

        public override void Tick() { }

        public override void OnEnter()
        {
            // Turns the block detector box on
            playerBlockBox.SetActive(true);
            playerBlockBox.GetComponent<BlockCollision>().Active = true;
            //_sword.GetComponent<Rigidbody>().detectCollisions = true;

            // Assign block paramater id
            _animIDBlock = Animator.StringToHash("Block");

            // Set blocking id to true
            _animator.SetBool(_animIDBlock, true);

            // Set the sword to detect collisions
            //_sword.GetComponent<Rigidbody>().detectCollisions = false;
        }

        public override void OnExit()
        {
            // Turns the block detector box off
            playerBlockBox.SetActive(false);
            
            playerBlockBox.GetComponent<BlockCollision>().Active = false;
            playerBlockBox.GetComponent<BlockCollision>().ResetChipDamage(); 

            // Change the color back to white
            playerBlockBox.GetComponent<MeshRenderer>().material.color = Color.white;

            //_sword.GetComponent<Rigidbody>().detectCollisions = false;

            // Set the sword to not detect collisions
            // and turn off blocking paramater
            //_sword.GetComponent<Rigidbody>().detectCollisions = true;
            _animator.SetBool(_animIDBlock, false);
        }

    }

}
