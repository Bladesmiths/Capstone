using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.tag == "Targettable")
        {
            Debug.Log("Block Triggered" + other.gameObject);
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
}
