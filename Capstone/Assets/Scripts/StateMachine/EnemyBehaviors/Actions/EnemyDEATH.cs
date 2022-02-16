using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The behavior for when the Enemies die
    /// </summary>
    public class EnemyDEATH : Action
    {
        private Enemy _enemy;
        private float fadeOutTimer = 0f;
        private float fadeOutLength = 4f;
        private float shrinkSpeed = 1.0f;

        public EnemyDEATH(Enemy enemy)
        {
            _enemy = enemy;
        }

        public EnemyDEATH()
        {

        }

        public override TaskStatus OnUpdate()
        {
            fadeOutTimer += Time.deltaTime;
            

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
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Running;

        }

        public override void OnStart()
        {
            _enemy = GetComponent<Enemy>();
            // Removes the Enemy from its group and the attack queue
            AIDirector.Instance.RemoveFromGroups(_enemy);

            // Allows for the destruction of Enemy's
            _enemy.gameObject.GetComponent<CapsuleCollider>().enabled = false;

            for(int i = 0; i < _enemy.transform.GetChild(1).childCount; i++)
            {
                _enemy.transform.GetChild(1).GetChild(i).gameObject.AddComponent<BoxCollider>();
                _enemy.transform.GetChild(1).GetChild(i).gameObject.AddComponent<Rigidbody>();
                _enemy.transform.GetChild(1).GetChild(i).GetComponent<Rigidbody>().isKinematic = false;
            }

            _enemy.transform.GetChild(2).GetComponent<Rigidbody>().isKinematic = false;
            _enemy.transform.GetChild(3).GetComponent<Rigidbody>().isKinematic = false;
            _enemy.transform.GetChild(4).GetComponent<Rigidbody>().isKinematic = false;
            // Turns on all of the physics on the Enemy
            //foreach (Rigidbody child in _enemy.transform.GetComponentsInChildren<Rigidbody>())
            //{
            //    child.isKinematic = false;
            //}
        }

        public override void OnEnd()
        {

        }

    }
}
