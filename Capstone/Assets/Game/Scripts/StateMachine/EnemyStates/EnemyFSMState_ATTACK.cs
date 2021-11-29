using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class EnemyFSMState_ATTACK : EnemyFSMState
    {
        private GameObject _sword;
        private Enemy _enemy;
        private bool attack;

        public EnemyFSMState_ATTACK(GameObject sword, Enemy enemy)
        {
            _sword = sword;
            _enemy = enemy;
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
        }

        public override void OnExit()
        {
            _sword.transform.rotation = Quaternion.Euler(0f, _sword.transform.eulerAngles.y, 0f);
            _enemy.attackTimer = _enemy.attackTimerMax;
        }

        public void Attack()
        {
            _sword.transform.rotation = Quaternion.Slerp(_sword.transform.rotation, Quaternion.Euler(90f, _sword.transform.eulerAngles.y, 0f), 0.4f);

        }

        public void StopAttack()
        {
            _sword.transform.rotation = Quaternion.Slerp(_sword.transform.rotation, Quaternion.Euler(0f, _sword.transform.eulerAngles.y, 0f), 0.4f);

        }

    }
}
