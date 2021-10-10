using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Bladesmiths.Capstone.Enums;

using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The main class for the Player character
    /// This is where all of the transitions between states 
    /// are defined and how they are transitioned between
    /// </summary>
    public class Player : Character, IDamaging
    {
        // Reference to the Finite State Machine
        private FiniteStateMachine FSM;

        //[SerializeField] private TransitionManager playerTransitionManager;
        [Header("Player Fields")]

        [SerializeField]
        private PlayerInputsScript inputs;

        // Gets a reference to the player
        [SerializeField]
        private GameObject player;
        [SerializeField]
        private GameObject sword;

        [SerializeField]
        private GameObject parryDetector;
        [SerializeField]
        private GameObject blockDetector;

        [OdinSerialize]
        private Dictionary<PlayerCondition, float> speedValues = new Dictionary<PlayerCondition, float>();

        //private PlayerFSMState_MOVING move;
        private PlayerFSMState_PARRYATTEMPT parryAttempt;
        private PlayerFSMState_PARRYSUCCESS parrySuccess;
        private PlayerFSMState_IDLE idleMovement;
        private PlayerFSMState_IDLE idleCombat;


        private PlayerFSMState_ATTACK attack;
        private PlayerFSMState_DEATH death;
        private PlayerFSMState_TAKEDAMAGE takeDamage;
        private PlayerFSMState_DODGE dodge;
        private PlayerFSMState_JUMP jump;
        private PlayerFSMState_BLOCK block;
        private PlayerFSMState_NULL nullState;

        private TargetLock targetLock; 

        [Header("State Fields")]
        public bool inState;
        public bool damaged;
        public bool parryEnd;
        private float dodgeTimer;
        [SerializeField] [Range(0.0f, 1.0f)]
        private float chipDamagePercent; 

        [Header("Cinemachine Target Fields")]
        public float cinemachineTargetYaw;
        public float cinemachineTargetPitch;
        private const float threshold = 0.01f;
        [SerializeField] 
        public GameObject CinemachineCameraTarget;
        private float TopClamp = 70.0f;
        private float BottomClamp = -30.0f;
        private float CameraAngleOverride = 0.0f;
        private bool LockCameraPosition = false;

        [Header("Grounded Fields")]
        public LayerMask GroundLayers;
        public bool Grounded = true;
        public bool isGrounded = true;
        public float GroundedOffset = -0.10f;
        public float GroundedRadius = 0.20f;
        [SerializeField] private float landTimeout;

        [Header("Sword Fields")]
        [SerializeField]
        private float currentSwordDamage;
        private Sword currentSword; 
        private List<Sword> swords = new List<Sword>(); 

        // Testing for damaging system
        [Header("Damaging Timer Fields (Testing)")]
        [SerializeField]
        private float damagingTimerLimit;
        private float damagingTimer;
        private bool damaging;

        #region Fields from the Move State and Jump State

        [Header("Move/Jump Fields")]
        public float timer;

        private Animator animator;

        private int animIDForward;
        private int animIDGrounded;
        private int animIDJump;
        private int animIDFreeFall;
        private float animBlend;
        private bool hasAnimator;
        private CharacterController controller;

        public float speed;
        float targetSpeed = 0;

        private float targetRotation = 0.0f;
        private float rotationVelocity;
        private float verticalVelocity = 0.0f;
        private float terminalVelocity = 53.0f;
        public float SpeedChangeRate = 10.0f;
        public float RotationSmoothTime = 0.12f;

        public float FallTimeout = 0.15f;
        //private float landTimeout;
        public float LandTimeoutDelta = 0;

        private Vector3 controllerVelocity;

        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        private GameObject playerCamera;


        public float RunSpeed = 10.0f;
        public float WalkSpeed = 4.0f;
        public float MoveSpeedCurrentMax = 10.0f;

        public float JumpTimeout = 0.50f;

        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;
        #endregion

        // The event to call when damaging is finished
        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;

        private float idRemovalTimer;


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

        public float Damage { get => currentSwordDamage; }

        private void Awake()
        {
            Health = 1000;
            animator = GetComponent<Animator>();
            animIDForward = Animator.StringToHash("Forward");
            animBlend = 0;
            dodgeTimer = 0;

            jumpTimeoutDelta = JumpTimeout;
            fallTimeoutDelta = FallTimeout;

            controller = GetComponent<CharacterController>();
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera");

            hasAnimator = TryGetComponent(out animator);
            animIDGrounded = Animator.StringToHash("Grounded");
            animIDJump = Animator.StringToHash("Jump");
            animIDFreeFall = Animator.StringToHash("FreeFall");

            LandTimeoutDelta = -1.0f;

            isGrounded = false;
            controllerVelocity = Vector2.zero;

            Health = MaxHealth;
            damaged = false;
            inState = false;

            parryEnd = false;

            sword.GetComponent<Sword>().Player = this;

            // Creates the FSM
            FSM = new FiniteStateMachine();

            // Subscribe to the FSM's OnStateChange event
            FSM.OnStateChange += SpeedUpdate;

            // Creates all of the states
            parryAttempt = new PlayerFSMState_PARRYATTEMPT(parryDetector, inputs, this);
            parrySuccess = new PlayerFSMState_PARRYSUCCESS(parryDetector, inputs, this);
            block = new PlayerFSMState_BLOCK(this, inputs, animator, sword, blockDetector);
            //move = new PlayerFSMState_MOVING(this, inputs, animator, GroundLayers);
            idleMovement = new PlayerFSMState_IDLE(animator);
            idleCombat = new PlayerFSMState_IDLE(animator);
            attack = new PlayerFSMState_ATTACK(this, inputs, animator, sword);
            death = new PlayerFSMState_DEATH(this);
            takeDamage = new PlayerFSMState_TAKEDAMAGE(this);
            dodge = new PlayerFSMState_DODGE(this, inputs, animator, GroundLayers);
            jump = new PlayerFSMState_JUMP(this, inputs, GroundLayers, landTimeout);
            nullState = new PlayerFSMState_NULL();

            // Adds all of the possible transitions
            FSM.AddTransition(idleCombat, attack, IsAttacking());
            FSM.AddTransition(attack, idleCombat, IsCombatIdle());
            FSM.AddTransition(idleCombat, death, Alive());
            FSM.AddTransition(attack, death, Alive());
            FSM.AddTransition(idleCombat, dodge, IsDodging());
            FSM.AddTransition(dodge, idleCombat, IsDodgingStopped());

            FSM.AddTransition(idleCombat, block, IsBlockPressed());
            FSM.AddTransition(block, parryAttempt, IsBlockReleased());
            FSM.AddTransition(parryAttempt, idleCombat, IsParryReleased());


            cinemachineTargetYaw = player.transform.rotation.eulerAngles.y;

            FSM.AddAnyTransition(takeDamage, IsDamaged());
            FSM.AddTransition(takeDamage, idleCombat, IsAbleToDamage());

            // Sets the current state
            FSM.SetCurrentState(idleCombat);

            targetLock = GetComponent<TargetLock>();
            blockDetector.GetComponent<BlockCollision>().ChipDamagePercentage = chipDamagePercent;

            // Temporary probably
            currentSword = sword.GetComponent<Sword>();
            currentSwordDamage = currentSword.Damage;
            damagingIds = new List<int>();

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
        public Func<bool> IsCombatIdle() => () => (attack.Timer >= 2.0f) && !inputs.parry; // Attack Timer conditional should be compared to length of animation

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
        public Func<bool> IsDamaged() => () => damaged;

        /// <summary>
        /// The condition for having been attacked
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsAbleToDamage() => () => takeDamage.timer >= 0.5f;

        /// <summary>
        /// The condition for going from MOVE to DODGE state
        /// </summary>
        public Func<bool> IsDodging() => () => inputs.dodge && controller.isGrounded;

        /// <summary>
        /// The condition for going from DODGE to MOVE state
        /// </summary>
        /// <returns></returns>
        // TODO: Should implement something like when dodging animation stops
        public Func<bool> IsDodgingStopped() => () => dodge.timer >= 1.1f;

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
        };

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
            FSM.Tick();

            Jump();
            Move();

            // Testing
            // If the enemy is currently damaging an object
            if (damaging)
            {
                // Update the timer
                damagingTimer += Time.deltaTime;

                // If the timer is equal to or exceeds the limit
                if (damagingTimer >= damagingTimerLimit)
                {
                    // If the damaging finished event has subcribing delegates
                    // Call it, running all subscribing delegates
                    if (DamagingFinished != null)
                    {
                        DamagingFinished(ID);
                    }
                    // If the damaging finished event doesn't have any subscribing events
                    // Something has gone wrong because damaging shouldn't be true otherwise
                    else
                    {
                        Debug.Log("Damaging Finished Event was not subscribed to correctly");
                    }

                    // Reset fields
                    damagingTimer = 0.0f;
                    damaging = false;


            // Temp setup for clearing id's from the list
            // if(idRemovalTimer >= 0.5f)
            // {
            //     damagingIds.Clear();
            //     idRemovalTimer = 0.0f;
            // }
            // if(damagingIds.Count > 0)
            //     idRemovalTimer += Time.deltaTime;
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
        /// Allows for other classes to get a reference to the player's state
        /// </summary>
        /// <returns></returns>
        public PlayerFSMState GetPlayerFSMState()
        {
            return (PlayerFSMState)FSM.GetCurrentState();
        }

        /// <summary>
        /// Allows for the camera to rotate with the player
        /// </summary>
        public void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (inputs.look.sqrMagnitude >= threshold && !LockCameraPosition)
            {
                cinemachineTargetYaw += inputs.look.x * Time.deltaTime;
                cinemachineTargetPitch += inputs.look.y * Time.deltaTime;
            }

            // clamp our rotations so our values are limited 360 degrees
            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

            // Don't update the rotation of the camera's target if target lock is active
            if (!targetLock.Active)
            {
                // Cinemachine will follow this target
                CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
            }
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
        /// Runs whenever the state changes in the FSM
        /// Attaches to the Delegate in the class
        /// </summary>
        private void SpeedUpdate()
        {
            // Gets each speed value based off of what state the player is in
            if (!speedValues.TryGetValue(((PlayerFSMState)FSM.GetCurrentState()).ID, out targetSpeed))
            {
                Debug.Log($"Speed Update Failed. Current State is {((PlayerFSMState)FSM.GetCurrentState()).ID}");
            }
        }

        /// <summary>
        /// Is ran in the Update and allows for the player to move
        /// </summary>
        private void Move()
        {
            hasAnimator = this.TryGetComponent(out animator);

            // Checks to see if the player is grounded
            if (controller.isGrounded)
            {
                if (verticalVelocity < 0.0f)
                {
                    verticalVelocity = -2f;
                }
            }

            Vector3 inputDirection = Vector3.zero;
            Vector3 targetDirection = Vector3.zero;

            speed = targetSpeed;

            // if the input is greater than 1 then set the speed to the max
            if (inputs.move.magnitude <= 1)
            {
                speed *= inputs.move.magnitude;
            }

            // if the speed is less than the walkspeed and greater than 0 then set it to the walk speed

            speed = Mathf.Clamp(speed, speed > 0 ? WalkSpeed : 0, targetSpeed);

            // If the player isn't moving set their speed to 0
            if (inputs.move == Vector2.zero) speed = 0.0f;

            // Animator input
            //animator.SetFloat(animIDForward, speed / targetSpeed);
            animBlend = Mathf.Lerp(animBlend, speed, Time.deltaTime * SpeedChangeRate);

            // normalise input direction
            inputDirection = new Vector3(inputs.move.x, 0.0f, inputs.move.y).normalized;

            // Runs if the player is inputting a movement key and whenever the targetspeed is not 0
            // This allows for the player to not rotate a different direction based off of what they
            // are inputting when dodging or in another state
            if (inputs.move != Vector2.zero && speed != 0)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

                // rotate to face input direction relative to camera position
                this.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
            }

            if (verticalVelocity < terminalVelocity)
            {
                verticalVelocity += Gravity * Time.deltaTime;
            }

            // move the player
            controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

            if (hasAnimator)
            {
                animator.SetFloat(animIDForward, animBlend);
                //animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        /// <summary>
        /// Allows for the player to jump
        /// Mainly just sets the vertical velocity
        /// </summary>
        private void Jump()
        {
            if (controller.isGrounded)
            {
                //Debug.Log("<color=brown>Grounded</color>");
                fallTimeoutDelta = FallTimeout;

                isGrounded = true;

                // update animator if using character
                if (hasAnimator)
                {
                    animator.SetBool(animIDJump, false);
                    animator.SetBool(animIDFreeFall, false);
                    verticalVelocity = 0f;
                }

                // stop our velocity dropping infinitely when grounded
                if (verticalVelocity < 0.0f)
                {
                    verticalVelocity = -2f;
                }

                // Jump
                // Checks to make sure that the player should be able to move
                if (inputs.jump && targetSpeed != 0)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    controllerVelocity = controller.velocity.normalized * inputs.move.magnitude;

                    // update animator if using character
                    if (hasAnimator)
                    {
                        animator.SetBool(animIDJump, true);
                    }
                }

                // jump timeout
                if (jumpTimeoutDelta >= 0.0f)
                {
                    jumpTimeoutDelta -= Time.deltaTime;
                }

                // Land Timer
                if (LandTimeoutDelta >= 0.0f)
                {
                    animator.SetBool(animIDJump, false);
                    animator.SetBool(animIDFreeFall, false);
                    animator.SetBool(animIDGrounded, true);

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

                LandTimeoutDelta = landTimeout;

                // reset the jump timeout timer
                jumpTimeoutDelta = JumpTimeout;

                // update animator if using character
                if (hasAnimator)
                {
                    animator.SetBool(animIDFreeFall, true);
                    animator.SetBool(animIDGrounded, false);

                }

                inputs.jump = false;
            }


            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (verticalVelocity < terminalVelocity)
            {
                verticalVelocity += Gravity * Time.deltaTime;
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
        /// Attack with the player's sword
        /// </summary>
        /// <param name="targetID">The id of the object to attack</param>
        /// <param name="damage">The amount of damage to give to the target</param>
        public void SwordAttack(int targetID, float damage)
        {
            ObjectController.DamageableObjects[targetID].DamageableObject.TakeDamage(ID, damage);

            // Testing
            damaging = true;
        }

        /// <summary>
        /// Subtract an amount of damage from the character's health
        /// </summary>
        /// <param name="damagingID">The id of the damaging object that is damaging this character</param>
        /// <param name="damage">The amount of damage to be subtracted</param>
        /// <returns>Returns a boolean indicating whether damage was taken or not</returns>
        public override bool TakeDamage(int damagingID, float damage)
        {
            // If the player is not in invincibility frames
            // They can take damage
            if (dodge.canDmg)
            {
                // The resullt of Character's Take Damage
                // Was damage taken or not
                bool damageResult = base.TakeDamage(damagingID, damage);

                // If damage was taken
                // Update the playerHealth field for analytics
                if (damageResult)
                {
                    // Playtest 1
                    playerHealth.CurrentValue -= damage;

                    // Does not set damaged to true if block has been triggered
                    // Might need to be changed slightly eventually to better
                    // account for player taking damage from behind them
                    damaged = (blockDetector.GetComponent<BlockCollision>().BlockTriggered) ? false : true;
                }

                // Return whether damage was taken or not
                return damageResult; 
            }

            // Return false if the player cannot currently be damaged
            return false; 
        }
    }
}