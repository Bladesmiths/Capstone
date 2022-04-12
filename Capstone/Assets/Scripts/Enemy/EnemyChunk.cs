using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class EnemyChunk : MonoBehaviour
    {
        private float fadeOutTimer = 0f;
        private float fadeOutLength = 4f;
        public float shrinkSpeed = 20.0f;

        // Update is called once per frame
        void Update()
        {
            fadeOutTimer += Time.deltaTime;

            // When the object should fade out
            if (fadeOutTimer >= fadeOutLength)
            {
                // Shrink the cubes
                transform.localScale = new Vector3(
                    transform.localScale.x - (shrinkSpeed * Time.deltaTime),
                    transform.localScale.y - (shrinkSpeed * Time.deltaTime),
                    transform.localScale.z - (shrinkSpeed * Time.deltaTime));

                //Debug.Log(transform.localScale);

                // After the cubes are shrunk, destroy it
                if (transform.localScale.x <= 0 &&
                    transform.localScale.y <= 0 &&
                    transform.localScale.z <= 0)
                {
                    MonoBehaviour.Destroy(gameObject);
                }
            }



        }

        public void ApplyForce()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(Vector3.one, ForceMode.Impulse);

        }


    }
}
