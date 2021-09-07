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

    public class EnemyFSMState_DAMAGED : EnemyFSMState
    {
        public EnemyFSMState_DAMAGED()
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
}
