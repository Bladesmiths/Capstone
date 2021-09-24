using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the player is dodging
    /// TODO: will need to re-write as this is only a temp interaction for what it feels like
    /// </summary>
    public class PlayerFSMState_DODGE : PlayerFSMState
    {
        public float timer;
        public float dmgTimer;

        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;

        private int _animIDSpeed;
        private int _animIDDodge;
        private int _animIDMotionSpeed;
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

        public bool canDmg = true;

        public PlayerFSMState_DODGE(Player player, PlayerInputsScript input, Animator animator, LayerMask layers)
        {
            _player = player;
            _input = input;
            _animator = animator;
            GroundLayers = layers;
        }

        public override void Tick()
        {
            timer += Time.deltaTime;
            _input.dodge = false;

            dmgTimer += Time.deltaTime;
            if (dmgTimer >= 0.1f)
            {
                canDmg = true;
                _player.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;

            }
            else
            {
                _player.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.blue;

            }

            //Vector2 movement = _input.move.normalized * (10 * Time.deltaTime);
            //_controller.Move(new Vector3(movement.x, 0, movement.y));


            if (Grounded)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }


            Vector3 inputDirection = Vector3.zero;
            Vector3 targetDirection = Vector3.zero;


            float targetSpeed = 20;

            //if (_input.move == Vector2.zero) targetSpeed = 0.0f;


            //if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;



            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * 1, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            inputDirection = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).normalized;

            if (inputDirection.magnitude == 0)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _player.transform.eulerAngles.y;
                inputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward * -1;

                //inputDirection = new Vector3(0, 0, -1) + new Vector3(_player.transform.rotation.eulerAngles.y, 0.0f, 0.0f);

                inputDirection.Normalize();

            }

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            //if (_input.move != Vector2.zero)
            //{
            //    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
            //    float rotation = Mathf.SmoothDampAngle(_player.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            //    // rotate to face input direction relative to camera position
            //    _player.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            //    targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            //}


            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            // move the player
            _controller.Move(inputDirection * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


            GroundedCheck();

        }

        public override void OnEnter()
        {
            timer = 0;
            dmgTimer = 0;
            canDmg = false;
            _controller = _player.GetComponent<CharacterController>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");

            // Testing
            ((TestDataInt)GameObject.Find("TestingController").GetComponent<TestingController>().ReportedData["numDodges"]).Data.CurrentValue++;
        }

        public override void OnExit()
        {
            canDmg = true;
            _controller.SimpleMove(Vector3.zero);
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(_player.transform.position.x, _player.transform.position.y - GroundedOffset, _player.transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);


        }
    }
}
