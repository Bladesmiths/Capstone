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

using Bladesmiths.Capstone.UI;
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
        [SerializeField]
        private BalancingData currentBalancingData;

        //[SerializeField] private TransitionManager playerTransitionManager;
        [Header("Player Fields")]

        public static Player instance;

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
        private Quaternion respawnRotation;
        [SerializeField]
        private float respawnCamAxisValue;

        public bool canDmg;

        [OdinSerialize]
        private Dictionary<PlayerCondition, float> speedValues = new Dictionary<PlayerCondition, float>();

        [OdinSerialize]
        public Dictionary<SwordType, GameObject> shatterSwords = new Dictionary<SwordType, GameObject>();

        [SerializeField] private GameObject boss;
        
        public GameObject swordHilt;

        private TargetLock targetLock;
        public BossTrigger bossTrigger;

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
        public Sword currentSword;
        [OdinSerialize]
        public Dictionary<SwordType, GameObject> allSwords = new Dictionary<SwordType, GameObject>();
        [OdinSerialize]
        public Dictionary<SwordType, GameObject> currentSwords = new Dictionary<SwordType, GameObject>();
        [OdinSerialize]
        private Dictionary<SwordType, GameObject> swordsGeo = new Dictionary<SwordType, GameObject>();
        private int animIDSwordChoice;
        #endregion

        #region Health-Related Fields
        private float provisionalDamageDecayDelayTimer;
        private float provisionalDamageDecayTimer;
        #endregion

        #region Damaging System Fields
        [Header("Damaging Timer Fields (Testing)")]
        private bool damaging;
        #endregion

        #region UI Fields
        private float points = 0;
        private float maxPoints = 8;

        [SerializeField] private UIManager uiManager;
        [SerializeField] private GameObject fade;
        public bool hasFadedToBlack;
        public bool justDied;
        public bool shouldLookAt = true;
        #endregion

        #region Fields from the Move State and Jump State

        [Header("Move/Jump Fields")]
        public float timer;

        public Animator animator;

        private int animIDForward;
        private int animIDGrounded;
        private int animIDJump;
        private int animIDFreeFall;
        private int animIDDamaged;
        private int animIDDead; 
        private int animIDSpeed;
        private int animIDAttack;
        private int animIDMotionSpeed;
        private int animIDBlock;
        private int animIDDodge;
        private int animIDMoving;

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

        public CinemachineFreeLook FreeLookCam { get => freeLookCam; }

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
        public override ObjectController ObjectController 
        { 
            get => objectController; 
            set
            {
                objectController = value;
                targetLock.ObjectController = value;
            }
        }
        public ParryCollision ParryDetector { get => parryDetector.GetComponent<ParryCollision>(); }
        public GameObject ParryDetectorObject
        {
            get => parryDetector.gameObject;
        }
        public GameObject BlockDetector
        {
            get => blockDetector.gameObject;
        }
        public float ChipDamageTotal { get; private set; }
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
        public GameObject Sword { get => sword; }
        #endregion
        #endregion

        private void Awake()
        {
            Health = 1000;
            animator = GetComponent<Animator>();
            animIDForward = Animator.StringToHash("Forward");
            animIDAttack = Animator.StringToHash("Attack");
            animIDBlock = Animator.StringToHash("Block");
            animIDDodge = Animator.StringToHash("Dodge");
            animIDMoving = Animator.StringToHash("Moving");
            animIDDead = Animator.StringToHash("Dead");
            animBlend = 0;
            dodgeTimer = 0;
            canDmg = true;

            if (instance == null)
            {
                instance = this;
            }

            jumpTimeoutDelta = JumpTimeout;
            fallTimeoutDelta = FallTimeout;

            controller = GetComponent<CharacterController>();
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera");

            hasAnimator = TryGetComponent(out animator);
            animIDGrounded = Animator.StringToHash("Grounded");
            animIDJump = Animator.StringToHash("Jump");
            animIDFreeFall = Animator.StringToHash("FreeFall");
            animIDDamaged = Animator.StringToHash("Damaged");

            LandTimeoutDelta = -1.0f;

            isGrounded = true;
            controllerVelocity = Vector2.zero;

            Health = MaxHealth;
            damaged = false;
            inState = false;

            parryEnd = false;

            // Subscribe to the FSM's OnStateChange event
            //FSM.OnStateChange += SpeedUpdate;

            // Subscribing parry collision to block collision events to keep those fields updated
            blockDetector.GetComponent<BlockCollision>().OnBlock += BlockOccured;
            parryDetector.GetComponent<ParryCollision>().Player = this;

            cinemachineTargetYaw = player.transform.rotation.eulerAngles.y;

            targetLock = GameObject.Find("TargetLockManager").GetComponent<TargetLock>();

            inputs.player = this;

            currentSword = allSwords[SwordType.Topaz].GetComponent<Sword>();
            animIDSwordChoice = Animator.StringToHash("Sword Choice");

            SetRespawn(transform, freeLookCam.m_XAxis.Value);

            ResetChipDamageTimers(); 
        }

        void Start()
        {
            boss = Boss.instance.gameObject;
        }

        private void Update()
        {
            //FSM.Tick();

            Move();
            Jump();

            DecayProvisionalDamage();

            // If the player is dead and just died (fadeToBlack is still occuring)
            if (points >= maxPoints)
            {
                StartCoroutine(FadeToBlack());
            }

        }

        /// <summary>
        /// Allows for other classes to get a reference to the player's state
        /// </summary>
        /// <returns></returns>
        public PlayerCondition GetPlayerFSMState()
        {
            return CheckAnimationBehavior(animator.GetCurrentAnimatorStateInfo(0)).ID;
        }

        /// <summary>
        /// Adds points for the second playtest
        /// </summary>
        public void AddPoints()
        {
            points++;
        }

        public void AddToMaxPoints()
        {
            points = maxPoints;
        }

        /// <summary>
        /// Runs whenever the state changes in the FSM
        /// Attaches to the Delegate in the class
        /// </summary>
        public void SpeedUpdate(AnimatorStateInfo stateInfo)
        {   
            if (!speedValues.TryGetValue(CheckAnimationBehavior(stateInfo).ID, out targetSpeed))
            {
                Debug.Log($"Speed Update Failed. Current State is {CheckAnimationBehavior(stateInfo).ID}");
            }           
        }

        /// <summary>
        /// Checks to see which animation behavior is active
        /// </summary>
        /// <param name="stateInfo"></param>
        /// <returns></returns>
        public PlayerState_Base CheckAnimationBehavior(AnimatorStateInfo stateInfo)
        {
            if (animator.GetBehaviours(stateInfo.fullPathHash, 0).Length != 0)
            {
                return (PlayerState_Base)animator.GetBehaviours(stateInfo.fullPathHash, 0)[0];                
            }
            return null;
        }

        /// <summary>
        /// Resets the parameters in the animator
        /// </summary>
        public void ResetAnimationParameters()
        {
            //animator.SetBool(animIDBlock, false);
            animator.SetBool(animIDDodge, false);
            animator.SetFloat(animIDForward, 0);
            animator.SetBool(animIDJump, false);
            animator.SetBool(animIDAttack, false);
            animator.SetBool(animIDMoving, false);
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
            speed = targetSpeed * currentBalancingData.SwordData[currentSword.SwordType].PlayerMovementMultiplier;

            // if the input is greater than 1 then set the speed to the max
            if (inputs.move.magnitude <= 1)
            {
                speed *= inputs.move.magnitude;
            }

            // If the player isn't moving set their speed to 0
            if (inputs.move == Vector2.zero) speed = 0.0f;

            // Animator input
            // animator.SetFloat(animIDForward, speed / targetSpeed);
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
            if (controller.isGrounded || controller.velocity.magnitude == 0)
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
                // Set the old sword data and model to inactive and the new to active
                currentSwords[CurrentSword.SwordType].SetActive(false);
                GameObject shatterSword = Instantiate(shatterSwords[CurrentSword.SwordType], 
                    swordsGeo[CurrentSword.SwordType].transform.GetChild(0).position,
                    swordsGeo[CurrentSword.SwordType].transform.GetChild(0).rotation);

                PlayerShatterSword shatter = shatterSword.GetComponent<PlayerShatterSword>();

                

                currentSwords[newSwordType].SetActive(true);

                swordsGeo[CurrentSword.SwordType].SetActive(false);
                swordsGeo[newSwordType].SetActive(true);

                // Update the current sword field
                currentSword = currentSwords[newSwordType].GetComponent<Sword>();

                // Update the position according to offset
                //sword.transform.localPosition = currentSword.Offset.position;
                //sword.transform.localRotation = currentSword.Offset.rotation;
                //sword.transform.localScale = currentSword.Offset.localScale;

                // This doesn't work apparently, but I'm leaving the code here as a record
                // of the transforms that we need for the swords to look correct
                //if (newSwordType == SwordType.Ruby)
                //{
                //    sword.transform.Find("SwordHilt").position = new Vector3(0.094f, 0.008f, 0);
                //    sword.transform.position = new Vector3(0.029f, 0.08f, 0.0174f);
                //    sword.transform.rotation = Quaternion.Euler(70.354f, -15.63f, -26.015f); 
                //}
                //else
                //{
                //    sword.transform.Find("SwordHilt").position = Vector3.zero;
                //    sword.transform.position = new Vector3(-0.007f, 0.074f, 0.022f);
                //    sword.transform.rotation = Quaternion.identity;
                //}

                // Update the box collider dimensions
                sword.GetComponent<BoxCollider>().center = currentSwords[newSwordType].GetComponent<BoxCollider>().center;
                sword.GetComponent<BoxCollider>().size = currentSwords[newSwordType].GetComponent<BoxCollider>().size;

                sword.GetComponent<Sword>().SwordType = newSwordType;
                
                // Set the animation paramater to change the attack animation
                animator.SetFloat(animIDSwordChoice, (float)currentSword.SwordType);

                // TODO: Player sword switching animation

                //Update player health bar color
                uiManager.SwitchSwordHealthBar(currentSword.SwordType);
            }
        }

        /// <summary>
        /// Attack with the player's sword
        /// </summary>
        /// <param name="targetID">The id of the object to attack</param>
        public void SwordAttack(int targetID)
        {
            IdentifiedTeamPair damageablePair = ObjectController[targetID];
            float damageDealt = 0;
            if (damageablePair != null)
            {
                damageDealt = ((IDamageable)damageablePair.IdentifiedObject).TakeDamage(currentSword.ID, Damage);
            }
            Health += damageDealt * currentSword.LifeStealPercentage;

            if (damageDealt != 0)
            {
                currentSword.damaging = true;
            }
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
            if (canDmg)
            {
                // If the player isn't currently blocking
                // Apply the damage taken modifier
                if (GetPlayerFSMState() != Enums.PlayerCondition.F_Blocking)
                {
                    damage *= currentSword.DamageTakenModifier;
                    ChipDamageTotal = 0;
                    ResetChipDamageTimers(); 
                }

                // The resullt of Character's Take Damage
                // Was damage taken or not
                float damageResult = 0;

                if (objectController[damagingID] != null)
                {
                    damageResult = base.TakeDamage(damagingID, damage);
                }
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
                        animator.SetTrigger(animIDDamaged);
                        animator.SetBool(animIDDead, true);
                    }

                    //Enter take damage animation unless in ruby form
                    if (damaged && currentSword.SwordType != SwordType.Ruby)
                    {
                        animator.SetTrigger(animIDDamaged);
                    }
                }

                // Return whether damage was taken or not
                return damageResult; 
            }

            // Return 0 if the player cannot currently be damaged
            return 0; 
        }

        /// <summary>
        /// Method hooked to block event that updates fields when a block occurs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newChipDamageTotal"></param>
        public void BlockOccured(float newChipDamage)
        {
            ChipDamageTotal += newChipDamage;
            ResetChipDamageTimers();
        }

        /// <summary>
        /// Resets the chip damage field
        /// </summary>
        public void ResetChipDamage()
        {
            ChipDamageTotal = 0;
        }

        /// <summary>
        /// Checks and updates provisional damage timers and does provisional 
        /// damage decay if necessary
        /// </summary>
        public void DecayProvisionalDamage()
        {
            if (provisionalDamageDecayDelayTimer <= 0)
            {
                provisionalDamageDecayTimer -= Time.deltaTime;
            }
            else
            {
                provisionalDamageDecayDelayTimer -= Time.deltaTime;
            }

            if (provisionalDamageDecayDelayTimer <= 0)
            {
                if (provisionalDamageDecayTimer <= 0)
                {
                    if (ChipDamageTotal >= currentBalancingData.ProvisionalDamageDecayRate)
                    {
                        ChipDamageTotal -= currentBalancingData.ProvisionalDamageDecayRate;
                    }
                    else
                    {
                        ChipDamageTotal = 0;
                    }

                    provisionalDamageDecayTimer = currentBalancingData.ProvisionalDamageDecayTimeLimit;
                }
            }
        }

        /// <summary>
        /// Resets the 2 timers related to provisional damage
        /// </summary>
        public void ResetChipDamageTimers()
        {
            provisionalDamageDecayDelayTimer = currentBalancingData.ProvisionalDamageDecayDelay;
            provisionalDamageDecayTimer = currentBalancingData.ProvisionalDamageDecayTimeLimit;
        }

        /// <summary>
        /// Sets the player's respawn point and rotation
        /// </summary>
        /// <param name="position">The respawn point for the player</param>
        /// <param name="rotation">The respawn rotation for the player</param>
        public void SetRespawn(Transform respawnTransform, float camXAxisValue)
        {
            respawnPoint = respawnTransform.position;
            respawnRotation = respawnTransform.rotation;
            respawnCamAxisValue = camXAxisValue;
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
                uiManager.ResetChunks();
                transform.position = respawnPoint;
                freeLookCam.m_XAxis.Value = respawnCamAxisValue;
                player.GetComponent<Animator>().rootRotation = respawnRotation;
                animator.SetBool(animIDDead, false);
            }
            damaged = false; 

            Boss.instance.Health = Boss.instance.MaxHealth;
            bossTrigger.BossTriggerReset();

            // Call the fade in method multiple times so it can fade
            StartCoroutine(FadeIn());
        }

        public IEnumerator FadeToBlack()
        {
            // Unhide the fade out image
            if (fade.activeSelf == false)
                fade.SetActive(true);

            // If the fade isn't fully opaque
            while (fade.GetComponent<Image>().color.a < 1)
            {
                fade.GetComponent<Image>().color = new Color(0, 0, 0, fade.GetComponent<Image>().color.a + Time.deltaTime);
                yield return new WaitForSeconds(0.25f);
            }
            
            if (fade.GetComponent<Image>().color.a >= 1)
            {
                //Debug.Log("Loading winscreen");

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
                yield return new WaitForSeconds(0.25f);
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