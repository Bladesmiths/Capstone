using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Bladesmiths.Capstone
{
    public class ResetEnemySword : Action
    {
        private Vector3 dist;
        private NavMeshAgent agent;

        private float timer;
        private float timerMax;
        private GameObject sword;
        private Enemy enemy;


        public ResetEnemySword()
        {

        }

        public override void OnStart()
        {
            agent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<Enemy>(); 
            timerMax = 1f;
            timer = 0;
            sword = enemy.Sword;

        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
                    
            sword.transform.rotation = enemy.swordRot;
            return TaskStatus.Success;
            
        }

        public override void OnEnd()
        {
            
        }

    }
}