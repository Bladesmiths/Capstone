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
        private bool hasAnimator;

        private int _animIDSpeed;
        private int _animIDDodge;
        private int _animIDMotionSpeed;
        private int _animIDForward;
        private int _animIDDodgeMove;
        private float animBlend;
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

        private float currentHorizontalSpeed;
        private Vector3 inputDirection;


        public PlayerFSMState_DODGE(Player player, PlayerInputsScript input, Animator animator, LayerMask layers)
        {
            _player = player;
            _input = input;
            _animator = animator;
            GroundLayers = layers;
            id = PlayerCondition.F_Dodging;
        }

        public override void Tick()
        {
            timer += Time.deltaTime;
            _input.dodge = false;

            dmgTimer += Time.deltaTime;
            if (dmgTimer >= 0.3f)
            {
                canDmg = true;
                _player.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;

            }
            else
            {
                _player.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.blue;

            }
            
            if (_controller.isGrounded)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }

            Vector3 targetDirection = Vector3.zero;

            float targetSpeed = 5;

            // a reference to the players current horizontal velocity
            currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;



            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

           
            if (hasAnimator)
            {
                _animator.SetBool(_animIDDodge, true);
                
            }

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
            dmgTimer = 0;
            canDmg = false;
            _controller = _player.GetComponent<CharacterController>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");
            hasAnimator = _player.TryGetComponent(out _animator);
            _animIDDodge = Animator.StringToHash("Dodge");
            _animIDForward = Animator.StringToHash("Forward");

            //inputDirection = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).normalized;

            if (_input.move == Vector2.zero)
            {
                _targetRotation = _player.transform.eulerAngles.y;

                inputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.back;
                

            }
            else
            {
                Vector3 inputMove = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

                // rotate to face input direction relative to camera position
                _targetRotation = Mathf.Atan2(inputMove.x, inputMove.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;

                _player.transform.rotation = Quaternion.Euler(0.0f, _targetRotation, 0.0f);

                inputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            
            }

            //_player.transform.rotation = Quaternion.Euler(0.0f, _targetRotation, 0.0f);

            // Testing
            //((TestDataInt)GameObject.Find("TestingController").GetComponent<TestingController>().ReportedData["numDodges"]).Data.CurrentValue++;
        }

        public override void OnExit()
        {
            canDmg = true;
            _controller.SimpleMove(Vector3.zero);
            _animator.SetBool(_animIDDodge, false);

        }


    }
}
