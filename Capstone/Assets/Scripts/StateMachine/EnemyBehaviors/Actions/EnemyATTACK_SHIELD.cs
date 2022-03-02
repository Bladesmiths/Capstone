using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The behavior for how the Enemies should attack
    /// </summary>
    public class EnemyATTACK_SHIELD : Action
    {
        [SerializeField]
        private AnimationCurve curve;
        [SerializeField]
        private AnimationCurve shieldCurve;
        private GameObject _sword;
        private GameObject _shield;
        private Enemy_Shield _enemy;
        private bool attack;
        private float timer;
        private float timerMax;

        private float preAttackTimer;
        private float preAttackTimerMax;

        public EnemyATTACK_SHIELD(GameObject sword, Enemy enemy)
        {
            _sword = sword;
            //_enemy = enemy;
            preAttackTimer = 0f;
            preAttackTimerMax = 0.5f;
        }

        public EnemyATTACK_SHIELD()
        {

        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            float val = curve.Evaluate(timer);
            float shieldVal = shieldCurve.Evaluate(timer);
            _sword.transform.rotation = Quaternion.Euler(val, _sword.transform.eulerAngles.y, 0f);
            _shield.transform.localRotation = Quaternion.Euler(0f, shieldVal, 0f);

            //Debug.Log(timer);

            if(timer >= timerMax)
            {
                _enemy.CanHit = false;
                return TaskStatus.Success;
            }


            //if (attack)
            //{
            //    Attack();

            //    if (_sword.transform.localEulerAngles.x == 90)
            //    {
            //        attack = false;
            //    }
            //}
            //else
            //{
            //    StopAttack();

            //    if (_sword.transform.localEulerAngles.x <= 0.1)
            //    {
            //        _enemy.CanHit = false;
            //    }
            //}
            return TaskStatus.Running;
        }

        public override void OnStart()
        {
            _enemy = gameObject.GetComponent<Enemy_Shield>();
            _sword = _enemy.Sword;
            _shield = _enemy.shield;
            preAttackTimer = 0f;
            preAttackTimerMax = 0.5f;
            attack = true;
            _sword.GetComponent<BoxCollider>().enabled = true;
            timer = 0f;
            timerMax = 1f;
            _enemy.attackTimerMax = Random.Range(0.75f, 2f);
            _enemy.InCombat = true;
        }

        public override void OnEnd()
        {
            _sword.transform.rotation = Quaternion.Euler(0f, _sword.transform.eulerAngles.y, 0f);
            _shield.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            _enemy.attackTimer = _enemy.attackTimerMax;
            _enemy.ClearDamaging();
            _sword.GetComponent<BoxCollider>().enabled = false;
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
