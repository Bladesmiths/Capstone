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
            Debug.Log("Parent Pos: " + center);

            for (int i = 0; i < bodies.Length; i++)
            {
                Vector3 dist = bodies[i].gameObject.transform.position - center;
                //dist = transform.rotation * dist;
                Debug.Log("RigidBody Pos: " + bodies[i].gameObject.transform.position);
                Debug.Log("Distance: " + dist.normalized);
                //rb.gameObject.transform.parent = null;
                bodies[i].AddForce(dist.normalized * 10, ForceMode.Impulse);

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
