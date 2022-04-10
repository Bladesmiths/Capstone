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
    public class EnemyDASHATTACK : Action
    {
        [SerializeField]
        private AnimationCurve curve;
        //private GameObject _sword;
        private Enemy _enemy;
        private bool attack;
        public float timer;
        public float timerMax;
        public float waitTimer;
        public float waitTimerMax;

        private float preAttackTimer;
        private float preAttackTimerMax;
        private NavMeshAgent agent;

        private Vector3 dist;
        private Vector3 playerPos;
        private bool dash;

        public EnemyDASHATTACK(GameObject sword, Enemy enemy)
        {
            //_sword = sword;
            _enemy = enemy;
            preAttackTimer = 0f;
            preAttackTimerMax = 0.5f;
        }

        public EnemyDASHATTACK()
        {

        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            float val = curve.Evaluate(timer);
            //_sword.transform.rotation = Quaternion.Euler(val, _sword.transform.eulerAngles.y, 0f);

            if(timer <= timerMax)
            {
                dist = Player.instance.transform.position - transform.position;
                agent.SetDestination(transform.position);
            }
            else
            {
                waitTimer += Time.deltaTime;
            }

            if(waitTimer >= waitTimerMax)
            {

            }


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
            //_sword = _enemy.Sword;
            //_sword = _enemy.Sword;
            //_sword.GetComponent<Sword>().damaging = true;
            preAttackTimer = 0f;
            preAttackTimerMax = 0.5f;
            attack = true;
            //_sword.GetComponent<BoxCollider>().enabled = true;
            timer = 0f;
            timerMax = 1f;
            _enemy.attackTimerMax = Random.Range(0.5f, 2f);
            _enemy.canMove = true;
            _enemy.InCombat = true;
            waitTimer = 0;
            waitTimerMax = 0.5f;
            dash = true;

            dist = Player.instance.transform.position - transform.position;
        }

        public override void OnEnd()
        {
            //_sword.transform.rotation = Quaternion.Euler(0f, _sword.transform.eulerAngles.y, 0f);
            _enemy.attackTimer = _enemy.attackTimerMax;
            _enemy.ClearDamaging();
            //_sword.GetComponent<BoxCollider>().enabled = false;
            _enemy.canMove = false;



        }
    }
}
