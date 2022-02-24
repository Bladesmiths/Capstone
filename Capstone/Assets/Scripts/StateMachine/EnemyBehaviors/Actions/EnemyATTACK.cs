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
    public class EnemyATTACK : Action
    {
        [SerializeField]
        private AnimationCurve curve;
        private GameObject _sword;
        private Enemy _enemy;
        private bool attack;
        private float timer;
        private float timerMax;

        private float preAttackTimer;
        private float preAttackTimerMax;
        private NavMeshAgent agent;

        public EnemyATTACK(GameObject sword, Enemy enemy)
        {
            _sword = sword;
            _enemy = enemy;
            preAttackTimer = 0f;
            preAttackTimerMax = 0.5f;
        }

        public EnemyATTACK()
        {

        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            float val = curve.Evaluate(timer);
            _sword.transform.rotation = Quaternion.Euler(val, _sword.transform.eulerAngles.y, 0f);
            //Debug.Log(timer);

            if(timer >= timerMax)
            {
                _enemy.CanHit = false;
                return TaskStatus.Success;
            }

            if (agent != null)
            {
                agent.SetDestination(transform.position);
            }

            Vector3 dist = Player.instance.transform.position - transform.position;

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
            _sword = _enemy.Sword;
            preAttackTimer = 0f;
            preAttackTimerMax = 0.5f;
            attack = true;
            _sword.GetComponent<BoxCollider>().enabled = true;
            timer = 0f;
            timerMax = 1f;
            _enemy.attackTimerMax = Random.Range(0.75f, 3f);
            _enemy.canMove = true;
        }

        public override void OnEnd()
        {
            _sword.transform.rotation = Quaternion.Euler(0f, _sword.transform.eulerAngles.y, 0f);
            _enemy.attackTimer = _enemy.attackTimerMax;
            _enemy.ClearDamaging();
            _sword.GetComponent<BoxCollider>().enabled = false;
            _enemy.canMove = false;

        }

        /// <summary>
        /// The method for the Enemy attacking the Player
        /// </summary>
        public void Attack()
        {
            preAttackTimer += Time.deltaTime;
            if (preAttackTimer <= preAttackTimerMax)
            {
                _sword.transform.rotation = Quaternion.Slerp(_sword.transform.rotation, Quaternion.Euler(-50f, _sword.transform.eulerAngles.y, 0f), 0.1f);
            }
            else
            {
                _sword.transform.rotation = Quaternion.Slerp(_sword.transform.rotation, Quaternion.Euler(90f, _sword.transform.eulerAngles.y, 0f), 0.6f);
            }
        }

        /// <summary>
        /// The method for resetting the Enemy's sword
        /// </summary>
        public void StopAttack()
        {
            _sword.transform.rotation = Quaternion.Slerp(_sword.transform.rotation, Quaternion.Euler(0f, _sword.transform.eulerAngles.y, 0f), 0.4f);

        }

    }
}
