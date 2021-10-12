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
        private float _targetRotation = 0.0f;
        private GameObject camera;

        public float Timer { get { return timer; } }

        public PlayerFSMState_ATTACK(Player player, PlayerInputsScript input, Animator animator, GameObject sword)
        {
            _player = player;
            _input = input;
            _animator = animator;
            _sword = sword;
            _sword.GetComponent<Rigidbody>().detectCollisions = false;
            id = PlayerCondition.F_Attacking;
        }

        public override void Tick()
        {
            timer += Time.deltaTime;

            _sword.GetComponent<Rigidbody>().detectCollisions = true;
            
            _input.attack = false;

            if (_controller.isGrounded)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }

            Vector3 targetDirection = Vector3.zero;

            float targetSpeed = 2f;

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
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

            _targetRotation = _player.transform.eulerAngles.y;

            inputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        }

        public override void OnExit()
        {
            _sword.GetComponent<Rigidbody>().detectCollisions = false;
            _animator.SetBool(_animIDAttack, false);
        }

    }

}
