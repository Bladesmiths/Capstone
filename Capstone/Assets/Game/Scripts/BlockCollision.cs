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
        public float ChipDamageTotal { get; private set; }


        public delegate void OnBlockDelegate(int id, float chipDamageTotal);
        public delegate void OnRemoveBlockIDDelegate(int id); 

        // Event declaration
        public event OnBlockDelegate OnBlock;
        public event OnRemoveBlockIDDelegate OnRemoveBlockID; 

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

            OnRemoveBlockID(blockedID); 
        }

        public void ResetChipDamage()
        {
            ChipDamageTotal = 0;
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
                if (ObjectController[damagingObject.ID].ObjectTeam != Enums.Team.Player && 
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
                    ChipDamageTotal += blockedDamage;
                    player.TakeDamage(damagingObject.ID, blockedDamage);

                    OnBlock(damagingObject.ID, ChipDamageTotal); 

                    // Debug stuff
                    Debug.Log($"Block Triggered by: {other.gameObject}");
                }
            }
        }
    }
}