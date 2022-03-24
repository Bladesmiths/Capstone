using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bladesmiths.Capstone
{ 
    public class BreakableBox : MonoBehaviour
    {
        public bool isBroken = false;
        private float fadeOutTimer = 0;
        [SerializeField]
        private float fadeOutLength = 10.5f;
        private float shrinkSpeed = 1.0f;

        // Update is called once per frame
        void Update()
        {
            if (isBroken)
            {
                fadeOutTimer += Time.deltaTime;
            }

            // When the object should fade out
            if (fadeOutTimer >= fadeOutLength)
            {
                // Shrink the cubes
                transform.localScale = new Vector3(transform.localScale.x - (shrinkSpeed * Time.deltaTime), transform.localScale.y - (shrinkSpeed * Time.deltaTime), transform.localScale.z - (shrinkSpeed * Time.deltaTime));
                // After the cubes are shrunk, destroy it
                if (transform.localScale.x <= 0 && transform.localScale.y <= 0 && transform.localScale.z <= 0)
                {
                    Destroy(gameObject);
                }

            }
        }

        // If the box is hit by an attack
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.GetComponent<Sword>())
            {
                // Save that the box is broken
                isBroken = true;

                //collision.transform.root.gameObject.GetComponent<Player>().AddPoints();
                // Turn off the parent box collider
                GetComponent<BoxCollider>().enabled = false;
                // Loop through all the childen and enable their physics
                foreach (Rigidbody child in transform.GetComponentsInChildren<Rigidbody>())
                {
                    child.isKinematic = false;
                }
            }
        }
    }
}
