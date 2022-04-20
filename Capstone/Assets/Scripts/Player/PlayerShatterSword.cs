using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class PlayerShatterSword : MonoBehaviour
    {
        public Vector3 center;
        public Vector3 composite;

        private void Start()
        {
            center = transform.position;
            Rigidbody[] bodies = transform.GetComponentsInChildren<Rigidbody>();
            composite = Vector3.zero;

            foreach(Rigidbody rb in bodies)
            {
                composite += rb.gameObject.transform.position;
            }

            composite /= bodies.Length;

            composite = new Vector3(composite.x - Player.instance.transform.forward.x, composite.y - 0.02f, composite.z - Player.instance.transform.forward.z);

            //composite = Player.instance.transform.localRotation * composite;


            for (int i = 0; i < bodies.Length; i++)
            {
                Vector3 force = (bodies[i].gameObject.transform.position - composite).normalized;
                force = new Vector3(force.x + Random.Range(-3f, 6f), Mathf.Abs(force.y) * 15f, force.z + Random.Range(-3f, 3f));
                
                bodies[i].AddForceAtPosition(force, composite, ForceMode.Impulse);
            }
        }

        private void Update()
        {
            if(transform.childCount == 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(composite, 0.1f);
        }
    }
}
