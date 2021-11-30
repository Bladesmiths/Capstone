using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The Seeking behavior for the Enemy
    /// </summary>
    public class EnemyFSMState_SEEK : EnemyFSMState
    {
        private Player _player;
        private Enemy _enemy;
        private float sideMoveTimer;
        private float sideMoveTimerMax;
        private float seekAgainTimer;
        private float seekAgainTimerMax;
        private CharacterController controller;
        private float speed;
        private float dir;
        private float surroundDistance;

        public EnemyFSMState_SEEK(Player player, Enemy enemy)
        {
            _player = player;
            _enemy = enemy;
        }

        public override void Tick()
        {
            MovementCheck();

            


        }

        private void AttackTimer()
        {
            _enemy.attackTimer -= Time.deltaTime;
            if(_enemy.attackTimer <= 0)
            {
                AIDirector.Instance.PopulateAttackQueue(_enemy);
            }

        }

        public override void OnEnter()
        {
            sideMoveTimer = 0;
            sideMoveTimerMax = 1f;
            seekAgainTimerMax = 1f;
            seekAgainTimer = seekAgainTimerMax;
            controller = _enemy.GetComponent<CharacterController>();
            speed = 1.5f;
            dir = Random.Range(-1, 2);
            surroundDistance = 2.5f;
        }

        public override void OnExit()
        {
            _enemy.moveVector = _enemy.transform.position;
            _enemy.rotateVector = _player.transform.position - _enemy.transform.position;
        }
        
        /// <summary>
        /// Runs the movement code for seeking the player
        /// </summary>
        public void MovementCheck()
        {
            Vector3 movementVector = Vector3.zero;

            Vector3 dist = _player.transform.position - _enemy.transform.position;

            // If the Enemy is within X units and the seekAgainTimer isn't started
            if (dist.magnitude > surroundDistance && seekAgainTimer == seekAgainTimerMax)
            {
                //movementVector = dist.normalized;
                movementVector = _player.transform.position;
            }
            else
            {
                // Starts the first timer
                sideMoveTimer += Time.deltaTime;
            }

            // When the sideMoveTimer is finished 
            if(sideMoveTimer >= sideMoveTimerMax)
            {
                // Get the perpendicular vector to the distance between the Player and the Enemy
                //dir = dir == 0 ? 1 : -1;
                Vector3 move = Vector3.Cross(Vector3.up, dist) * dir;
                movementVector = move + _enemy.transform.position;
                AttackTimer();
                seekAgainTimer -= Time.deltaTime;
            }

            // When the seekAgainTimer is finished
            if(seekAgainTimer <= 0)
            {
                // Rest everything
                seekAgainTimer = seekAgainTimerMax;
                sideMoveTimer = 0;
                dir = Random.Range(-1, 2);

            }

            Debug.DrawLine(_enemy.transform.position, movementVector, Color.red);

            _enemy.moveVector = movementVector;
            _enemy.rotateVector = dist;

        }

    }

}