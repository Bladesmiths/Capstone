using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    private bool isBroken;
    private float fadeOutTimer;
    private float fadeOutLength;

    // Start is called before the first frame update
    void Start()
    {
        isBroken = false;
        fadeOutTimer = 0;
        fadeOutLength = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(isBroken)
        {
            fadeOutTimer += Time.deltaTime;
        }

        if(fadeOutTimer >= fadeOutLength)
        {
            Destroy(gameObject);
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
