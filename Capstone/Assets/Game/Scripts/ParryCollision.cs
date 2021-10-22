using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone 
{
    public class ParryCollision : MonoBehaviour
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
            if (other.gameObject.GetComponent<IDamaging>() != null && other.tag != "Player")
            {
                Debug.Log("Parry Triggered" + other.gameObject);
                //gameObject.GetComponent<MeshRenderer>().material.color = Color.red;

                gameObject.GetComponentInParent<Player>().parrySuccessful = true;
            }
        }
    }
}