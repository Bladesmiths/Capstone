using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
            Vector3 dirToEnemy = _player.transform.position - _enemy.transform.position;

            //Debug.Log(Vector3.Distance(_enemy.transform.position, _player.transform.position + _player.transform.rotation * Vector3.forward));//_player.transform.forward * Vector3.Dot(dirToEnemy, _player.transform.forward)));
            //Debug.DrawLine(_enemy.transform.position, _player.transform.position + _player.transform.rotation * Vector3.forward);// _player.transform.forward * Vector3.Dot(dirToEnemy, _player.transform.forward));

            //Debug.Log(_player.transform.forward * Vector3.Dot(dirToEnemy, _player.transform.forward));

            if (Vector3.Distance(_enemy.transform.position, _player.transform.rotation * Vector3.forward) >= 1f)
            {
                float dir = Vector3.Dot(dirToEnemy, _player.transform.right) < 0 ? -1 : 1;
                _speed = 20 * Time.deltaTime * dir;

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
