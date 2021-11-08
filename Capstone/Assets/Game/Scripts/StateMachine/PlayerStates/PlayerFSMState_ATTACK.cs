using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the Player attacks
    /// </summary>
    public class PlayerFSMState_ATTACK : PlayerFSMState
    {
        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;
        private GameObject _sword;
        private CharacterController _controller;

        private int _animIDSpeed;
        private int _animIDAttack;
        private int _animIDMotionSpeed;
        private bool _hasAnimator;

        private float timer;
        private float currentHorizontalSpeed;
        private float _verticalVelocity = 0.0f;
        private float _terminalVelocity = 53.0f;
        public float Gravity = -15.0f;
        private Vector3 inputDirection;
        private Quaternion _targetRotation;
        private GameObject camera;

        public float Timer { get { return timer; } }

        public PlayerFSMState_ATTACK(Player player, PlayerInputsScript input, Animator animator, GameObject sword)
        {
            _player = player;
            _input = input;
            _animator = animator;
            _sword = sword;
            //_sword.GetComponent<Rigidbody>().detectCollisions = false;
            id = PlayerCondition.F_Attacking;
            //_sword.GetComponent<Rigidbody>().detectCollisions = false;
        }

        public override void Tick()
        {
            timer += Time.deltaTime;

            //_sword.GetComponent<Rigidbody>().detectCollisions = true;
            
            _input.attack = false;

            if (_controller.isGrounded)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }

            Vector3 targetDirection = Vector3.zero;

            float targetSpeed = 1.8f;

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            // move the player
            //_controller.Move(inputDirection.normalized * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        public override void OnEnter()
        {
            timer = 0;
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDAttack = Animator.StringToHash("Attack");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _controller = _player.GetComponent<CharacterController>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");
            if (_animator != null)
            {
                _hasAnimator = true;
            }
            else
            {
                _hasAnimator = false;
            }



            _animator.SetBool(_animIDAttack, true);

            if (_input.move == Vector2.zero)
            {
                _targetRotation = Quaternion.Euler(0.0f, _player.transform.eulerAngles.y, 0.0f);

                inputDirection = _targetRotation * Vector3.forward;
            }
            else
            {
                Vector3 inputMove = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

                // rotate to face input direction relative to camera position
                _targetRotation = Quaternion.Euler(0.0f, Mathf.Atan2(inputMove.x, inputMove.z) *
                    Mathf.Rad2Deg + camera.transform.eulerAngles.y, 0.0f);

                _player.transform.rotation = _targetRotation;

                inputDirection = _targetRotation * Vector3.forward;
            }
        }

        public override void OnExit()
        {
            //_sword.GetComponent<Rigidbody>().detectCollisions = false;
            _animator.SetBool(_animIDAttack, false);
        }

    }

}
