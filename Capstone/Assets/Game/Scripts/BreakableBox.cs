using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    public bool isBroken;
    private float fadeOutTimer;
    private float fadeOutLength;
    private float shrinkSpeed;

    // Start is called before the first frame update
    void Start()
    {
        isBroken = false;
        fadeOutTimer = 0;
        fadeOutLength = 20.5f;
        shrinkSpeed = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(isBroken)
        {
            fadeOutTimer += Time.deltaTime;
        }

        // When the object should fade out
        if(fadeOutTimer >= fadeOutLength)
        {
            // Shrink the cubes
            transform.localScale = new Vector3(transform.localScale.x - (shrinkSpeed * Time.deltaTime), transform.localScale.y - (shrinkSpeed * Time.deltaTime), transform.localScale.z - (shrinkSpeed * Time.deltaTime));
            // After the cubes are shrunk, destroy it
            if(transform.localScale.x <= 0 && transform.localScale.y <= 0 && transform.localScale.z <= 0)
            {
                Destroy(gameObject);
            }
            
        }
    }

    // If the box is hit by an attack
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.gameObject.CompareTag("Player"))
        {
            // Save that the box is broken
            isBroken = true;
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
