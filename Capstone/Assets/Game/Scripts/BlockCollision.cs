using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
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
            if (other.transform.root.gameObject.tag == "Damaging")
            {
                Debug.Log("Block Triggered" + other.gameObject);
                gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                gameObject.transform.root.gameObject.GetComponent<Player>().isDamagable = false;
            }
        }
    }
}
