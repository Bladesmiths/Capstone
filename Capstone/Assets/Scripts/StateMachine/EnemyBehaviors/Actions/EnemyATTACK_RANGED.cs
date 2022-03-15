using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using Bladesmiths.Capstone.Testing;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The behavior for how the Enemies should attack
    /// </summary>
    public class EnemyATTACK_RANGED : Action
    {
        [SerializeField]
        private AnimationCurve curve;
        private GameObject _sword;
        private Enemy_Ranged _enemy;
        private bool attack;
        private float timer;
        private float timerMax;

        private float preAttackTimer;
        private float preAttackTimerMax;
        private Vector3 projectileVelocity;

        public float fireTimerLimit;
        public float fireTimer;
        private float speed;

        public EnemyATTACK_RANGED(GameObject sword, Enemy_Ranged enemy)
        {
            _sword = sword;
            _enemy = enemy;
            preAttackTimer = 0f;
            preAttackTimerMax = 0.5f;
        }

        public EnemyATTACK_RANGED()
        {

        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            float val = curve.Evaluate(timer);
            //_sword.transform.rotation = Quaternion.Euler(val, _sword.transform.eulerAngles.y, 0f);
            //Debug.Log(timer);
            CheckIfInRange();

            if (timer >= timerMax)
            {
                _enemy.CanHit = false;
                return TaskStatus.Success;
            }


            return TaskStatus.Running;
        }

        public override void OnStart()
        {
            _enemy = gameObject.GetComponent<Enemy_Ranged>();
            _sword = _enemy.Sword;
            speed = 6f;
            preAttackTimer = 0f;
            preAttackTimerMax = 0.5f;
            fireTimerLimit = 2f;
            fireTimer = fireTimerLimit;
            attack = true;
            _sword.GetComponent<BoxCollider>().enabled = true;
            timer = 0f;
            timerMax = 1f;
            _enemy.attackTimerMax = Random.Range(0.75f, 2f);
            projectileVelocity = Vector3.forward * speed;
        }

        public override void OnEnd()
        {
            _sword.transform.rotation = Quaternion.Euler(0f, _sword.transform.eulerAngles.y, 0f);
            _enemy.attackTimer = _enemy.attackTimerMax;
            _enemy.ClearDamaging();
            _sword.GetComponent<BoxCollider>().enabled = false;
        }

        public void CheckIfInRange()
        {
            RaycastHit hit;

            if (Physics.Raycast(_enemy.shootLoc.position, transform.forward, out hit, 10f, _enemy.PlayerLayer))
            {
                timer += Time.deltaTime;
                if (fireTimer >= fireTimerLimit)
                {
                    FireProjectile();
                    fireTimer = 0;
                }

            }

        }

        private void FireProjectile()
        {
            GameObject newProjectile = MonoBehaviour.Instantiate(_enemy.projectilePrefab, _enemy.shootLoc.position, Quaternion.identity);
            newProjectile.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            TestingProjectile projectileComponent = newProjectile.GetComponent<TestingProjectile>();
            projectileComponent.Velocity = Quaternion.Euler(0, transform.eulerAngles.y, 0) * projectileVelocity;

            _enemy.ObjectController.AddIdentifiedObject(Enums.Team.Enemy, projectileComponent);
        }



    }
}
