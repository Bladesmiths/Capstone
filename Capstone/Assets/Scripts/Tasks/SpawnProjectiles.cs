using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone.Testing
{
    public class SpawnProjectiles : Action
    {

        public SharedGameObject playerShared;
        private GameObject player;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private GameObject projectile;
        [SerializeField] private float maxLifeTime;

        [SerializeField] private List<GameObject> activeProjectiles;

        public override void OnStart()
        {
            player = playerShared.Value;
            activeProjectiles = new List<GameObject>();
        }

        public override TaskStatus OnUpdate()
        {
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
                spawnPos.y += 0.3f;
                direction = Quaternion.Euler(0, angle += 10 , 0) * direction;
            }

            gameObject.GetComponent<Boss>().activeProjectiles = activeProjectiles;

            return TaskStatus.Success;
        }
    }
}
