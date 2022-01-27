using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class LastBreakableBox : MonoBehaviour
    {
        public bool isBroken;
        public bool boxActive = false;
        private float fadeOutTimer;
        private float fadeOutLength;
        private float shrinkSpeed;

        [SerializeField]
        private Material originalCenter;
        [SerializeField]
        private Material originalOuter;
        [SerializeField]
        private Material newCenter;
        [SerializeField]
        private Material newOuter;

        // Start is called before the first frame update
        void Start()
        {
            isBroken = false;
            fadeOutTimer = 0;
            fadeOutLength = 10.5f;
            shrinkSpeed = 1.0f;

            foreach (MeshRenderer renderer in transform.GetComponentsInChildren<MeshRenderer>())
            {
                if(renderer.material.name == "Mat_Gem_Cluster_Center (Instance)")
                {
                    renderer.material = newCenter;

                }
                else if (renderer.material.name == "Mat_Gem_Cluster_Outer (Instance)")
                {
                    renderer.material = newOuter;

                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(AIDirector.Instance.enemyGroup.Count == 0)
            {
                //The box is now active and breakable
                boxActive = true;
                foreach (MeshRenderer renderer in transform.GetComponentsInChildren<MeshRenderer>())
                {
                    if (renderer.material.name == "Mat_Gem_Cluster_Center 1 (Instance)")
                    {
                        renderer.material = originalCenter;

                    }
                    else if (renderer.material.name == "Mat_Gem_Cluster_Outer 1 (Instance)")
                    {
                        renderer.material = originalOuter;

                    }
                }
            }

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
            if (collision.collider.gameObject.GetComponent<Sword>() && AIDirector.Instance.enemyGroup.Count == 0)
            {
                // Save that the box is broken
                isBroken = true;

                collision.transform.root.gameObject.GetComponent<Player>().AddPoints();
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
