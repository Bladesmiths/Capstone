using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public class SpawnEnemyProjectile : MonoBehaviour
    {
        public Vector3 velocity;
        [SerializeField] private float timeTillDestruction;

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
        }

        public void OnCollisionEnter(Collision collision)
        {
            Debug.Log(collision.gameObject.name);
        }
    }
}
