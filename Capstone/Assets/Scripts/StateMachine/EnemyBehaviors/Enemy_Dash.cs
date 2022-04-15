using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;
using UnityEngine.AI;


namespace Bladesmiths.Capstone
{    
    public class Enemy_Dash : StateMachineBehaviour
    {
        private Vector3 dist;
        private Vector3 distance;

        private NavMeshAgent agent;
        public Vector3 playerPos;
        public AnimationCurve curveZ;
        public AnimationCurve curveY;
        private float timer;
        private float timerMax;
        //private GameObject sword;
        private Enemy enemy;
        public float enemySpeed;
        public float enemyAccl;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy = animator.gameObject.GetComponent<Enemy>();
            enemy.headCollider.enabled = true;
            agent = animator.gameObject.GetComponent<NavMeshAgent>();
            //enemy.animator.SetTrigger("Dash");
            agent.speed = enemySpeed;
            agent.acceleration = enemyAccl;
            //playerPos = Player.instance.transform.position;// - transform.position;
            distance = Player.instance.transform.position - animator.transform.position;
            playerPos = (-distance.normalized * 1.5f) + Player.instance.transform.position;
            timerMax = 1f;
            timer = 0;
            //sword = enemy.Sword;
            //sword.GetComponent<BoxCollider>().enabled = true;
            enemy.attackTimerMax = Random.Range(0.5f, 2f);
            enemy.isAttacking = true;
            enemy.attackedYet = true;

        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateMove(animator, stateInfo, layerIndex);
            agent.SetDestination(playerPos);
            dist = playerPos - animator.transform.position;
        }

        //public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    base.OnStateUpdate(animator, stateInfo, layerIndex);
            
        //}

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.headCollider.enabled = false;
            agent.speed = 3.5f;
            agent.acceleration = 8f;
            //sword.GetComponent<BoxCollider>().enabled = false;
            enemy.isAttacking = false;

        }
    }








    
}
