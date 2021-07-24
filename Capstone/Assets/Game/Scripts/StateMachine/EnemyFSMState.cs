using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Base class for possible states
    /// </summary>
    public class EnemyFSMState : IState
    {
        protected EnemyCondition id;

        public EnemyCondition ID { get; set; }

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

    public class EnemyFSMState_MOVING : EnemyFSMState
    {
        public EnemyFSMState_MOVING()
        {

        }
        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

    }

    public class EnemyFSMState_IDLE : EnemyFSMState
    {
        public EnemyFSMState_IDLE()
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

    public class EnemyFSMState_SEEK : EnemyFSMState
    {
        private Player player;
        private Enemy enemy;

        public EnemyFSMState_SEEK(Player _player, Enemy _enemy)
        {
            player = _player;
            enemy = _enemy;
        }

        public override void Tick()
        {
            Vector3 dist = player.transform.position - enemy.transform.position;

            enemy.gameObject.transform.position += dist.normalized * Time.deltaTime;

        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

    }
}
