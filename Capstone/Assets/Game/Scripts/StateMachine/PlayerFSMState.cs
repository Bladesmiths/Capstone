using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using StarterAssets;


namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Base class for possible states for the Player
    /// </summary>
    public class PlayerFSMState : IState
    {
        protected PlayerCondition id;

        public PlayerCondition ID { get; set; }

        public virtual void Tick()
        {

        }

        public virtual void OnEnter()
        {
            
        }

        public virtual void OnExit()
        {

        }


    }

    /// <summary>
    /// The state that sllows for the player to move
    /// </summary>
    public class PlayerFSMState_MOVING : PlayerFSMState
    {
        public float timer;

        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;

        private int _animIDForward;
        private int _animIDDodge;
        private int _animIDMotionSpeed;
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

        public bool Grounded = true;
        public bool isGrounded = true;
        public float GroundedOffset = -0.10f;
        public float GroundedRadius = 0.20f;
        public LayerMask GroundLayers;
        private int _animIDGrounded;

        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        private GameObject camera;


        public float RunSpeed = 10.0f;
        public float WalkSpeed = 4.0f;
        public float MoveSpeedCurrentMax = 10.0f;


        public PlayerFSMState_MOVING(Player player, PlayerInputsScript input, Animator animator, LayerMask layers)
        {
            _player = player;
            _input = input;
            _animator = animator;
            GroundLayers = layers;
        }

        public override void Tick()
        {
            _hasAnimator = _player.TryGetComponent(out _animator);


            //if(_input.move == Vector2.zero)
            //{
            //    timer += Time.deltaTime;

            //}

            //Vector2 movement = _input.move.normalized * (10 * Time.deltaTime);
            //_controller.Move(new Vector3(movement.x, 0, movement.y));


            if (Grounded)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }
            else
            {
                _verticalVelocity = 0;

            }


            Vector3 inputDirection = Vector3.zero;
            Vector3 targetDirection = Vector3.zero;


            float targetSpeed = _input.move.normalized.magnitude * MoveSpeedCurrentMax;

            if (targetSpeed > 0 && targetSpeed < WalkSpeed)
            {
                targetSpeed = WalkSpeed;
            }

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;


            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude; //maybe normalize this instead?

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

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

            // normalise input direction
            inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(_player.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                // rotate to face input direction relative to camera position
                _player.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            }
            targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;


            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


            GroundedCheck();

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDForward, _animBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }

        }

        public override void OnEnter()
        {
            _animIDForward = Animator.StringToHash("Forward");
            _animBlend = 0;

            //timer = 0;
            _controller = _player.GetComponent<CharacterController>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        public override void OnExit()
        {

        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(_player.transform.position.x, _player.transform.position.y - GroundedOffset, _player.transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            

        }
    }

    /// <summary>
    /// The state for when the player is not moving
    /// </summary>
    public class PlayerFSMState_IDLE : PlayerFSMState
    {
        private Animator _animator;

        private int _animIDIdle;
        private int _animIDForward;

        public PlayerFSMState_IDLE()
        {
            
        }

        public PlayerFSMState_IDLE(Animator animator)
        {
            _animator = animator;
        }

        public override void Tick()
        {
            _animator.SetBool(_animIDIdle, true);
        }

        public override void OnEnter()
        {
            _animIDIdle = Animator.StringToHash("Idle");
            _animIDForward = Animator.StringToHash("Forward");
            _animator.SetFloat(_animIDForward, 0.0f);
        }

        public override void OnExit()
        {
            _animator.SetBool(_animIDIdle, false);
        }

    }

    /// <summary>
    /// The state for when the Player is parrying enemy attacks
    /// </summary>
    public class PlayerFSMState_PARRY : PlayerFSMState
    {
        public float timer;
        private GameObject _playerParryBox;
        private PlayerInputsScript _input;
        public PlayerFSMState_PARRY(GameObject playerParryBox, PlayerInputsScript input)
        {
            _playerParryBox = playerParryBox;
            _input = input;
        }

        public override void Tick()
        {
            timer += Time.deltaTime;
            
            _playerParryBox.SetActive(true);
        }

        public override void OnEnter()
        {
            timer = 0;
        }

        public override void OnExit()
        {
            _input.parry = false;
            _playerParryBox.SetActive(false);
            _playerParryBox.GetComponent<MeshRenderer>().material.color = Color.white;
        }

    }

    /// <summary>
    /// The state for when the Player attacks
    /// </summary>
    public class PlayerFSMState_ATTACK : PlayerFSMState
    {
        private Player _player;
        private PlayerInputsScript _input;
        private Animator _animator;
        private GameObject _sword;

        private int _animIDSpeed;
        private int _animIDAttack;
        private int _animIDMotionSpeed;
        private bool _hasAnimator;

        public PlayerFSMState_ATTACK(Player player, PlayerInputsScript input, Animator animator, GameObject sword)
        {
            _player = player;
            _input = input;
            _animator = animator;
            _sword = sword;
            _sword.GetComponent<Rigidbody>().detectCollisions = false;

        }

        public override void Tick()
        {
            _sword.GetComponent<Rigidbody>().detectCollisions = true;
            //int layerMask = 1 << 8;
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDAttack, false);

            }

            //layerMask = ~layerMask;
            RaycastHit hit;

            //if (Physics.Raycast(_player.transform.position + Vector3.up, _player.transform.TransformDirection(Vector3.forward), out hit, 2f))
            if (_sword.GetComponent<Rigidbody>().detectCollisions)
            {

                //Physics.Raycast(_player.transform.position + Vector3.up, _player.transform.TransformDirection(Vector3.forward), out hit, 2f);

                //Debug.DrawRay(_player.transform.position + Vector3.up, _player.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");

                //if (hit.collider.gameObject.GetComponent<Enemy>())
                //{
                //    hit.collider.gameObject.GetComponent<Enemy>().Damaged();

                //}


            }
            else
            {
                //Debug.DrawRay(_player.transform.position, _player.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");

            }

            if (_input.attack)
            {
                if (_hasAnimator)
                {
                    //_animator.SetBool(_animIDAttack, true);

                }
            }

            _input.attack = false;

        }

        public override void OnEnter()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDAttack = Animator.StringToHash("Attack");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

            if (_animator != null)
            {
                _hasAnimator = true;
            }
            else
            {
                _hasAnimator = false;
            }

        }

        public override void OnExit()
        {
            _sword.GetComponent<Rigidbody>().detectCollisions = false;
        }

    }

    /// <summary>
    /// The state for when the player takes damage
    /// CURRENTLY NOT IN USE
    /// </summary>
    public class PlayerFSMState_TAKEDAMAGE : PlayerFSMState
    {
        private Player _player;
        public float timer;

        public PlayerFSMState_TAKEDAMAGE(Player player)
        {
            _player = player;

        }

        public override void Tick()
        {
            timer += Time.deltaTime;

        }

        public override void OnEnter()
        {
            _player.isDamaged = false;
            timer = 0;
            _player.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.blue;
            _player.inState = true;

        }

        public override void OnExit()
        {
            _player.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;
            _player.inState = false;
        }

    }

    /// <summary>
    /// The state for when the Player is dead
    /// </summary>
    public class PlayerFSMState_DEATH : PlayerFSMState
    {
        Player _player;
        public PlayerFSMState_DEATH(Player player)
        {
            _player = player;
        }

        public override void Tick()
        {

        }

        public override void OnEnter()
        {
            _player.inState = true;
        }

        public override void OnExit()
        {
            _player.inState = false;
        }

    }

    /// <summary>
    /// The state for when the Player is dead
    /// </summary>
    public class PlayerFSMState_NULL : PlayerFSMState
    {
        public PlayerFSMState_NULL()
        {

        }

        public override void Tick()
        {

        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

    }

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

        private int _animIDSpeed;
        private int _animIDDodge;
        private int _animIDMotionSpeed;
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

        public PlayerFSMState_DODGE(Player player, PlayerInputsScript input, Animator animator, LayerMask layers)
        {
            _player = player;
            _input = input;
            _animator = animator;
            GroundLayers = layers;
        }

        public override void Tick()
        {
            timer += Time.deltaTime;
            _input.dodge = false;

            dmgTimer += Time.deltaTime;
            if(dmgTimer >= 0.1f)
            {
                canDmg = true;
                _player.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;

            }
            else
            {
                _player.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;

            }

            //Vector2 movement = _input.move.normalized * (10 * Time.deltaTime);
            //_controller.Move(new Vector3(movement.x, 0, movement.y));


            if (Grounded)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }


            Vector3 inputDirection = Vector3.zero;
            Vector3 targetDirection = Vector3.zero;


            float targetSpeed = 20;

            //if (_input.move == Vector2.zero) targetSpeed = 0.0f;


            //if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * 1, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            inputDirection = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).normalized;

            if (inputDirection.magnitude == 0)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _player.transform.eulerAngles.y;
                inputDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward * -1;

                //inputDirection = new Vector3(0, 0, -1) + new Vector3(_player.transform.rotation.eulerAngles.y, 0.0f, 0.0f);

                inputDirection.Normalize();

            }

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            //if (_input.move != Vector2.zero)
            //{
            //    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
            //    float rotation = Mathf.SmoothDampAngle(_player.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            //    // rotate to face input direction relative to camera position
            //    _player.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            //    targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            //}


            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            // move the player
            _controller.Move(inputDirection * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


            GroundedCheck();

        }

        public override void OnEnter()
        {
            timer = 0;
            dmgTimer = 0;
            canDmg = false;
            _controller = _player.GetComponent<CharacterController>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        public override void OnExit()
        {
            canDmg = true;
           _controller.SimpleMove(Vector3.zero);
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(_player.transform.position.x, _player.transform.position.y - GroundedOffset, _player.transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);


        }
    }

    /// <summary>
    /// The state for when the player is jumping
    /// </summary>
    public class PlayerFSMState_JUMP : PlayerFSMState
    {
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;

        private Player _player;
        private PlayerInputsScript _input;
        private CharacterController _controller;
        private Animator _animator;
        private int _animIDJump;
        private int _animIDFreeFall;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        public bool Grounded = true;
        public bool isGrounded = true;
        public float GroundedOffset = -0.10f;
        public float GroundedRadius = 0.20f;
        public LayerMask GroundLayers;
        private int _animIDGrounded;

        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        private Vector3 controllerVelocity;


        public bool _hasAnimator;

        public PlayerFSMState_JUMP(Player player, PlayerInputsScript input, LayerMask layers)
        {
            _player = player;
            _input = input;
            GroundLayers = layers;
            isGrounded = true;
        }

        public override void Tick()
        {
            if (Grounded)
            {
                isGrounded = true;

                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    //_animator.SetBool(_animIDJump, false);
                    //_animator.SetBool(_animIDFreeFall, false);
                    _verticalVelocity = 0f;
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump/* && _jumpTimeoutDelta <= 0.0f*/)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        //_animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                //if (_jumpTimeoutDelta >= 0.0f)
                //{
                //    _jumpTimeoutDelta -= Time.deltaTime;

                //}
                //else
                //{

                //}

                controllerVelocity = _controller.velocity.normalized;

                _controller.Move(new Vector3(controllerVelocity.x * 10, _verticalVelocity, controllerVelocity.z * 10) * Time.deltaTime);



            }
            else
            {
                // reset the jump timeout timer
                //_jumpTimeoutDelta = JumpTimeout;
                isGrounded = false;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {

                    _fallTimeoutDelta -= Time.deltaTime;
                }
                
                // if we are not grounded, do not jump
                _input.jump = false;

                controllerVelocity = _controller.velocity.normalized;

                _controller.Move(new Vector3(controllerVelocity.x * 10, _verticalVelocity, controllerVelocity.z * 10) * Time.deltaTime);

            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            GroundedCheck();


        }

        public override void OnEnter()
        {
            _hasAnimator = _player.TryGetComponent(out _animator);
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _controller = _player.gameObject.GetComponent<CharacterController>();

            isGrounded = false;
        }

        public override void OnExit()
        {

        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(_player.transform.position.x, _player.transform.position.y - GroundedOffset, _player.transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                //_animator.SetBool(_animIDGrounded, Grounded);
            }

        }

    }


    
}
