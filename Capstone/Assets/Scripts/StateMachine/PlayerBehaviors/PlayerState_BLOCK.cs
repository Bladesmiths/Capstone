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
    public class PlayerState_BLOCK : PlayerState_Base
    {
        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;
        private GameObject _sword;
        private Vector3 inputDirection;
        private Quaternion _targetRotation;
        private GameObject camera;
        // The ID of the block paramater in the Player's animator controller
        private int _animIDBlock;
        private bool entered;

        // The block object that notifies if the player has blocked something
        public GameObject playerBlockBox;
        //public PlayerState_BLOCK(Player player, PlayerInputsScript input, Animator animator, 
        //    GameObject sword, GameObject playerBlockDetector)
        //{
        //    _player = player;
        //    _input = input;
        //    _animator = animator;
        //    _sword = sword;

        //    playerBlockBox = playerBlockDetector;
        //    id = PlayerCondition.F_Blocking;
        //}

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (entered)
            {
                entered = false;
                // Allows for the player to snap to the direction they are inputting
                if (_input.move == Vector2.zero)
                {
                    _targetRotation = Quaternion.Euler(0.0f, _player.transform.eulerAngles.y, 0.0f);

                    inputDirection = _targetRotation * Vector3.forward;
                }
                else if (_input.move != Vector2.zero)
                {
                    Vector3 inputMove = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

                    // rotate to face input direction relative to camera position
                    _targetRotation = Quaternion.Euler(0.0f, Mathf.Atan2(inputMove.x, inputMove.z) *
                        Mathf.Rad2Deg + camera.transform.eulerAngles.y, 0.0f);

                    _player.transform.rotation = _targetRotation;

                    inputDirection = _targetRotation * Vector3.forward;
                }
            }

        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _player = animator.GetComponent<Player>();
            _input = _player.GetComponent<PlayerInputsScript>();
            _animator = animator;
            playerBlockBox = _player.BlockDetector;

            //_sword = sword;

            //playerBlockBox = _player.transform.GetComponentInChildren<BlockCollision>().gameObject;
            id = PlayerCondition.F_Blocking;
            base.OnStateEnter(animator, stateInfo, layerIndex);
            entered = true;


            camera = GameObject.FindGameObjectWithTag("MainCamera");

            //// Allows for the player to snap to the direction they are inputting
            //if (_input.move == Vector2.zero)
            //{
            //    _targetRotation = Quaternion.Euler(0.0f, _player.transform.eulerAngles.y, 0.0f);

            //    inputDirection = _targetRotation * Vector3.forward;
            //}
            //else
            //{
            //    Vector3 inputMove = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            //    // rotate to face input direction relative to camera position
            //    _targetRotation = Quaternion.Euler(0.0f, Mathf.Atan2(inputMove.x, inputMove.z) *
            //        Mathf.Rad2Deg + camera.transform.eulerAngles.y, 0.0f);

            //    _player.transform.rotation = _targetRotation;

            //    inputDirection = _targetRotation * Vector3.forward;
            //}

            // Turns the block detector box on
            playerBlockBox.SetActive(true);
            playerBlockBox.GetComponent<BlockCollision>().Active = true;

            //// Assign block paramater id
            _animIDBlock = Animator.StringToHash("Block");

            //// Set blocking id to true
            //_animator.SetBool(_animIDBlock, true);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Turns the block detector box off
            playerBlockBox.SetActive(false);

            // Turning block collision off and resetting its chip damage
            playerBlockBox.GetComponent<BlockCollision>().Active = false;

            // Change the color back to white
            playerBlockBox.GetComponent<MeshRenderer>().material.color = Color.white;

            // Set the sword to not detect collisions
            // and turn off blocking paramater
            _animator.SetBool(_animIDBlock, false);
        }

    }

}
