using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class EnemyFSMState_SURROUND_BASIC : EnemyFSMState
    {
        private Player _player;
        private Enemy _enemy;
        private float _speed;
        private float surroundTimer;
        private float surroundTimerMax;

        public EnemyFSMState_SURROUND_BASIC(Player player, Enemy enemy)
        {
            _player = player;
            _enemy = enemy;
        }

        public override void Tick()
        {
            Debug.DrawRay(_player.transform.position + Vector3.up, _player.transform.right, Color.blue);
            Debug.DrawRay(_player.transform.position + Vector3.up, _player.transform.position + _player.transform.forward * 2f, Color.red);
            Debug.DrawRay(_enemy.transform.position + Vector3.up, _player.transform.position - _enemy.transform.position, Color.yellow);

            Debug.Log(Vector3.Distance(_enemy.transform.position, _player.transform.position + _player.transform.forward));

            if (Vector3.Distance(_enemy.transform.position, _player.transform.position + _player.transform.forward * 2f) >= 0.01f)
            {
                Vector3 dirToEnemy = _player.transform.position - _enemy.transform.position;

                float dir = Vector3.Dot(dirToEnemy, _player.transform.right) < 0 ? 1 : -1;

                

                _speed = _speed * Time.deltaTime * dir;
                _enemy.transform.RotateAround(_player.transform.position, Vector3.up, _speed);

            }

        }

        public override void OnEnter()
        {
            _speed = 20f;
        }

        public override void OnExit()
        {

        }

    }
}
