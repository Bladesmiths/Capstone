using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class EnemyFSMState_STUN : EnemyFSMState
    {
        private float stunTimer;
        private float stunTimerMax;
        private GameObject _sword;
        private Enemy _enemy;
        private Player _player;
        private float defaultSword;
        public bool continueAttacking;

        public EnemyFSMState_STUN(GameObject sword, Enemy enemy, Player player)
        {
            _sword = sword;
            _enemy = enemy;
            _player = player;
            defaultSword = sword.transform.localEulerAngles.y;
        }
        public override void OnEnter()
        {
            stunTimer = 0;
            stunTimerMax = 1f;
            continueAttacking = false;
            _enemy.stunned = false;
            defaultSword = _sword.transform.eulerAngles.y;
            _player.parrySuccessful = false;
        }

        public override void Tick()
        {
            //_sword.transform.rotation.SetFromToRotation(defaultSword, new Vector3(-25.419f, -18.094f, -32.499f));
            if (stunTimer <= stunTimerMax / 2f)
            {
                _sword.transform.rotation = Quaternion.Lerp(_sword.transform.rotation, Quaternion.Euler(-25.419f, _sword.transform.eulerAngles.y, -32.499f), 0.1f);
            }
            else
            {
                _sword.transform.rotation = Quaternion.Lerp(_sword.transform.rotation, Quaternion.Euler(0f, _sword.transform.eulerAngles.y, 0f), 0.8f);
            }

            stunTimer += Time.deltaTime;
            if(stunTimer >= stunTimerMax)
            {
                continueAttacking = true;
            }
        }

        public override void OnExit()
        {
            _sword.transform.rotation = Quaternion.Euler(0f, defaultSword, 0f);
        }

    }
}
