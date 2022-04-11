using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone.Testing
{
    public class BossSpawnEnemyProjectiles : Action
    {
        public SharedGameObject playerShared;
        private GameObject player;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private Transform enemySpawnPoint;
        [SerializeField] private GameObject projectile;

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            for(int i = 0; i < 3; i ++)
            {
                GameObject tempProj = GameObject.Instantiate(projectile);
                tempProj.transform.forward = transform.forward;

                // Rotates the projectile so the 3 move out in a spread
                tempProj.transform.Rotate(0, i * 20, 0);
                // Set the velocity to go forward and down
                tempProj.GetComponent<SpawnEnemyProjectile>().velocity += tempProj.transform.forward * 7;
                tempProj.GetComponent<SpawnEnemyProjectile>().velocity += -tempProj.transform.up * 5;

                tempProj.transform.position = projectileSpawnPoint.position;

                // One of the 3 should be a health pickup
                if (i == 1)
                {
                    tempProj.GetComponent<SpawnEnemyProjectile>().isHealthPickup = true;
                }
            }
            return TaskStatus.Success;
        }
    }
}
