using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone
{
    public class EnemySTUN : Action
    {
        private float stunTimer;
        public float stunTimerMax;
        private GameObject _sword;
        private Enemy _enemy;
        public AnimationCurve curveY;
        public AnimationCurve curveZ;


        public EnemySTUN(GameObject sword, Enemy enemy, Player player)
        {
            _sword = sword;
            _enemy = enemy;
            
        }

        public EnemySTUN()
        {
            
        }

        public override void OnStart()
        {
            _enemy = GetComponent<Enemy>();
            _sword = _enemy.Sword;
            _sword.GetComponent<BoxCollider>().enabled = true;
            _sword.GetComponent<BoxCollider>().isTrigger = false;
            _sword.GetComponent<Rigidbody>().isKinematic = false;

            stunTimer = 0;
            stunTimerMax = 1f;
            _enemy.stunned = false;
            //defaultSword = _sword.transform.eulerAngles.y;
        }

        public override TaskStatus OnUpdate()
        {
            stunTimer += Time.deltaTime;
            if(stunTimer >= stunTimerMax)
            {
                stunTimer = 0f;
                return TaskStatus.Success;
            }

            return TaskStatus.Running;

        }

        public override void OnEnd()
        {
            _sword.transform.localPosition = _enemy.defaultSwordPos;
            _sword.transform.localRotation = _enemy.swordRot;
            _sword.GetComponent<BoxCollider>().enabled = false;
            _sword.GetComponent<BoxCollider>().isTrigger = true;
            _sword.GetComponent<Rigidbody>().isKinematic = true;
            _enemy.blocked = false;
            _enemy.parried = false;
        }

    }
}
