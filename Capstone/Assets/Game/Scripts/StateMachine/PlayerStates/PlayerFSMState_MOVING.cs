using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state that sllows for the player to move
    /// </summary>
    public class PlayerFSMState_MOVING : PlayerFSMState
    {
        public float timer;

        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;

        private int _animIDForward;
        private int _animIDDodge;
        private int _animIDMotionSpeed;
        private float _animBlend;
        private bool _hasAnimator;
        private CharacterController _controller;

        private float _speed;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity = 0.0f;
        private float _terminalVelocity = 53.0f;
        public float SpeedChangeRate = 10.0f;
        public float RotationSmoothTime = 0.12f;

        public bool Grounded = true;
        public bool isGrounded = true;
        public float GroundedOffset = -0.10f;
        public float GroundedRadius = 0.20f;
        public LayerMask GroundLayers;
        private int _animIDGrounded;

        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        private GameObject camera;


        public float RunSpeed = 10.0f;
        public float WalkSpeed = 4.0f;
        public float MoveSpeedCurrentMax = 10.0f;


        public PlayerFSMState_MOVING(Player player, PlayerInputsScript input, Animator animator, LayerMask layers)
        {
            _player = player;
            _input = input;
            _animator = animator;
            GroundLayers = layers;
        }

        public override void Tick()
        {
            _hasAnimator = _player.TryGetComponent(out _animator);


            //if(_input.move == Vector2.zero)
            //{
            //    timer += Time.deltaTime;

            //}

            //Vector2 movement = _input.move.normalized * (10 * Time.deltaTime);
            //_controller.Move(new Vector3(movement.x, 0, movement.y));


            if (_controller.isGrounded)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }



            Vector3 inputDirection = Vector3.zero;
            Vector3 targetDirection = Vector3.zero;


            //float targetSpeed = _input.move.normalized.magnitude * MoveSpeedCurrentMax;
            float targetSpeed = _input.move.magnitude * MoveSpeedCurrentMax;

            if (_input.move.magnitude > 1)
            {
                targetSpeed = _input.move.normalized.magnitude * MoveSpeedCurrentMax;
            }

            if (targetSpeed > 0 && targetSpeed < WalkSpeed)
            {
                targetSpeed = WalkSpeed;
            }

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;


            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude; //maybe normalize this instead?

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // Animator input
            //_animator.SetFloat(_animIDForward, _speed / targetSpeed);
            _animBlend = Mathf.Lerp(_animBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            //Debug.Log(_speed);

            // normalise input direction
            inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(_player.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                // rotate to face input direction relative to camera position
                _player.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            }
            targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;


            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


            //GroundedCheck();

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDForward, _animBlend);
                //_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }

        }

        public override void OnEnter()
        {
            _animIDForward = Animator.StringToHash("Forward");
            _animBlend = 0;

            //timer = 0;
            _controller = _player.GetComponent<CharacterController>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");

        }

        public override void OnExit()
        {

        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(_player.transform.position.x, _player.transform.position.y - GroundedOffset, _player.transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);



        }
    }

}
