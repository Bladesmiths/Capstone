using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;

using StarterAssets;

namespace Bladesmiths.Capstone
{
    public class Player : Character, IDamageable
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

        private PlayerFSMState_MOVING move;
        private PlayerFSMState_PARRY parry;
        private PlayerFSMState_IDLE idle;

        
        PlayerFSMState_ATTACK attack;
        PlayerFSMState_DEATH death;
        PlayerFSMState_TAKEDAMAGE takeDamage;
        PlayerFSMState_DODGE dodge;
        PlayerFSMState_JUMP jump;

        public bool isDamaged;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private const float _threshold = 0.01f;
        [SerializeField] private GameObject CinemachineCameraTarget;
        private float TopClamp = 70.0f;
        private float BottomClamp = -30.0f;
        private float CameraAngleOverride = 0.0f;
        private bool LockCameraPosition = false;

        public LayerMask GroundLayers;
        public bool Grounded = true;
        public bool isGrounded = true;
        public float GroundedOffset = -0.10f;
        public float GroundedRadius = 0.20f;

        private void Awake()
        {

            MaxHealth = 3;
            Health = 3;
            isDamaged = false;

            // Creates the FSM
            Movement_FSM = new FiniteStateMachine();
            Combat_FSM = new FiniteStateMachine();

            // Creates all of the states

            parry = new PlayerFSMState_PARRY(parryDetector);
            move = new PlayerFSMState_MOVING(this, inputs, GetComponent<Animator>(), GroundLayers);
            idle = new PlayerFSMState_IDLE();
            attack = new PlayerFSMState_ATTACK(this, inputs, GetComponent<Animator>(), sword);
            death = new PlayerFSMState_DEATH();
            takeDamage = new PlayerFSMState_TAKEDAMAGE(this);
            dodge = new PlayerFSMState_DODGE(this, inputs, GetComponent<Animator>(), GroundLayers);
            jump = new PlayerFSMState_JUMP(this, inputs, GroundLayers);

            // Adds all of the possible transitions
            Movement_FSM.AddTransition(move, idle, IsIdle());
            Movement_FSM.AddTransition(idle, move, IsMoving());
            Movement_FSM.AddTransition(move, dodge, IsDodging());
            Movement_FSM.AddTransition(dodge, move, IsDodgingStopped());
            Movement_FSM.AddTransition(jump, idle, IsGrounded());
            Movement_FSM.AddTransition(move, jump, IsJumping());
            Movement_FSM.AddTransition(idle, jump, IsJumping());


            Combat_FSM.AddTransition(idle, attack, IsAttacking());
            Combat_FSM.AddTransition(attack, idle, IsIdle());
            Combat_FSM.AddTransition(idle, death, Alive());
            Combat_FSM.AddTransition(attack, death, Alive());
            Combat_FSM.AddTransition(idle, parry, IsBlockReleased());
            Combat_FSM.AddTransition(parry, idle, IsReleased());



            // Sets the current state
            Combat_FSM.SetCurrentState(idle);
            Movement_FSM.SetCurrentState(idle);


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
        /// The condition fro going between the BLOCK and PARRY state
        /// Should be replaced with the input system call later on and not hard coded to right click
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsBlockReleased() => () => inputs.parry == true;


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
        public Func<bool> IsAbleToDamage() => () => takeDamage.timer >= 1f;

        /// <summary>
        /// The condition for going from MOVE to DODGE state
        /// </summary>
        public Func<bool> IsDodging() => () => inputs.dodge;

        /// <summary>
        /// The condition for going from DODGE to MOVE state
        /// </summary>
        /// <returns></returns>
        // TODO: Should implement something like when dodging animation stops
        public Func<bool> IsDodgingStopped() => () => dodge.timer >= 0.7;

        /// <summary>
        /// The condition for having been attacked
        /// </summary>
        /// <returns></returns>
        public Func<bool> Alive() => () => Health == 0;

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

        private void Update()
        {
            Movement_FSM.Tick();
            Combat_FSM.Tick();

        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private bool GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(this.transform.position.x, this.transform.position.y - GroundedOffset, this.transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            return Grounded;

        }

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

        public void TakingDamage(float dmg)
        {
            Health -= dmg;
            isDamaged = true;

        }

       


    }
}