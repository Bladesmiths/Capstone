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
    public class PlayerState_JUMP : PlayerState_Base
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


        public PlayerState_JUMP(Player player, PlayerInputsScript input, LayerMask layers, float landTimeout)
        {
            _player = player;
            _input = input;
            GroundLayers = layers;
            isGrounded = true;
            _speed = 15;
            _landTimeout = landTimeout;
            id = PlayerCondition.F_Jumping;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("<color=red>LandTimeout: </color>" + LandTimeoutDelta);

        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _player = animator.GetComponent<Player>();
            _input = _player.GetComponent<PlayerInputsScript>();
            //GroundLayers = layers;
            isGrounded = true;
            _speed = 15;
            //_landTimeout = landTimeout;
            id = PlayerCondition.F_Jumping;
            base.OnStateEnter(animator, stateInfo, layerIndex);

            _hasAnimator = _player.TryGetComponent(out _animator);
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _controller = _player.gameObject.GetComponent<CharacterController>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");

            isGrounded = false;
            controllerVelocity = Vector2.zero;

            LandTimeoutDelta = -1.0f;

            // Testing
            ((TestDataInt)GameObject.Find("TestingController").GetComponent<TestingController>().ReportedData["numJumps"]).Data.CurrentValue++;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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

            }

        }

    }
}
