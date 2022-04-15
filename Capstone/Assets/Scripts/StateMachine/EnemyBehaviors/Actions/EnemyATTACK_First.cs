using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The behavior for how the Enemies should attack
    /// </summary>
    public class EnemyATTACK_First : Action
    {        
        private Enemy _enemy;
        private bool attack;
        public float timer;
        public float timerMax;       
        private NavMeshAgent agent;

        private Vector3 dist;
        private Vector3 playerPos;

        public EnemyATTACK_First(GameObject sword, Enemy enemy)
        {
            //_sword = sword;
            _enemy = enemy;
            
        }

        public EnemyATTACK_First()
        {

        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            //_sword.transform.rotation = Quaternion.Euler(val, _sword.transform.eulerAngles.y, 0f);
            //Debug.Log(timer);

            if(timer >= timerMax)
            {
                return TaskStatus.Success;
            }

            if (agent != null)
            {
                agent.SetDestination(playerPos);
            }


            //dist.Normalize();
            Vector3 lookRotVec = new Vector3(dist.x + 0.001f, 0f, dist.z);
            if (lookRotVec.magnitude > Mathf.Epsilon)
            {
                Quaternion q = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotVec),
                    Time.deltaTime * 5f);
                transform.rotation = q;
            }

            return TaskStatus.Running;
        }

        public override void OnStart()
        {
            _enemy = gameObject.GetComponent<Enemy>();
            agent = GetComponent<NavMeshAgent>();
            _enemy.animator.SetTrigger("Attack");
           
            attack = true;
            timer = 0f;
            timerMax = 1f;
            _enemy.canMove = true;
            _enemy.InCombat = true;            
            _enemy.isAttacking = true;
            _enemy.attackedYet = true;

            dist = Player.instance.transform.position - transform.position;
            playerPos = (dist * 0.1f) + transform.position;

        }

        public override void OnEnd()
        {
            _enemy.attackTimer = _enemy.attackTimerMax;
            _enemy.ClearDamaging();
            _enemy.canMove = false;

        }
    }
}
