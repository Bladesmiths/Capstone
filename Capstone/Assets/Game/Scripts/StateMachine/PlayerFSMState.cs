using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using StarterAssets;


namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Base class for possible states
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

    public class PlayerFSMState_MOVING : PlayerFSMState
    {
        public float timer;

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

        private GameObject camera;

        public PlayerFSMState_MOVING(Player player, PlayerInputsScript input, Animator animator)
        {
            _player = player;
            _input = input;
            _animator = animator;
        }

        public override void Tick()
        {
            //if(_input.move == Vector2.zero)
            //{
            //    timer += Time.deltaTime;

            //}

            //Vector2 movement = _input.move.normalized * (10 * Time.deltaTime);
            //_controller.Move(new Vector3(movement.x, 0, movement.y));


            Vector3 inputDirection = Vector3.zero;
            Vector3 targetDirection = Vector3.zero;


            float targetSpeed = _input.move.magnitude * 10;

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;


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
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

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
                targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            }

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);




        }

        public override void OnEnter()
        {
            //timer = 0;
            _controller = _player.GetComponent<CharacterController>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        public override void OnExit()
        {

        }

    }

    public class PlayerFSMState_IDLE : PlayerFSMState
    {
        public PlayerFSMState_IDLE()
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

    public class PlayerFSMState_PARRY : PlayerFSMState
    {
        public float timer;
        private GameObject _playerParryBox;
        public PlayerFSMState_PARRY(GameObject playerParryBox)
        {
            _playerParryBox = playerParryBox;
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
            _playerParryBox.SetActive(false);
            _playerParryBox.GetComponent<MeshRenderer>().material.color = Color.white;
        }

    }

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

    public class PlayerFSMState_TAKEDAMAGE : PlayerFSMState
    {
        private Player _player;
        private bool _isDamaged;
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

        }

        public override void OnExit()
        {

        }

    }

    public class PlayerFSMState_DEATH : PlayerFSMState
    {
        public PlayerFSMState_DEATH()
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

    public class PlayerFSMState_DODGE : PlayerFSMState
    {
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

        private GameObject camera;

        public float timer;

        public PlayerFSMState_DODGE(Player player, PlayerInputsScript input, Animator animator)
        {
            _player = player;
            _input = input;
            _animator = animator;
        }

        public override void Tick()
        {
            //Vector2 movement = _input.move.normalized * (10 * Time.deltaTime);
            //_controller.Move(new Vector3(movement.x, 0, movement.y));

            timer += Time.deltaTime;

            float targetSpeed = _input.move.magnitude * 10;


            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

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

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(_player.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                // rotate to face input direction relative to camera position
                _player.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);




        }

        public override void OnEnter()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDDodge = Animator.StringToHash("Dodge");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _controller = _player.GetComponent<CharacterController>();
            camera = GameObject.FindGameObjectWithTag("MainCamera");
            timer = 0;

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
        }
    }
}
