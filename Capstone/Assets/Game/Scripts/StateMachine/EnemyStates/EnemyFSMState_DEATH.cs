using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class EnemyFSMState_DEATH : EnemyFSMState
    {
        Enemy _enemy;
        private float fadeOutTimer = 0f;
        private float fadeOutLength = 1f;
        private float shrinkSpeed = 1.0f;

        public EnemyFSMState_DEATH(Enemy enemy)
        {
            _enemy = enemy;
        }

        public override void Tick()
        {
            fadeOutTimer += Time.deltaTime;
            _enemy.gameObject.GetComponent<CapsuleCollider>().enabled = false;

            // When the object should fade out
            if (fadeOutTimer >= fadeOutLength)
            {
                // Shrink the cubes
                _enemy.transform.localScale = new Vector3(
                    _enemy.transform.localScale.x - (shrinkSpeed * Time.deltaTime),
                    _enemy.transform.localScale.y - (shrinkSpeed * Time.deltaTime),
                    _enemy.transform.localScale.z - (shrinkSpeed * Time.deltaTime));

                // After the cubes are shrunk, destroy it
                if (_enemy.transform.localScale.x <= 0 &&
                    _enemy.transform.localScale.y <= 0 &&
                    _enemy.transform.localScale.z <= 0)
                {
                    MonoBehaviour.Destroy(_enemy.gameObject);
                }
            }

        }

        public override void OnEnter()
        {
            // When the enemy is dead destroy it
            AIDirector.Instance.RemoveFromGroups(_enemy);
            //MonoBehaviour.Destroy(_enemy.gameObject);
        }

        public override void OnExit()
        {

        }

    }
}
