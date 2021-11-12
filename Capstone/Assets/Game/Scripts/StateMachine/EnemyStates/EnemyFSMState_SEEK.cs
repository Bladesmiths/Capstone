using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class EnemyFSMState_SEEK : EnemyFSMState
    {
        private Player _player;
        private Enemy _enemy;

        public EnemyFSMState_SEEK(Player player, Enemy enemy)
        {
            _player = player;
            _enemy = enemy;

        }

        public override void Tick()
        {
            Vector3 dist = _player.transform.position + Vector3.up - _enemy.transform.position;

            _enemy.gameObject.transform.position += dist.normalized * Time.deltaTime;



        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

    }

}