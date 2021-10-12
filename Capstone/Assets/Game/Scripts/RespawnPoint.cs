using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class RespawnPoint : MonoBehaviour
    {
        private Vector3 respawnPoint;
        private bool hasActivated;

        // Start is called before the first frame update
        void Start()
        {
            // Respawn point is the empty gameobject attached to this
            respawnPoint = transform.GetChild(0).position;
            hasActivated = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            // If the object collided with is the player
            if (other.transform.root.gameObject.CompareTag("Player"))
            {
                if (hasActivated == false)
                {
                    // Set the players respawn point to this if passing through it for the first time
                    other.transform.root.gameObject.GetComponent<Player>().RespawnPoint = respawnPoint;
                    hasActivated = true;
                }
            }
        }
    }
}
