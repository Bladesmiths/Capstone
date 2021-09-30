using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the player is jumping
    /// </summary>
    public class PlayerFSMState_JUMP : PlayerFSMState
    {
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        public float LandTimeoutDelta = 0;

        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;
        private float _landTimeout;

        private Player _player;
        private PlayerInputsScript _input;
        private CharacterController _controller;
        private Animator _animator;
        private int _animIDJump;
        private int _animIDFreeFall;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        public bool Grounded = true;
        public bool isGrounded = true;
        public float GroundedOffset = -0.10f;
        public float GroundedRadius = 0.20f;
        public LayerMask GroundLayers;
        private int _animIDGrounded;

        public float JumpHeight = 1.2f;
        public float Gravity = -20.0f;

        private Vector3 controllerVelocity;


        public bool _hasAnimator;

        private float _speed;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;

        public float SpeedChangeRate = 10.0f;
        public float RotationSmoothTime = 0.20f;

        private GameObject camera;
        Vector3 currentSpeed = Vector3.zero;

        private Vector3 maxSpeed;


        public PlayerFSMState_JUMP(Player player, PlayerInputsScript input, LayerMask layers, float landTimeout)
        {
            _player = player;
            _input = input;
            GroundLayers = layers;
            isGrounded = true;
            _speed = 15;
            _landTimeout = landTimeout;
            id = PlayerCondition.F_Jumping;
        }

        public override void Tick()
        {
            if (_controller.isGrounded)
            {
                //Debug.Log("<color=brown>Grounded</color>");

                isGrounded = true;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                    _verticalVelocity = 0f;
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump/* && _jumpTimeoutDelta <= 0.0f*/)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    controllerVelocity = _controller.velocity.normalized * _input.move.magnitude;

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // Get the current velocity of the player


                // Land Timer
                if (LandTimeoutDelta >= 0.0f)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                    _animator.SetBool(_animIDGrounded, true);

                    LandTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // move the player
                    _controller.Move(new Vector3(controllerVelocity.x * 15, _verticalVelocity, controllerVelocity.z * 15) * Time.deltaTime);
                }
            }

            else
            {
                //Debug.Log("<color=blue>In Air</color>");

                LandTimeoutDelta = _landTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                    _animator.SetBool(_animIDGrounded, false);
                }

                _input.jump = false;

                Vector3 inputDirection = Vector3.zero;
                Vector3 targetDirection = Vector3.zero;
                Vector3 movementVector = Vector3.zero;

                //_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);


                float targetSpeed = _input.move.magnitude * 10;

                //if (_input.move == Vector2.zero) targetSpeed = 0.0f;



                // a reference to the players current horizontal velocity
                float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

                float speedOffset = 0.1f;
                //float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

                // accelerate or decelerate to target speed
                if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
                {
                    // creates curved result rather than a linear one giving a more organic speed change
                    // note T in Lerp is clamped, so we don't need to clamp our speed
                    _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * 1f, Time.deltaTime * SpeedChangeRate);

                    // round speed to 3 decimal places
                    _speed = Mathf.Round(_speed * 1000f) / 1000f;
                }
                else
                {
                    _speed = targetSpeed;
                }

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
                    targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                }
                //targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                //targetDirection += controllerVelocity.normalized;

                //if (targetDirection.x <= 0.01 || targetDirection.x >= -0.01)
                //{
                //    targetDirection = new Vector3(0, targetDirection.y, targetDirection.z);

                //}

                //else if(targetDirection.x > 0)
                //{
                //    targetDirection -= new Vector3(Time.deltaTime, 0, 0);

                //}

                //else if(targetDirection.x < 0)
                //{
                //    targetDirection += new Vector3(Time.deltaTime, 0, 0);

                //}

                //if(targetDirection.z <= 0.01 || targetDirection.z >= -0.01)
                //{
                //    targetDirection = new Vector3(targetDirection.x, targetDirection.y, 0);

                //}

                //else if (targetDirection.z > 0)
                //{
                //    targetDirection -= new Vector3(Time.deltaTime, 0, 0);

                //}

                //else if(targetDirection.z < 0)
                //{
                //    targetDirection += new Vector3(0, 0, Time.deltaTime);

                //}

                // move the player
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }





            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            //Debug.Log("<color=red>LandTimeout: </color>" + LandTimeoutDelta);

        }

        public override void OnEnter()
        {
            _hasAnimator = _player.TryGetComponent(out _animator);
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _controller = _player.gameObject.GetComponent<CharacterController>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");

            isGrounded = false;
            controllerVelocity = Vector2.zero;

            //currentSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).normalized;

            LandTimeoutDelta = -1.0f;

            // Testing
            ((TestDataInt)GameObject.Find("TestingController").GetComponent<TestingController>().ReportedData["numJumps"]).Data.CurrentValue++;
        }

        public override void OnExit()
        {
            if (_hasAnimator)
            {
            }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(_player.transform.position.x, _player.transform.position.y - GroundedOffset, _player.transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                //_animator.SetBool(_animIDGrounded, Grounded);
            }

        }

    }
}
