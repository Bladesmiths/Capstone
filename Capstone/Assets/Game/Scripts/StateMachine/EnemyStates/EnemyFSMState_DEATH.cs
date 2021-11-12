using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class EnemyFSMState_DEATH : EnemyFSMState
    {
        Enemy _enemy;
        public EnemyFSMState_DEATH(Enemy enemy)
        {
            _enemy = enemy;
        }

        public override void Tick()
        {

        }

        public override void OnEnter()
        {
            // When the enemy is dead destroy it
            MonoBehaviour.Destroy(_enemy.gameObject);
        }

        public override void OnExit()
        {

        }

    }
}
