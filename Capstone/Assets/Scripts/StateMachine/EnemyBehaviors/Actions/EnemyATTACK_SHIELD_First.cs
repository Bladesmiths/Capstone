using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The behavior for how the Enemies should attack
    /// </summary>
    public class EnemyATTACK_SHIELD_First : Action
    {
        [SerializeField]
        private AnimationCurve shieldCurve;
        //private GameObject _sword;
        //private GameObject _shield;
        private Enemy_Shield _enemy;
        private bool attack;
        private float timer;
        private float timerMax;
        private NavMeshAgent agent;

        private float preAttackTimer;
        private float preAttackTimerMax;

        private Vector3 dist;
        private Vector3 playerPos;

        public EnemyATTACK_SHIELD_First(GameObject sword, Enemy enemy)
        {
            
            preAttackTimer = 0f;
            preAttackTimerMax = 0.5f;
        }

        public EnemyATTACK_SHIELD_First()
        {

        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            //float shieldVal = shieldCurve.Evaluate(timer);
            //_sword.transform.rotation = Quaternion.Euler(val, _sword.transform.eulerAngles.y, 0f);
            //_shield.transform.localRotation = Quaternion.Euler(0f, shieldVal, 0f);

            //Debug.Log(timer);

            if(timer >= timerMax)
            {
                //_enemy.CanHit = false;
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
            _enemy = gameObject.GetComponent<Enemy_Shield>();
            //_sword = _enemy.Sword;
            agent = GetComponent<NavMeshAgent>();
            //_shield = _enemy.shield;
            _enemy.animator.SetTrigger("Attack");

            attack = true;
            timer = 0f;
            timerMax = 1f;
            _enemy.attackTimerMax = Random.Range(0.75f, 2f);
            _enemy.InCombat = true;
            _enemy.isAttacking = true;
            _enemy.attackedYet = true;

            dist = Player.instance.transform.position - transform.position;
            playerPos = (dist * 0.1f) + transform.position;
        }

        public override void OnEnd()
        {
            //_sword.transform.rotation = Quaternion.Euler(0f, _sword.transform.eulerAngles.y, 0f);
            //_shield.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            _enemy.attackTimer = _enemy.attackTimerMax;
            _enemy.ClearDamaging();
            _enemy.canMove = false;
            //_sword.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
