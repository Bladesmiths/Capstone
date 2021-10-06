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

        private int _animIDBlock;
        private bool _hasAnimator;

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

        }

        public override void OnEnter()
        {
            // Turns the block detector box on
            //playerBlockBox.SetActive(true);

            _animIDBlock = Animator.StringToHash("Block");

            if (_animator != null)
            {
                _hasAnimator = true;
            }
            else
            {
                _hasAnimator = false;
            }

            _animator.SetBool(_animIDBlock, true);

            _sword.GetComponent<Rigidbody>().detectCollisions = true;
        }

        public override void OnExit()
        {
            // Turns the block detector box off
            //playerBlockBox.SetActive(false);

            // Change the color back to white
            playerBlockBox.GetComponent<MeshRenderer>().material.color = Color.white;

            _sword.GetComponent<Rigidbody>().detectCollisions = false;
            _animator.SetBool(_animIDBlock, false);
        }

    }

}
