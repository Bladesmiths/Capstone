using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class HealthPickup : MonoBehaviour
    {
        [SerializeField]
        private float healthToAdd;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<Sword>())
            {
                collision.gameObject.GetComponentInParent<Player>().Health += healthToAdd; 
            }
        }
    }
}