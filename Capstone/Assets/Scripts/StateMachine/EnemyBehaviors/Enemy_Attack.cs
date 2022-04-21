using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;
using UnityEngine.AI;


namespace Bladesmiths.Capstone
{    
    public class Enemy_Attack : StateMachineBehaviour
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

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy = animator.gameObject.GetComponent<Enemy>();
            _enemy.axeCollider.enabled = true;

            agent = animator.gameObject.GetComponent<NavMeshAgent>();
            //_enemy.axeCollider.enabled = true;

            //_sword = _enemy.Sword;
            //_sword.GetComponent<Sword>().damaging = true;
            preAttackTimer = 0f;
            preAttackTimerMax = 0.5f;
            attack = true;
            //_sword.GetComponent<BoxCollider>().enabled = true;
            timer = 0f;
            timerMax = 1f;
            //_enemy.attackTimerMax = Random.Range(0.5f, 2f);
            _enemy.canMove = true;
            _enemy.InCombat = true;
            waitTimer = 0;
            waitTimerMax = 1f;
            _enemy.isAttacking = true;
            _enemy.attackedYet = true;

            dist = Player.instance.transform.position - animator.transform.position;
            playerPos = (dist * 0.1f) + animator.transform.position;
            _enemy.attackTimerMax = Random.Range(0.75f, 1.5f);

            if (animator.gameObject.GetComponent<Enemy_Ranged>() != null)
            {
                animator.gameObject.GetComponent<Enemy_Ranged>().FireProjectile();
            }


        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _enemy.axeCollider.enabled = false;
            _enemy.attackTimer = _enemy.attackTimerMax;
            _enemy.ClearDamaging();
            
            _enemy.canMove = false;
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateMove(animator, stateInfo, layerIndex);
            agent.SetDestination(playerPos);

            Vector3 lookRotVec = new Vector3(dist.x + 0.001f, 0f, dist.z);
            if (lookRotVec.magnitude > Mathf.Epsilon)
            {
                Quaternion q = Quaternion.Slerp(animator.transform.rotation, Quaternion.LookRotation(lookRotVec),
                    Time.deltaTime * 5f);
                animator.transform.rotation = q;
            }
        }
    }








    
}
