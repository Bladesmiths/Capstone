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
        private bool sideMove;
        private float dir;

        public EnemyFSMState_SEEK(Player player, Enemy enemy)
        {
            _player = player;
            _enemy = enemy;
            sideMove = false;
        }

        public override void Tick()
        {
            MovementCheck();
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

        }

        public override void OnExit()
        {

        }
        
        /// <summary>
        /// Runs the movement code for seeking the player
        /// </summary>
        public void MovementCheck()
        {
            Vector3 movementVector = Vector3.zero;

            Vector3 dist = _player.transform.position - _enemy.transform.position;

            // If the Enemy is within 2 units and the seekAgainTimer isn't started
            if (dist.magnitude > 2f && seekAgainTimer == seekAgainTimerMax)
            {
                movementVector = dist.normalized;

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
                Vector3 move = Vector3.Cross(Vector3.up, dist) * dir;
                movementVector = move.normalized;

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

            movementVector.Normalize();

            // Move the Enemy
            controller.Move(movementVector * speed * Time.deltaTime);

            // Rotate the Enemy towards the Player
            Quaternion q = Quaternion.Slerp(_enemy.transform.rotation, Quaternion.LookRotation(dist, Vector3.up), 0.15f);
            q.eulerAngles = new Vector3(0, q.eulerAngles.y, 0);
            _enemy.transform.rotation = q;

        }

    }

}