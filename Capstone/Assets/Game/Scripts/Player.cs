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

        [SerializeField]
        private CinemachineFreeLook freeLookCam;
        #endregion

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
        private int animIDSwordChoice;
        #endregion

        #region Damaging System Fields
        [Header("Damaging Timer Fields (Testing)")]
        private bool damaging;
        #endregion

        #region UI Fields
        private float points = 0;
        private float maxPoints = 8;

        [SerializeField] private GameObject fade;
        public bool hasFadedToBlack;
        public bool justDied;
        #endregion

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
            death = new PlayerFSMState_DEATH(this, animator);
            takeDamage = new PlayerFSMState_TAKEDAMAGE(this, animator);
            dodge = new PlayerFSMState_DODGE(this, inputs, animator, GroundLayers);
            jump = new PlayerFSMState_JUMP(this, inputs, GroundLayers, landTimeout);
            nullState = new PlayerFSMState_NULL();

            // Adds all of the possible transitions
            FSM.AddTransition(idleCombat, attack, IsAttacking());
            FSM.AddTransition(attack, idleCombat, IsCombatIdle());
            FSM.AddTransition(idleCombat, dodge, IsDodging());
            FSM.AddTransition(dodge, idleCombat, IsDodgingStopped());

            FSM.AddTransition(idleCombat, block, IsBlockPressed());
            FSM.AddTransition(block, parryAttempt, IsBlockReleased());
            FSM.AddTransition(parryAttempt, parrySuccess, IsParrySuccessful());
            FSM.AddTransition(parrySuccess, idleCombat, IsParryFinished()); 
            FSM.AddTransition(parryAttempt, idleCombat, IsParryFinished());


            cinemachineTargetYaw = player.transform.rotation.eulerAngles.y;

            FSM.AddAnyTransition(death, Dead());

            FSM.AddAnyTransition(takeDamage, IsDamaged());
            FSM.AddTransition(takeDamage, idleCombat, IsAbleToDamage());

            // Sets the current state
            FSM.SetCurrentState(idleCombat);

            targetLock = GetComponent<TargetLock>();

            inputs.player = this;

            currentSword = swords[SwordType.Quartz].GetComponent<Sword>();
            animIDSwordChoice = Animator.StringToHash("Sword Choice");
        }


        private void Update()
        {
            FSM.Tick();

            Jump();
            Move();

            // If the player is dead and just died (fadeToBlack is still occuring)
            if(points >= maxPoints)
            {
                FadeToBlack();
            }
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


            // If the player isn't moving set their speed to 0
            if (inputs.move == Vector2.zero) speed = 0.0f;

            // Animator input
            // animator.SetFloat(animIDForward, speed / targetSpeed);
            animBlend = Mathf.Lerp(animBlend, speed, Time.deltaTime * SpeedChangeRate);

            // normalise input direction
            inputDirection = new Vector3(inputs.move.x, 0.0f, inputs.move.y).normalized;

            // Recentering code for the camera
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

            // Applies gravity
            if (verticalVelocity < terminalVelocity)
            {
                verticalVelocity += Gravity * Time.deltaTime;
            }

            // move the player
            controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

            if (hasAnimator)
            {
                animator.SetFloat(animIDForward, animBlend);
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
                sword.transform.localScale = currentSword.Offset.localScale;

                // Update the box collider dimensions
                sword.GetComponent<BoxCollider>().center = swords[newSwordType].GetComponent<BoxCollider>().center;
                sword.GetComponent<BoxCollider>().size = swords[newSwordType].GetComponent<BoxCollider>().size;

                // Set the animation paramater to change the attack animation
                animator.SetFloat(animIDSwordChoice, (float)currentSword.SwordType);

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

            damaging = true;
        }

        public void ClearDamaging()
        {
            // If the player is currently damaging an object
            if (damaging)
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
                damaging = false;
            }
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
                    if(Health <= 0)
                    {
                        damaged = true;
                    }
                    
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

        /// <summary>
        /// Respawns the player
        /// </summary>
        public override void Respawn()
        {
            // If the player's health hasn't been reset yet
            if (Health != MaxHealth)
            {
                // Do one time only resets
                Health = MaxHealth;
                transform.position = respawnPoint;
                transform.rotation = Quaternion.Euler(respawnRotation);
            }
            damaged = false; 

            // Call the fade in method multiple times so it can fade
            StartCoroutine(FadeIn());

            FSM.SetCurrentState(idleCombat);
        }

        public void FadeToBlack()
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

        private IEnumerator FadeIn()
        {
            // If the fade isn't fully transparent
            while (fade.GetComponent<Image>().color.a > 0)
            {
                fade.GetComponent<Image>().color = new Color(0, 0, 0, fade.GetComponent<Image>().color.a - Time.deltaTime);
                yield return null;
            }

            // Needs to be separate from above if so it triggers before state change
            if (fade.GetComponent<Image>().color.a <= 0)
            {
                fade.SetActive(false);
                hasFadedToBlack = false;
                fade.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            }
        }
    }
} 