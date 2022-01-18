using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;
using System.Linq;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the player is dodging
    /// TODO: will need to re-write as this is only a temp interaction for what it feels like
    /// </summary>
    public class PlayerState_DODGE : PlayerState_Base
    {
        public float timer;
        public float dmgTimer;

        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;
        private bool hasAnimator;
        private float speed;

        private int _animIDSpeed;
        private int _animIDDodge;
        private int _animIDMotionSpeed;
        private int _animIDForward;
        private int _animIDDodgeMove;
        private int animIDMoving;
        private float animBlend;
        private bool _hasAnimator;
        private CharacterController _controller;

        private float _speed;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity = 0.0f;
        private float _terminalVelocity = 53.0f;
        private float SpeedChangeRate = 10.0f;
        private float RotationSmoothTime = 0.12f;

        private bool Grounded = true;
        private bool isGrounded = true;
        public float GroundedOffset = -0.10f;
        public float GroundedRadius = 0.20f;
        public LayerMask GroundLayers;
        private int _animIDGrounded;

        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        private GameObject camera;

        public bool canDmg = true;

        private float currentHorizontalSpeed;
        private Vector3 inputDirection;
        private float rotationVelocity;

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateMove(animator, stateInfo, layerIndex);

            if (_input.dodge)
            {
                if (_input.move == Vector2.zero)
                {
                    _targetRotation = _player.transform.eulerAngles.y;

                    inputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.back;
                    speed = 0f;

                    animator.SetBool(animIDMoving, false);


                }
                else if (_input.move != Vector2.zero)
                {
                    Vector3 inputMove = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

                    // rotate to face input direction relative to camera position
                    _targetRotation = Mathf.Atan2(inputMove.x, inputMove.z) * Mathf.Rad2Deg + camera.transform.localEulerAngles.y;
                    speed = 10f;
                    _player.transform.rotation = Quaternion.Euler(0.0f, _targetRotation, 0.0f);
                    
                    inputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
                    animator.SetBool(animIDMoving, false);

                }
            }

        }

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            animIDMoving = Animator.StringToHash("Moving");

        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timer += Time.deltaTime;
            _input.dodge = false;
            

            dmgTimer += Time.deltaTime;
            if (dmgTimer >= 0.3f)
            {
                _player.canDmg = true;
            }           

            if (_controller.isGrounded)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }

            Vector3 targetDirection = Vector3.zero;

            float targetSpeed = speed;

            // a reference to the players current horizontal velocity
            currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _player = animator.GetComponent<Player>();
            _input = _player.GetComponent<PlayerInputsScript>();
            _animator = animator;
            id = PlayerCondition.F_Dodging;
            base.OnStateEnter(animator, stateInfo, layerIndex);

            timer = 0;
            dmgTimer = 0;
            _player.canDmg = false;
            _controller = _player.GetComponent<CharacterController>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");
            hasAnimator = _player.TryGetComponent(out _animator);
            _animIDDodge = Animator.StringToHash("Dodge");
            _animIDForward = Animator.StringToHash("Forward");

            // Testing
            //((TestDataInt)GameObject.Find("TestingController").GetComponent<TestingController>().ReportedData["numDodges"]).Data.CurrentValue++;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            canDmg = true;
            _controller.SimpleMove(Vector3.zero);
            _animator.SetBool(_animIDDodge, false);
        }
    }
}
