using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class BlockCollision : MonoBehaviour
    {
        private List<int> blockedObjectIDs = new List<int>();

        public ObjectController ObjectController { get; set; }
        public bool BlockTriggered { get; private set; }
        public bool Active { get; set; }
        public float ChipDamagePercentage { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            ObjectController = GameObject.Find("ObjectController").GetComponent<ObjectController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Removes an ID from the list of blocked IDs
        /// If there are no more blocked IDs, then block is no longer triggered
        /// </summary>
        /// <param name="blockedID"></param>
        public void RemoveBlockedID(int blockedID)
        {
            blockedObjectIDs.Remove(blockedID);

            if (blockedObjectIDs.Count == 0)
            {
                BlockTriggered = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Exits the method if the colliding object is in Player or Default
            // This will probably need to be added to as we go on
            if (!Active || (LayerMask.GetMask("Player", "Default") & 1 << other.gameObject.layer) != 0)
            {
                return;
            }

            // Retrieves the IDamaging Object
            IDamaging damagingObject = other.GetComponent<IDamaging>();

            // If there is a damaging object
            if (damagingObject != null)
            {
                // If the damaging object is not on the same team as the player
                // And its ID has not already been blocked
                if (ObjectController.DamagingObjects[damagingObject.ID].ObjectTeam != Enums.Team.Player && 
                    !blockedObjectIDs.Contains(damagingObject.ID))
                {
                    // Block has been triggered
                    BlockTriggered = true;

                    // Add the ID of the damaging object to the blocked ID list
                    // And subscribe to the DamagingFinished event
                    blockedObjectIDs.Add(damagingObject.ID);
                    damagingObject.DamagingFinished += RemoveBlockedID;

                    // Get a reference to the player
                    Player player = gameObject.transform.root.gameObject.GetComponent<Player>();

                    // Calculate the chip damage and make the Player take that damage
                    float blockedDamage = damagingObject.Damage * ChipDamagePercentage;
                    player.TakeDamage(damagingObject.ID, blockedDamage);

                    // Debug stuff
                    Debug.Log($"Block Triggered by: {other.gameObject}");
                }



                
                Player player = gameObject.transform.root.gameObject.GetComponent<Player>();

                // For now the boss cylinder is hard coded to add it's id to the list. In the future replace with an interface
                if (other.transform.root.gameObject.name == "BossCylinder")
                {
                    //Debug.Log("blocked");
                    if(player.damagingIds.Contains(other.transform.root.gameObject.GetComponent<BossCylinder>().id) == false)
                    {
                        player.damagingIds.Add(other.transform.root.gameObject.GetComponent<BossCylinder>().id);
                    }
                }
                //Debug.Log("Block Triggered" + other.gameObject);
                gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.transform.root.gameObject.tag == "Damaging")
        //     {
        //         Debug.Log("Block Triggered" + other.gameObject);
        //         gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        //         gameObject.transform.root.gameObject.GetComponent<Player>().isDamagable = false;
        //     }
        // }
    }
}