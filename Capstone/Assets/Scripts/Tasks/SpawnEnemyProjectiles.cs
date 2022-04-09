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
                tempProj.GetComponent<SpawnEnemyProjectile>().velocity = (enemySpawnPoint.position - projectileSpawnPoint.position).normalized * 5;
                tempProj.transform.position = projectileSpawnPoint.position;
                //tempProj.transform.position = new Vector3(tempProj.transform.forward.x + (i * 5), tempProj.transform.forward.y, tempProj.transform.forward.z);
                //new Vector3(projectileSpawnPoint.position.x + (i * 5), projectileSpawnPoint.position.y,projectileSpawnPoint.position.z);

            }

            return TaskStatus.Success;
        }
    }
}
