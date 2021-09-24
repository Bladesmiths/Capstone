using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;

using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The main class for the Player character
    /// This is where all of the transitions between states 
    /// are defined and how they are transitioned between
    /// </summary>
    public class Player : Character
    {        
        // Reference to the Finite State Machine
        private FiniteStateMachine Movement_FSM;
        private FiniteStateMachine Combat_FSM;

        //[SerializeField] private TransitionManager playerTransitionManager;

        [SerializeField] private PlayerInputsScript inputs;

        // Gets a reference to the player
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject sword;

        [SerializeField] private GameObject parryDetector;
        [SerializeField] private GameObject blockDetector;

        //private PlayerFSMState_MOVING move;
        private PlayerFSMState_PARRYATTEMPT parryAttempt;
        private PlayerFSMState_PARRYSUCCESS parrySuccess;
        private PlayerFSMState_IDLE idleMovement;
        private PlayerFSMState_IDLE idleCombat;


        PlayerFSMState_ATTACK attack;
        PlayerFSMState_DEATH death;
        PlayerFSMState_TAKEDAMAGE takeDamage;
        PlayerFSMState_DODGE dodge;
        PlayerFSMState_JUMP jump;
        PlayerFSMState_BLOCK block;
        PlayerFSMState_NULL nullState;

        public bool isDamaged;
        public bool inState;

        public float _cinemachineTargetYaw;
        public float _cinemachineTargetPitch;
        private const float _threshold = 0.01f;
        [SerializeField] public GameObject CinemachineCameraTarget;
        private float TopClamp = 70.0f;
        private float BottomClamp = -30.0f;
        private float CameraAngleOverride = 0.0f;
        private bool LockCameraPosition = false;

        public LayerMask GroundLayers;
        public bool Grounded = true;
        public bool isGrounded = true;
        public float GroundedOffset = -0.10f;
        public float GroundedRadius = 0.20f;
        [SerializeField] private float landTimeout;

        #region Fields from the Move State and Jump State

        public float timer;

        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;

        private int _animIDForward;
        private int _animIDDodge;
        private int _animIDMotionSpeed;
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
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

        public float FallTimeout = 0.15f;
        private float _landTimeout;
        public float LandTimeoutDelta = 0;

        private Vector3 controllerVelocity;

        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        private GameObject camera;


        public float RunSpeed = 10.0f;
        public float WalkSpeed = 4.0f;
        public float MoveSpeedCurrentMax = 10.0f;

        public float JumpTimeout = 0.50f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        #endregion

        public bool parryEnd;

        #region Testing Fields 
        [Header("Testing Fields")]
        [SerializeField]
        private ReactiveInt numAttacks;
        [SerializeField]
        private ReactiveInt numBlocks;
        [SerializeField]
        private ReactiveInt numDodges;
        [SerializeField]
        private ReactiveInt numJumps;
        [SerializeField]
        private ReactiveFloat playerHealth;
        #endregion

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _animIDForward = Animator.StringToHash("Forward");
            _animBlend = 0;

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            //timer = 0;
            _controller = GetComponent<CharacterController>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");

            _hasAnimator = TryGetComponent(out _animator);
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");

            LandTimeoutDelta = -1.0f;

            isGrounded = false;
            controllerVelocity = Vector2.zero;

            Health = MaxHealth;
            isDamaged = false;
            inState = false;

            parryEnd = false;

            // Creates the FSM
            Movement_FSM = new FiniteStateMachine();
            Combat_FSM = new FiniteStateMachine();

            // Creates all of the states
            parryAttempt = new PlayerFSMState_PARRYATTEMPT(parryDetector, inputs, this);
            parrySuccess = new PlayerFSMState_PARRYSUCCESS(parryDetector, inputs, this);
            block = new PlayerFSMState_BLOCK(blockDetector);
            //move = new PlayerFSMState_MOVING(this, inputs, _animator, GroundLayers);
            idleMovement = new PlayerFSMState_IDLE(_animator);
            idleCombat = new PlayerFSMState_IDLE(_animator);
            attack = new PlayerFSMState_ATTACK(this, inputs, _animator, sword);
            death = new PlayerFSMState_DEATH(this);
            takeDamage = new PlayerFSMState_TAKEDAMAGE(this);
            dodge = new PlayerFSMState_DODGE(this, inputs, _animator, GroundLayers);
            jump = new PlayerFSMState_JUMP(this, inputs, GroundLayers, landTimeout);
            nullState = new PlayerFSMState_NULL();

            // Adds all of the possible transitions
            // These are the possible transitions for the Player's Movement
            //Movement_FSM.AddTransition(move, idleMovement, IsIdle());
            //Movement_FSM.AddTransition(idleMovement, move, IsMoving());
            //Movement_FSM.AddTransition(move, dodge, IsDodging());
            Movement_FSM.AddTransition(dodge, idleMovement, IsDodgingStopped());
            //Movement_FSM.AddTransition(jump, idleMovement, IsGrounded());
            //Movement_FSM.AddTransition(move, jump, IsJumping());
            //Movement_FSM.AddTransition(idleMovement, jump, IsJumping());
            Movement_FSM.AddTransition(idleMovement, dodge, IsDodging());

            // NULL state for when player is in either TAKEDAMAGE or DEAD
            //Movement_FSM.AddAnyTransition(nullState, IsNull());
            Movement_FSM.AddTransition(nullState, idleMovement, NotNull());

            // These are the possible transitions for the Player's Combat
            Combat_FSM.AddTransition(idleCombat, attack, IsAttacking());
            Combat_FSM.AddTransition(attack, idleCombat, IsCombatIdle());
            Combat_FSM.AddTransition(idleCombat, death, Alive());
            Combat_FSM.AddTransition(attack, death, Alive());
            //Combat_FSM.AddTransition(idleCombat, parry, IsBlockReleased());
            //Combat_FSM.AddTransition(parry, idleCombat, IsReleased());
            Combat_FSM.AddTransition(idleCombat, block, IsBlockPressed());
            Combat_FSM.AddTransition(block, parryAttempt, IsBlockReleased());
            Combat_FSM.AddTransition(parryAttempt, idleCombat, IsParryReleased());

            Combat_FSM.AddAnyTransition(takeDamage, IsDamaged());
            Combat_FSM.AddTransition(takeDamage, idleCombat, IsAbleToDamage());

            // Sets the current state
            Combat_FSM.SetCurrentState(idleCombat);
            Movement_FSM.SetCurrentState(idleMovement);


        }

        /// <summary>
        /// The condition for going between the IDLE and MOVE states
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsMoving() => () => inputs.move != Vector2.zero;

        /// <summary>
        /// The condition for going between the MOVE and IDLE states
        /// </summary>
        /// <returns></returns>
        //public Func<bool> IsIdle() => () => move.timer >= 0.5f;
        public Func<bool> IsIdle() => () => this.gameObject.GetComponent<CharacterController>().velocity.magnitude <= 0;

        /// <summary>
        /// The condition for going between the MOVE and IDLE states
        /// </summary>
        /// <returns></returns>
        //public Func<bool> IsIdle() => () => move.timer >= 0.5f;
        public Func<bool> IsCombatIdle() => () => (attack.Timer >= 0.1f) && !inputs.parry; // Attack Timer conditional should be compared to length of animation

        /// <summary>
        /// The condition for going between the IDLE and BLOCK state
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsBlockPressed() => () => inputs.block == true;

        /// <summary>
        /// The condition for going between the BLOCK and PARRY state
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsBlockReleased() => () => inputs.block == false;

        /// <summary>
        /// The condition for going between the PARRY and IDLE state
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsParryReleased() => () => parryEnd == true;

        /// <summary>
        /// The condition for going between MOVE/IDLE and the ATTACK states
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsAttacking() => () => inputs.attack;

        /// <summary>
        /// The condition for having been attacked
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsDamaged() => () => isDamaged;

        /// <summary>
        /// The condition for having been attacked
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsAbleToDamage() => () => takeDamage.timer >= 0.5f;

        /// <summary>
        /// The condition for going from MOVE to DODGE state
        /// </summary>
        public Func<bool> IsDodging() => () => inputs.dodge;

        /// <summary>
        /// The condition for going from DODGE to MOVE state
        /// </summary>
        /// <returns></returns>
        // TODO: Should implement something like when dodging animation stops
        public Func<bool> IsDodgingStopped() => () => dodge.timer >= 0.3;

        /// <summary>
        /// The condition for having been attacked
        /// </summary>
        /// <returns></returns>
        public Func<bool> Alive() => () => Health <= 0;

        /// <summary>
        /// Waits .5 seconds until the parry switches back to the default state
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsReleased() => () => parryAttempt.timer >= 0.5;

        /// <summary>
        /// Checks if the player is grounded
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsGrounded() => () =>
        { 
            return gameObject.GetComponent<CharacterController>().isGrounded && jump.LandTimeoutDelta <= 0.0f;
        };//GroundedCheck();

        /// <summary>
        /// Checks to see if the jump button has been pressed
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsJumping() => () => inputs.jump;
        
        /// <summary>
        /// The condition for going to the NULL state
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsNull() => () => inState == true;

        /// <summary>
        /// The condition for going to the NULL state
        /// </summary>
        /// <returns></returns>
        public Func<bool> NotNull() => () => inState == false;

        private void Update()
        {
            Movement_FSM.Tick();
            Combat_FSM.Tick();

            Jump();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        /// <summary>
        /// Checks to see if the Player is on the ground
        /// </summary>
        /// <returns></returns>
        private bool GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(this.transform.position.x, this.transform.position.y - GroundedOffset, this.transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            return Grounded;

        }

        /// <summary>
        /// Allows for the camera to rotate with the player
        /// </summary>
        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (inputs.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                _cinemachineTargetYaw += inputs.look.x * Time.deltaTime;
                _cinemachineTargetPitch += inputs.look.y * Time.deltaTime;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        /// <summary>
        /// Sets the angle for the camera going around the player
        /// </summary>
        /// <param name="lfAngle"></param>
        /// <param name="lfMin"></param>
        /// <param name="lfMax"></param>
        /// <returns></returns>
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        /// <summary>
        /// Is ran in the Update and allows for the player to move
        /// </summary>
        private void Move()
        {
            _hasAnimator = this.TryGetComponent(out _animator);


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
            float targetSpeed = inputs.move.magnitude * MoveSpeedCurrentMax;

            if (inputs.move.magnitude > 1)
            {
                targetSpeed = MoveSpeedCurrentMax;
            }

            if (targetSpeed > 0 && targetSpeed < WalkSpeed)
            {
                targetSpeed = WalkSpeed;
            }

            if (inputs.move == Vector2.zero) targetSpeed = 0.0f;


            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude; //maybe normalize this instead?

            float speedOffset = 0.1f;
            float inputMagnitude = inputs.analogMovement ? inputs.move.magnitude : 1f;

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
            inputDirection = new Vector3(inputs.move.x, 0.0f, inputs.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (inputs.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                // rotate to face input direction relative to camera position
                this.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

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

        /// <summary>
        /// Allows for the player to jump
        /// Mainly just sets the vertical velocity
        /// </summary>
        private void Jump()
        {
            if (_controller.isGrounded)
            {
                //Debug.Log("<color=brown>Grounded</color>");
                _fallTimeoutDelta = FallTimeout;

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
                if (inputs.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    controllerVelocity = _controller.velocity.normalized * inputs.move.magnitude;

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
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
                    LandTimeoutDelta = -1f;
                }
                
            }

            else
            {
                //Debug.Log("<color=blue>In Air</color>");

                LandTimeoutDelta = _landTimeout;

                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                        _animator.SetBool(_animIDGrounded, false);

                    }
                }
               
                inputs.jump = false;

                
            }





            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

        }

        protected override void Attack()
        {
            // Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.TransformDirection(Vector3.forward), 2) && 
        }
        protected override void ActivateAbility()
        {

        }
        protected override void Block()
        {

        }
        protected override void Parry()
        {

        }
        protected override void Dodge()
        {

        }
        protected override void SwitchWeapon(int weaponSelect)
        {

        }
        protected override void Die()
        {

        }

        /// <summary>
        /// Allows for the player to take damage
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamage(float damage)
        {
            if (dodge.canDmg)
            {
                base.TakeDamage(damage);
                isDamaged = true;

                // Playtest 1
                playerHealth.CurrentValue -= damage;

                // Shouldn't this be going to the Player's TakeDamage State?
            }
        }
    }
}