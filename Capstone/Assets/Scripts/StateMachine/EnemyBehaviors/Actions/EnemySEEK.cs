using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The Seeking behavior for the Enemy
    /// </summary>
    public class EnemySEEK : Action
    {
        private Transform player;
        private Enemy _enemy;
        private float sideMoveTimer;
        private float sideMoveTimerMax;
        private float seekAgainTimer;
        private float seekAgainTimerMax;
        private CharacterController controller;
        private float speed;
        private float dir;
        private float surroundDistance;
        public Vector3 moveVec;

        public EnemySEEK(Player player, Enemy enemy)
        {
            _enemy = enemy;
        }
        public EnemySEEK()
        {
            
        }

        public override TaskStatus OnUpdate()
        {
            MovementCheck();
            return TaskStatus.Running;
        }

        private void AttackTimer()
        {
            _enemy.attackTimer -= Time.deltaTime;
            if(_enemy.attackTimer <= 0)
            {
                AIDirector.Instance.PopulateAttackQueue(_enemy);
            }

        }

        public override void OnStart()
        {
            sideMoveTimer = 0;
            sideMoveTimerMax = 1f;
            seekAgainTimerMax = 1f;
            seekAgainTimer = seekAgainTimerMax;
            speed = 1.5f;
            dir = Random.Range(-1, 2);
            surroundDistance = 2.5f;
            player = Player.instance.transform;
            _enemy = gameObject.GetComponent<Enemy>();
            controller = _enemy.GetComponent<CharacterController>();
            _enemy.InCombat = true;
            _enemy.canMove = true;

        }

        public override void OnEnd()
        {
            //_enemy.moveVector = transform.position;
            //_enemy.rotateVector = player.position - transform.position;
            _enemy.canMove = false;

        }

        /// <summary>
        /// Runs the movement code for seeking the player
        /// </summary>
        public void MovementCheck()
        {
            Vector3 movementVector = Vector3.zero;

            Vector3 dist = player.position - _enemy.transform.position;

            movementVector = player.position;

            // If the Enemy is within X units and the seekAgainTimer isn't started
            if (dist.magnitude > surroundDistance && seekAgainTimer == seekAgainTimerMax)
            {
                //movementVector = dist.normalized;
            }            

            Debug.DrawLine(_enemy.transform.position, movementVector, Color.red);
            moveVec = movementVector;
            _enemy.moveVector = movementVector;
            //_enemy.rotateVector = dist;

        }

    }

}