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

        private PlayerFSMState_MOVING move;
        private PlayerFSMState_PARRY parry;
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

        public bool parryEnd;

        private void Awake()
        {

            MaxHealth = 3;
            Health = 3;
            isDamaged = false;
            inState = false;

            parryEnd = false;

            // Creates the FSM
            Movement_FSM = new FiniteStateMachine();
            Combat_FSM = new FiniteStateMachine();

            // Creates all of the states
            parry = new PlayerFSMState_PARRY(parryDetector, inputs, this);
            block = new PlayerFSMState_BLOCK(blockDetector);
            move = new PlayerFSMState_MOVING(this, inputs, GetComponent<Animator>(), GroundLayers);
            idleMovement = new PlayerFSMState_IDLE(GetComponent<Animator>());
            idleCombat = new PlayerFSMState_IDLE(GetComponent<Animator>());
            attack = new PlayerFSMState_ATTACK(this, inputs, GetComponent<Animator>(), sword);
            death = new PlayerFSMState_DEATH(this);
            takeDamage = new PlayerFSMState_TAKEDAMAGE(this);
            dodge = new PlayerFSMState_DODGE(this, inputs, GetComponent<Animator>(), GroundLayers);
            jump = new PlayerFSMState_JUMP(this, inputs, GroundLayers);
            nullState = new PlayerFSMState_NULL();

            // Adds all of the possible transitions
            // These are the possible transitions for the Player's Movement
            Movement_FSM.AddTransition(move, idleMovement, IsIdle());
            Movement_FSM.AddTransition(idleMovement, move, IsMoving());
            Movement_FSM.AddTransition(move, dodge, IsDodging());
            Movement_FSM.AddTransition(dodge, idleMovement, IsDodgingStopped());
            Movement_FSM.AddTransition(jump, idleMovement, IsGrounded());
            Movement_FSM.AddTransition(move, jump, IsJumping());
            Movement_FSM.AddTransition(idleMovement, jump, IsJumping());
            Movement_FSM.AddTransition(idleMovement, dodge, IsDodging());

            // NULL state for when player is in either TAKEDAMAGE or DEAD
            Movement_FSM.AddAnyTransition(nullState, IsNull());
            Movement_FSM.AddTransition(nullState, idleMovement, NotNull());

            // These are the possible transitions for the Player's Combat
            Combat_FSM.AddTransition(idleCombat, attack, IsAttacking());
            Combat_FSM.AddTransition(attack, idleCombat, IsCombatIdle());
            Combat_FSM.AddTransition(idleCombat, death, Alive());
            Combat_FSM.AddTransition(attack, death, Alive());
            //Combat_FSM.AddTransition(idleCombat, parry, IsBlockReleased());
            //Combat_FSM.AddTransition(parry, idleCombat, IsReleased());
            Combat_FSM.AddTransition(idleCombat, block, IsBlockPressed());
            Combat_FSM.AddTransition(block, parry, IsBlockReleased());
            Combat_FSM.AddTransition(parry, idleCombat, IsParryReleased());

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
        public Func<bool> IsCombatIdle() => () => (attack.Timer >= 1) && !inputs.parry;

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
        public Func<bool> IsReleased() => () => parry.timer >= 0.5;

        /// <summary>
        /// Checks if the player is grounded
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsGrounded() => () => GroundedCheck();

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
        /// <param name="dmg"></param>
        public void TakingDamage(float dmg)
        {
            if (dodge.canDmg)
            {
                Health -= dmg;
                isDamaged = true;
            }
        }

       


    }
}