using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

namespace Bladesmiths.Capstone.Testing
{
    public class SpawnProjectiles : Action
    {

        public SharedGameObject playerShared;
        private GameObject player;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private GameObject projectile;
        [SerializeField] private float maxLifeTime;

        [SerializeField] private GameObject sword;
        [SerializeField] private Transform aboveHeadSwordTransform;
        [SerializeField] private float nodeDuration;

        [SerializeField] private List<GameObject> activeProjectiles;

        private float timer;

        public override void OnStart()
        {
            timer = 0;
            player = playerShared.Value;
            activeProjectiles = new List<GameObject>();
        }

        public override TaskStatus OnUpdate()
        {
            if(timer < nodeDuration)
            {
                sword.transform.DOLocalMove(aboveHeadSwordTransform.localPosition, nodeDuration);

                timer += Time.deltaTime;

                return TaskStatus.Running;
            }

            Vector3 spawnPos = projectileSpawnPoint.position;
            Vector3 direction = (player.transform.position - spawnPos).normalized;
            float angle = 0;

            // Spawn i projectiles
            for (int i = 0; i < 36; i ++)
            {
                // Have the projectiles face the player
                GameObject tempProjectile = GameObject.Instantiate(projectile, spawnPos, Quaternion.Euler(0, 0, 0));

                tempProjectile.GetComponent<TestingProjectile>().Velocity = direction * 20;
                tempProjectile.GetComponent<TestingProjectile>().canMove = false;
                tempProjectile.GetComponent<TestingProjectile>().timeTillDestruction = maxLifeTime;
                activeProjectiles.Add(tempProjectile);

                gameObject.GetComponent<Boss>().ObjectController.AddIdentifiedObject(Enums.Team.Enemy, tempProjectile.GetComponent<TestingProjectile>());

                // Move next projectile up a bit and have it be facing 10 degrees to right of the last one
                spawnPos.y += 0.1f;
                spawnPos.z = projectileSpawnPoint.position.z + (0.4f * Mathf.Sin(angle));
                spawnPos.x = projectileSpawnPoint.position.x + (0.4f * Mathf.Cos(angle));
                direction = Quaternion.Euler(0, angle += 10 , 0) * direction;
            }

            gameObject.GetComponent<Boss>().activeProjectiles = activeProjectiles;

            return TaskStatus.Success;
        }
    }
}
