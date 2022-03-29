using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class RespawnPoint : MonoBehaviour
    {
        private Vector3 respawnPoint;
        private Vector3 respawnRotation;
        private bool hasActivated;

        private Player player;

        // Start is called before the first frame update
        void Start()
        {
            // Respawn point is the empty gameobject attached to this
            //respawnPoint = transform.GetChild(0).position;
            //respawnRotation = transform.GetChild(0).rotation.eulerAngles;
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
                    player = other.transform.root.gameObject.GetComponent<Player>();
                    // Set the players respawn point to this if passing through it for the first time
                    player.SetRespawn(transform.GetChild(0));
                    hasActivated = true;
                }
            }
        }
    }
}
