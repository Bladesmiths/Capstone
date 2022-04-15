using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;
using System.Linq;
using UnityEditor;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the player is dashing
    /// TODO: will need to re-write as this is only a temp interaction for what it feels like
    /// </summary>
    public class PlayerState_DASH : PlayerState_Base
    {
        public float timer;
        private float maxTimer;

        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;
        private float speed;

        private int _animIDDodge;
        private int animIDMoving;
        private CharacterController _controller;

        private float _targetRotation = 0.0f;
        private float _verticalVelocity = 0.0f;
        private float _terminalVelocity = 53.0f;

        public float GroundedOffset = -0.10f;
        public float GroundedRadius = 0.20f;
        public LayerMask GroundLayers;

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

            speed = 25f;

            if (_input.dodge)
            {
                if (_input.move == Vector2.zero)
                {
                    _targetRotation = _player.transform.eulerAngles.y;

                    inputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                    animator.SetBool(animIDMoving, false);
                }
                else if (_input.move != Vector2.zero)
                {
                    Vector3 inputMove = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
                    // rotate to face input direction relative to camera position
                    _targetRotation = Mathf.Atan2(inputMove.x, inputMove.z) * Mathf.Rad2Deg + camera.transform.localEulerAngles.y;
                    _player.transform.rotation = Quaternion.Euler(0.0f, _targetRotation, 0.0f);

                    inputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                    animator.SetBool(animIDMoving, true);
                }
            }
        }

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timer += Time.deltaTime;
            _input.dodge = false;

            if (_controller.isGrounded)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }

            float targetSpeed = speed;

            // a reference to the players current horizontal velocity
            currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            //Debug.Log(inputDirection.normalized);

            // move the player
            _controller.Move(inputDirection.normalized * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _player = animator.GetComponent<Player>();
            _input = _player.GetComponent<PlayerInputsScript>();
            _animator = animator;
            id = PlayerCondition.F_Dashing;
            base.OnStateEnter(animator, stateInfo, layerIndex);

            animIDMoving = Animator.StringToHash("Moving");
            _animIDDodge = Animator.StringToHash("Dodging");

            timer = 0;
            maxTimer = stateInfo.length;
            _controller = _player.GetComponent<CharacterController>();
            camera = Camera.main.gameObject;

            _player.gameObject.layer = LayerMask.NameToLayer("IgnoreEnemies");
            _player.shouldLookAt = false;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _controller.SimpleMove(Vector3.zero);
            _player.gameObject.layer = LayerMask.NameToLayer("Player");
            _player.shouldLookAt = true;
        }
    }
}
