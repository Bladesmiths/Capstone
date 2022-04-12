using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public class SpawnEnemyProjectile : MonoBehaviour
    {
        public Vector3 velocity;
        public bool isHealthPickup;
        [SerializeField] private float timeTillDestruction;

        [SerializeField] private GameObject healthPickup;
        [SerializeField] private GameObject enemy;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Util.DestroyTimer(timeTillDestruction, gameObject));
        }

        // Update is called once per frame
        void Update()
        {
            if (velocity != Vector3.zero)
            {
                transform.position = transform.position + velocity * Time.deltaTime;
            }

            if (transform.position.y <= 0)
            {
                if (isHealthPickup)
                    SpawnHealthPickup();
                else
                    SpawnEnemy();
                Destroy(gameObject);
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            Debug.Log(collision.gameObject.name);
        }

        private void SpawnEnemy()
        {
            Instantiate(enemy, transform.position, transform.rotation);
        }

        private void SpawnHealthPickup()
        {
            Vector3 position = transform.position;
            position.y = 1;
            Instantiate(healthPickup, position, transform.rotation);
        }
    }
}
