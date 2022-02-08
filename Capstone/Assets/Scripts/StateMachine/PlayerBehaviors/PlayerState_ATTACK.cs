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
    /// The state for when the Player attacks
    /// </summary>
    public class PlayerState_ATTACK : PlayerState_Base
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
        private bool playSound;

        [SerializeField]
        private FMODUnity.EventReference SwordMissEvent;

        private Dictionary<SwordType, float> _animDurations = new Dictionary<SwordType, float>(); 

        public float Timer { get { return timer; } }
        public Dictionary<SwordType, float> AnimDurations { get { return _animDurations; } }

        //public PlayerState_ATTACK(Player player, PlayerInputsScript input, Animator animator, GameObject sword)
        //{
        //    _player = player;
        //    _input = input;
        //    _animator = animator;
        //    _sword = sword;
        //    id = PlayerCondition.F_Attacking;

        //    _animDurations.Add(SwordType.Quartz, animator.runtimeAnimatorController.animationClips.
        //        Where(clip => clip.name == "Sword And Shield Slash 2").ToArray()[0].
        //        length / player.CurrentBalancingData.AttackAnimSpeeds[SwordType.Quartz]);
        //    _animDurations.Add(SwordType.Ruby, animator.runtimeAnimatorController.animationClips.
        //        Where(clip => clip.name == "Ruby Slash_Colliders").ToArray()[0].
        //        length / player.CurrentBalancingData.AttackAnimSpeeds[SwordType.Ruby]);
        //    _animDurations.Add(SwordType.Sapphire, animator.runtimeAnimatorController.animationClips.
        //        Where(clip => clip.name == "Sapphire Inward Slash_Colliders").ToArray()[0].
        //        length / player.CurrentBalancingData.AttackAnimSpeeds[SwordType.Sapphire]);
        //}

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateMove(animator, stateInfo, layerIndex);

            if (_input.attack)
            {
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


        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            _animDurations.Add(SwordType.Topaz, animator.runtimeAnimatorController.animationClips.
                Where(clip => clip.name == "Sword And Shield Slash 2").ToArray()[0].
                length / _player.CurrentBalancingData.AttackAnimSpeeds[SwordType.Topaz]);
            _animDurations.Add(SwordType.Ruby, animator.runtimeAnimatorController.animationClips.
                Where(clip => clip.name == "Ruby Slash_Colliders").ToArray()[0].
                length / _player.CurrentBalancingData.AttackAnimSpeeds[SwordType.Ruby]);
            _animDurations.Add(SwordType.Sapphire, animator.runtimeAnimatorController.animationClips.
                Where(clip => clip.name == "Sapphire Inward Slash_Colliders").ToArray()[0].
                length / _player.CurrentBalancingData.AttackAnimSpeeds[SwordType.Sapphire]);
        }


        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timer += Time.deltaTime;

            _input.attack = false;

            if (_controller.isGrounded)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }

            Vector3 targetDirection = Vector3.zero;


            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //base.OnStateEnter(animator, stateInfo, layerIndex);

            _player = animator.GetComponent<Player>();
            _input = _player.GetComponent<PlayerInputsScript>();
            _animator = animator;
            //_sword = sword;
            id = PlayerCondition.F_Attacking;
            base.OnStateEnter(animator, stateInfo, layerIndex);

            playSound = true;



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

            // Allows for the player to snap to the direction they are inputting
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
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _animator.SetBool(_animIDAttack, false);
            _player.ClearDamaging();
            AIDirector.Instance.ResetBlocks();
        }
    }
}
