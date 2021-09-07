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
        StarterAssetsInputs _inputs;

        public PlayerFSMState_MOVING(StarterAssetsInputs inputs)
        {
            _inputs = inputs;
        }

        public override void Tick()
        {
            if(_inputs.move == Vector2.zero)
            {
                timer += Time.deltaTime;

            }
        }

        public override void OnEnter()
        {
            timer = 0;
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

        public PlayerFSMState_DODGE(Player player, PlayerInputsScript input, Animator animator)
        {
            _player = player;
            _input = input;
            _animator = animator;
        }

        public override void Tick()
        {
            
        }
        
        public override void OnEnter()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDDodge = Animator.StringToHash("Dodge");
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
        }
    }
}
