using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Bladesmiths.Capstone.Enums;
using UnityEngine.SceneManagement;
using Cinemachine;

using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The main class for the Player character
    /// This is where all of the transitions between states 
    /// are defined and how they are transitioned between
    /// </summary>
    public partial class Player : Character, IDamaging
    {
        #region Fields
        // Reference to the Finite State Machine
        private FiniteStateMachine FSM;
        [SerializeField]
        private BalancingData currentBalancingData; 

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

        private bool isRecentering;
        private float camRotation;

        [SerializeField]
        private Vector3 respawnPoint;
        [SerializeField]
        private Vector3 respawnRotation;


        [OdinSerialize]
        private Dictionary<PlayerCondition, float> speedValues = new Dictionary<PlayerCondition, float>();

        #region Player FSM States
        private PlayerFSMState_PARRYATTEMPT parryAttempt;
        private PlayerFSMState_PARRYSUCCESS parrySuccess;
        private PlayerFSMState_IDLE idleCombat;


        private PlayerFSMState_ATTACK attack;
        private PlayerFSMState_DEATH death;
        private PlayerFSMState_TAKEDAMAGE takeDamage;
        private PlayerFSMState_DODGE dodge;
        private PlayerFSMState_JUMP jump;
        private PlayerFSMState_BLOCK block;
        private PlayerFSMState_NULL nullState;
        #endregion

        private TargetLock targetLock;

        #region State Fields
        [Header("State Fields")]
        public bool inState;
        public bool damaged;
        public bool parryEnd;
        public bool parrySuccessful;
        private float dodgeTimer;
        #endregion

        #region Cinemachine Target Fields
        [Header("Cinemachine Target Fields")]
        public float cinemachineTargetYaw;
        public float cinemachineTargetPitch;
        private const float threshold = 0.01f;
        private float TopClamp = 70.0f;
        private float BottomClamp = -30.0f;
        private float CameraAngleOverride = 0.0f;
        private bool LockCameraPosition = false;

        [SerializeField]
        private CinemachineFreeLook freeLookCam;
        private bool recenter;
        private float recenterTimer = 0f;
        private float recenterTimerMax = 2f;


        #region Grounded Fields
        [Header("Grounded Fields")]
        public LayerMask GroundLayers;
        public bool Grounded = true;
        public bool isGrounded = true;
        public float GroundedOffset = -0.10f;
        public float GroundedRadius = 0.20f;
        [SerializeField] private float landTimeout;
        #endregion

        #region Sword Fields
        [Header("Sword Fields")]
        [SerializeField]
        private Sword currentSword;
        [OdinSerialize]
        private Dictionary<SwordType, GameObject> swords = new Dictionary<SwordType, GameObject>();
        #endregion

        // Testing for damaging system
        [Header("Damaging Timer Fields (Testing)")]
        [SerializeField]
        private float damagingTimerLimit;
        private float damagingTimer;
        private bool damaging;
        private float points = 0;
        private float maxPoints = 8;

        [SerializeField] private GameObject fade;
        public bool hasFadedToBlack;
        public bool justDied;

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
        public float RotationSmoothTime = 1000f;

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
        #endregion

        #region Properties
        public PlayerInputsScript Inputs { get => inputs; }
        public BalancingData CurrentBalancingData { get => currentBalancingData; }

        public ParryCollision ParryDetector { get => parryDetector.GetComponent<ParryCollision>(); }
        
        public float CurrentChipDamage { get => parryDetector.GetComponent<ParryCollision>().ChipDamageTotal; }

        public int Points { get => (int)points; }

        public int MaxPoints { get => (int)maxPoints; }

        public bool Damaging { get => damaging; set => damaging = value; }

        #region Properties from Swords
        public Sword CurrentSword { get => currentSword; }
        public float Damage { get => currentSword.Damage; }
        public float ChipDamagePercentage { get => currentSword.ChipDamagePercentage; }
        public float ParryDelay { get => currentSword.ParryDelay; }
        public float ParryLength { get => currentSword.ParryLength; }
        public float ParryCooldown { get => currentSword.ParryCooldown; }
        #endregion
        #endregion

        #endregion

        private void Awake()
        {
            Health = 1000;
            animator = GetComponent<Animator>();
            animIDForward = Animator.StringToHash("Forward");
            animBlend = 0;
            dodgeTimer = 0;

            isRecentering = false;

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

            // Creates the FSM
            FSM = new FiniteStateMachine();

            // Subscribe to the FSM's OnStateChange event
            FSM.OnStateChange += SpeedUpdate;

            // Subscribing parry collision to block collision events to keep those fields updated
            blockDetector.GetComponent<BlockCollision>().OnBlock += parryDetector.GetComponent<ParryCollision>().BlockOccured;

            // Creates all of the states
            parryAttempt = new PlayerFSMState_PARRYATTEMPT(this, inputs, animator, parryDetector);
            parrySuccess = new PlayerFSMState_PARRYSUCCESS(this, inputs, animator, parryDetector);
            block = new PlayerFSMState_BLOCK(this, inputs, animator, sword, blockDetector);
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
            FSM.AddTransition(parryAttempt, parrySuccess, IsParrySuccessful());
            FSM.AddTransition(parrySuccess, idleCombat, IsParryFinished()); 
            FSM.AddTransition(parryAttempt, idleCombat, IsParryFinished());


            cinemachineTargetYaw = player.transform.rotation.eulerAngles.y;

            FSM.AddAnyTransition(takeDamage, IsDamaged());
            FSM.AddTransition(takeDamage, idleCombat, IsAbleToDamage());

            // Sets the current state
            FSM.SetCurrentState(idleCombat);

            targetLock = GetComponent<TargetLock>();


            inputs.player = this;

            // Temporary
            // Should eventually be changed so it sets the player sword to quartz on start
            // from their dictionary of sword types to sword prefabs
            currentSword = swords[SwordType.Quartz].GetComponent<Sword>();
        }


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

                }
            }

            if(Health <= 0)
            {
                FSM.SetCurrentState(death);
            }

            // If the player is dead and just died (fadeToBlack is still occuring)
            if((FSM.GetCurrentState() == death && justDied) || points >= maxPoints)
            {
                FadeToBlack();
            }

        }

        private void LateUpdate()
        {
            //CameraRotation();

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
        /// Adds points for the second playtest
        /// </summary>
        public void AddPoints()
        {
            points++;
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

            // Not sure if this is the correct place to add PlayerMovementMultiplier
            // Because it changes the animation if reduced/increased too much
            speed = targetSpeed * currentSword.PlayerMovementMultiplier;

            // if the input is greater than 1 then set the speed to the max
            if (inputs.move.magnitude <= 1)
            {
                speed *= inputs.move.magnitude;
            }

            // if the speed is less than the walkspeed and greater than 0 then set it to the walk speed

            //speed = Mathf.Clamp(speed, speed > 0 ? WalkSpeed : 0, targetSpeed);

            // If the player isn't moving set their speed to 0
            if (inputs.move == Vector2.zero) speed = 0.0f;

            // Animator input
            // animator.SetFloat(animIDForward, speed / targetSpeed);
            animBlend = Mathf.Lerp(animBlend, speed, Time.deltaTime * SpeedChangeRate);

            // normalise input direction
            inputDirection = new Vector3(inputs.move.x, 0.0f, inputs.move.y).normalized;

            if(inputs.move != Vector2.zero)
            {
                freeLookCam.m_RecenterToTargetHeading.m_enabled = false;
                freeLookCam.m_YAxisRecentering.m_enabled = false;
                freeLookCam.m_RecenterToTargetHeading.CancelRecentering();
                freeLookCam.m_YAxisRecentering.CancelRecentering();
            }
            else
            {
                freeLookCam.m_RecenterToTargetHeading.m_enabled = true;
                freeLookCam.m_YAxisRecentering.m_enabled = true;
            }

            #region Possible Recentering Implementation. Currently not working
            // Checks to see if the player is moving
            //if(inputs.move == Vector2.zero)
            //{
            //     If they aren't moving start the countdown to recenter
            //    freeLookCam.m_RecenterToTargetHeading.m_enabled = true;
            //    freeLookCam.m_YAxisRecentering.m_enabled = true;

            //    if(recenter)
            //    {
            //         Force recentering if the input 
            //        recenter = false;
            //        freeLookCam.m_RecenterToTargetHeading.RecenterNow();
            //        freeLookCam.m_YAxisRecentering.RecenterNow();

            //    }

            //}
            //else
            //{
            //     If the player moves stop recentering
            //    if (recenter == true)
            //    {
            //        recenterTimer = 0;
            //        recenter = false;
            //    }
            //    freeLookCam.m_RecenterToTargetHeading.CancelRecentering();
            //    freeLookCam.m_YAxisRecentering.CancelRecentering();
            //    freeLookCam.m_RecenterToTargetHeading.m_enabled = false;
            //    freeLookCam.m_YAxisRecentering.m_enabled = false;

            //}

            // If the player isn't moving the second stick or mouse
            //if (inputs.look == Vector2.zero)
            //{
            //    recenterTimer += Time.deltaTime;
            //    Debug.Log(recenter);
            //    Debug.Log(recenterTimer);
            //    if(recenterTimer >= recenterTimerMax)
            //    {
            //         Start recentering the camera
            //        recenter = true;
            //        recenterTimer = 0f;
            //    }
            //}
            //else
            //{
            //    freeLookCam.m_RecenterToTargetHeading.CancelRecentering();
            //    freeLookCam.m_YAxisRecentering.CancelRecentering();
            //    recenterTimer = 0f;

            //}
            #endregion

            // Runs if the player is inputting a movement key and whenever the targetspeed is not 0
            // This allows for the player to not rotate a different direction based off of what they
            // are inputting when dodging or in another state
            if (inputs.move != Vector2.zero && speed != 0)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
                
                float rotation = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, targetRotation, 0.0f), 0.2f);

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

        /// <summary>
        /// Switches the players sword to a new sword
        /// </summary>
        /// <param name="newSwordType"></param>
        public void SwitchSword(SwordType newSwordType)
        {
            // Check to make sure the new sword is not the old sword
            // because that's a waste of time
            if (newSwordType != currentSword.SwordType)
            {
                // Set the old sword to inactive and the new to active
                swords[CurrentSword.SwordType].SetActive(false);
                swords[newSwordType].SetActive(true);

                // Update the current sword field
                currentSword = swords[newSwordType].GetComponent<Sword>();

                // Update the position according to offset
                sword.transform.localPosition = currentSword.Offset.position;
                sword.transform.localRotation = currentSword.Offset.rotation;
                sword.transform.localScale = CurrentSword.Offset.localScale;

                // Update the box collider dimensions
                sword.GetComponent<BoxCollider>().center = swords[newSwordType].GetComponent<BoxCollider>().center;
                sword.GetComponent<BoxCollider>().size = swords[newSwordType].GetComponent<BoxCollider>().size;

                // TODO: Player sword switching animation
            }
        }

        protected override void Die()
        {

        }

        /// <summary>
        /// Attack with the player's sword
        /// </summary>
        /// <param name="targetID">The id of the object to attack</param>
        public void SwordAttack(int targetID)
        {
            float damageDealt = ((IDamageable)ObjectController[targetID].IdentifiedObject).TakeDamage(ID, Damage);
            Health += damageDealt * currentSword.LifeStealPercentage; 

            // Testing
            damaging = true;
        }

        /// <summary>
        /// Subtract an amount of damage from the character's health
        /// </summary>
        /// <param name="damagingID">The id of the damaging object that is damaging this character</param>
        /// <param name="damage">The amount of damage to be subtracted</param>
        /// <returns>Returns a boolean indicating whether damage was taken or not</returns>
        public override float TakeDamage(int damagingID, float damage)
        {
            // If the player is not in invincibility frames
            // They can take damage
            if (dodge.canDmg)
            {
                // If the player isn't currently blocking
                // Apply the damage taken modifier
                if (GetPlayerFSMState().ID != Enums.PlayerCondition.F_Blocking)
                {
                    damage *= currentSword.DamageTakenModifier;
                }

                // The resullt of Character's Take Damage
                // Was damage taken or not
                float damageResult = base.TakeDamage(damagingID, damage);

                // If damage was taken
                // Update the playerHealth field for analytics
                if (damageResult > 0)
                {
                    // Playtest 1
                    playerHealth.CurrentValue -= damage;

                    // Does not set damaged to true if block has been triggered
                    // TODO: Better implementation of damaged flag
                    // Might need to be changed slightly eventually to better
                    // account for player taking damage from behind them
                    damaged = (blockDetector.GetComponent<BlockCollision>().BlockTriggered) ? false : true;
                }

                // Return whether damage was taken or not
                return damageResult; 
            }

            // Return 0 if the player cannot currently be damaged
            return 0; 
        }

        /// <summary>
        /// Sets the player's respawn point and rotation
        /// </summary>
        /// <param name="position">The respawn point for the player</param>
        /// <param name="rotation">The respawn rotation for the player</param>
        public void SetRespawn(Vector3 position, Vector3 rotation)
        {
            respawnPoint = position;
            respawnRotation = rotation;
        }

        public override void Respawn()
        {
            // If the player's health hasn't been reset yet
            if (Health != MaxHealth)
            {
                // Do one time only resets
                Health = MaxHealth;
                transform.position = respawnPoint;
                transform.rotation = Quaternion.Euler(respawnRotation);
                cinemachineTargetYaw = respawnRotation.y;
                cinemachineTargetPitch = respawnRotation.z;
            }

            // Call the fade in method multiple times so it can fade
            FadeIn();

            // When the fade in is done, change current state
            if(fade.GetComponent<Image>().color.a <= 0)
            {
                FSM.SetCurrentState(idleCombat);
            }
        }

        private void FadeToBlack()
        {
            // Unhide the fade out image
            if (fade.activeSelf == false)
                fade.SetActive(true);

            // If the fade isn't fully opaque
            if (fade.GetComponent<Image>().color.a < 1)
                fade.GetComponent<Image>().color = new Color(0, 0, 0, fade.GetComponent<Image>().color.a + Time.deltaTime);
            else
            {
                if (points >= maxPoints)
                {
                    SceneManager.LoadScene("WinScreen");
                }
                else
                {
                    hasFadedToBlack = true;
                    justDied = false;
                    // Set the alpha to 1.5 so it stays at full black for a little longer
                    fade.GetComponent<Image>().color = new Color(0, 0, 0, 1.5f);
                }
            }
        }

        private void FadeIn()
        {
            // If the fade isn't fully transparent
            if (fade.GetComponent<Image>().color.a > 0)
            {
                fade.GetComponent<Image>().color = new Color(0, 0, 0, fade.GetComponent<Image>().color.a - Time.deltaTime);
            }

            // Needs to be separate from above if so it triggers before state change
            if(fade.GetComponent<Image>().color.a <= 0)
            {
                fade.SetActive(false);
                hasFadedToBlack = false;
                fade.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            }
        }
    }
} 