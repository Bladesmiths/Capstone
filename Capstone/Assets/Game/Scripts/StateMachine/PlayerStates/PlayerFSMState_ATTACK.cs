using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;

namespace Bladesmiths.Capstone
{
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

        private float timer;

        public float Timer { get { return timer; } }

        public PlayerFSMState_ATTACK(Player player, PlayerInputsScript input, Animator animator, GameObject sword)
        {
            _player = player;
            _input = input;
            _animator = animator;
            _sword = sword;
            _sword.GetComponent<Rigidbody>().detectCollisions = false;
            id = PlayerCondition.F_Attacking;
        }

        public override void Tick()
        {
            timer += Time.deltaTime;

            _sword.GetComponent<Rigidbody>().detectCollisions = true;
            //int layerMask = 1 << 8;
            if (_hasAnimator)
            {
                //_animator.SetBool(_animIDAttack, false);

            }

            //layerMask = ~layerMask;
            //RaycastHit hit;

            //if (Physics.Raycast(_player.transform.position + Vector3.up, _player.transform.TransformDirection(Vector3.forward), out hit, 2f))
            if (_sword.GetComponent<Rigidbody>().detectCollisions)
            {

                //Physics.Raycast(_player.transform.position + Vector3.up, _player.transform.TransformDirection(Vector3.forward), out hit, 2f);

                //Debug.DrawRay(_player.transform.position + Vector3.up, _player.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                //Debug.Log("Did Hit");

                //if (hit.collider.gameObject.GetComponent<Enemy>())
                //{
                //    hit.collider.gameObject.GetComponent<Enemy>().Damaged();

                //}


            }
            else
            {
                //Debug.DrawRay(_player.transform.position, _player.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                //Debug.Log("Did not Hit");

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
            timer = 0;
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

            _animator.SetBool(_animIDAttack, true); 
        }

        public override void OnExit()
        {
            _sword.GetComponent<Rigidbody>().detectCollisions = false;
            _animator.SetBool(_animIDAttack, false);
        }

    }

}
