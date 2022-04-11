using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class PlayerShatterSword : MonoBehaviour
    {
        public Vector3 center;

        private void Start()
        {
            center = transform.position;
            Rigidbody[] bodies = transform.GetComponentsInChildren<Rigidbody>();

            for (int i = 0; i < bodies.Length; i++)
            {
                bodies[i].AddExplosionForce(5f, Player.instance.transform.position, 5f, 0.1f, ForceMode.Impulse);
            }
        }

        private void Update()
        {
            if(transform.childCount == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
