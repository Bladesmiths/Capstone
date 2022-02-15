using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone.Testing
{
    public class SpawnProjectiles : Action
    {
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private GameObject projectile;
        [SerializeField] private float maxLifeTime;

        [SerializeField] private List<GameObject> activeProjectiles;
        private float timer;


        public override void OnStart()
        {
            timer = 0;
            activeProjectiles = new List<GameObject>();
        }

        public override TaskStatus OnUpdate()
        {
            if (timer == 0)
            {
                projectile.GetComponent<TestingProjectile>().Velocity = new Vector3(1000, 0, 0);
                activeProjectiles.Add(GameObject.Instantiate(projectile, projectileSpawnPoint.position, Quaternion.Euler(0, 0, 0)));
                activeProjectiles.Add(GameObject.Instantiate(projectile, projectileSpawnPoint.position, Quaternion.Euler(0, 0, 0)));
                activeProjectiles.Add(GameObject.Instantiate(projectile, projectileSpawnPoint.position, Quaternion.Euler(0, 0, 0)));
            }

            timer += Time.deltaTime;

            if (timer < maxLifeTime)
            {
                return TaskStatus.Running;
            }
            else
            {
                for (int i = 0; i < activeProjectiles.Count; i++)
                {
                    GameObject.Destroy(activeProjectiles[i]);
                }
                activeProjectiles.Clear();
                return TaskStatus.Success;
            }
        }
    }
}
