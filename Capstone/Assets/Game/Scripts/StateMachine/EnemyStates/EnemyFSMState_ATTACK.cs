using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The behavior for how the Enemies should attack
    /// </summary>
    public class EnemyFSMState_ATTACK : EnemyFSMState
    {
        private GameObject _sword;
        private Enemy _enemy;
        private bool attack;

        private float preAttackTimer;
        private float preAttackTimerMax;

        public EnemyFSMState_ATTACK(GameObject sword, Enemy enemy)
        {
            _sword = sword;
            _enemy = enemy;
            preAttackTimer = 0f;
            preAttackTimerMax = 0.5f;
        }

        public override void Tick()
        {
            if (attack)
            {
                Attack();

                if (_sword.transform.localEulerAngles.x == 90)
                {
                    attack = false;
                }
            }
            else
            {
                StopAttack();

                if (_sword.transform.localEulerAngles.x <= 0.1)
                {
                    _enemy.CanHit = false;
                }
            }
        }

        public override void OnEnter()
        {
            attack = true;
            preAttackTimer = 0f;
        }

        public override void OnExit()
        {
            _sword.transform.rotation = Quaternion.Euler(0f, _sword.transform.eulerAngles.y, 0f);
            _enemy.attackTimer = _enemy.attackTimerMax;
            _enemy.ClearDamaging();

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
