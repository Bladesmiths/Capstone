using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class EnemyFSMState_SEEK : EnemyFSMState
    {
        private Player _player;
        private Enemy _enemy;
        private float sideMoveTimer;
        private float sideMoveTimerMax;

        public EnemyFSMState_SEEK(Player player, Enemy enemy)
        {
            _player = player;
            _enemy = enemy;

        }

        public override void Tick()
        {
            Vector3 movementVector = Vector3.zero;

            Vector3 dist = _player.transform.position - _enemy.transform.position;


            if (dist.magnitude > 1)
            {
                movementVector += dist.normalized;
                sideMoveTimer = 0;
            }
            else
            {
                sideMoveTimer += Time.deltaTime;
            }

            if(sideMoveTimer >= sideMoveTimerMax)
            {
                Vector3 dir = Random.Range(-1, 1) < 0 ? Vector3.right * -1 : Vector3.right;

                Vector3 move = _enemy.transform.position + _enemy.transform.rotation * dir;
                movementVector += move.normalized;

            }


            _enemy.transform.position += movementVector * Time.deltaTime;
            _enemy.transform.LookAt(movementVector * Time.deltaTime);
        }

        public override void OnEnter()
        {
            sideMoveTimer = 0;
            sideMoveTimerMax = 1f;
        }

        public override void OnExit()
        {

        }

    }

}