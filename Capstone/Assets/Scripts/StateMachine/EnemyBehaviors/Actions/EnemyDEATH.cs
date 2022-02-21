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
            //fadeOutTimer += Time.deltaTime;
            

            //// When the object should fade out
            //if (fadeOutTimer >= fadeOutLength)
            //{
            //    // Shrink the cubes
            //    _enemy.transform.localScale = new Vector3(
            //        _enemy.transform.localScale.x - (shrinkSpeed * Time.deltaTime),
            //        _enemy.transform.localScale.y - (shrinkSpeed * Time.deltaTime),
            //        _enemy.transform.localScale.z - (shrinkSpeed * Time.deltaTime));

            //    // After the cubes are shrunk, destroy it
            //    if (_enemy.transform.localScale.x <= 0 &&
            //        _enemy.transform.localScale.y <= 0 &&
            //        _enemy.transform.localScale.z <= 0)
            //    {
            //        MonoBehaviour.Destroy(_enemy.gameObject);
            //        return TaskStatus.Success;
            //    }
            //}
            return TaskStatus.Running;

        }

        public override void OnStart()
        {
            _enemy = GetComponent<Enemy>();
            // Removes the Enemy from its group and the attack queue
            AIDirector.Instance.RemoveFromGroups(_enemy);
            
            GameObject body = _enemy.bodyChunks;

            // Allows for the destruction of Enemy's
            _enemy.gameObject.GetComponent<CapsuleCollider>().enabled = false;

            int count = body.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                body.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
                body.transform.GetChild(0).gameObject.AddComponent<Rigidbody>();
                body.transform.GetChild(0).gameObject.AddComponent<EnemyChunk>().shrinkSpeed = 40f;
                body.transform.GetChild(0).parent = null;
            }

            count = _enemy.spine.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                _enemy.spine.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
                _enemy.spine.transform.GetChild(0).gameObject.AddComponent<Rigidbody>();
                _enemy.spine.transform.GetChild(0).gameObject.AddComponent<EnemyChunk>().shrinkSpeed = 40f;
                _enemy.spine.transform.GetChild(0).parent = null;
            }
                        
            count = _enemy.geo.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                GameObject gO = _enemy.geo.transform.GetChild(0).gameObject;
                gO.AddComponent<BoxCollider>();
                gO.AddComponent<Rigidbody>();
                gO.AddComponent<EnemyChunk>().shrinkSpeed = 1f;

                gO.transform.parent = null;

            }

            _enemy.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
            _enemy.transform.GetChild(0).gameObject.AddComponent<Rigidbody>();
            _enemy.transform.GetChild(0).gameObject.AddComponent<EnemyChunk>().shrinkSpeed = 1f;

            _enemy.transform.GetChild(1).gameObject.GetComponent<BoxCollider>().enabled = true;
            _enemy.transform.GetChild(1).gameObject.GetComponent<BoxCollider>().isTrigger = false;
            _enemy.transform.GetChild(1).gameObject.AddComponent<EnemyChunk>().shrinkSpeed = 1f;
            _enemy.transform.GetChild(1).gameObject.GetComponent<Rigidbody>().isKinematic = false;
            _enemy.transform.GetChild(1).parent = null;


            _enemy.gameObject.AddComponent<EnemyChunk>().shrinkSpeed = 2f;
        }

        public override void OnEnd()
        {

        }

    }
}
