using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone 
{
    public class ParryCollision : MonoBehaviour
    {
        private List<int> blockedObjectIDs = new List<int>();

        public ObjectController ObjectController { get; set; }
        public float ChipDamageTotal { get; private set; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void BlockOccured(int id, float newChipDamageTotal)
        {
            blockedObjectIDs.Add(id);
            ChipDamageTotal = newChipDamageTotal; 
        }

        /// <summary>
        /// Removes an ID from the list of blocked IDs
        /// If there are no more blocked IDs, then block is no longer triggered
        /// </summary>
        /// <param name="blockedID"></param>
        public void RemoveBlockedID(int blockedID)
        {
            blockedObjectIDs.Remove(blockedID);
        }

        public void ResetChipDamage()
        {
            ChipDamageTotal = 0; 
        }

        private void OnTriggerEnter(Collider other)
        {
            // Exits the method if the colliding object is in Player or Default
            // This will probably need to be added to as we go on
            if ((LayerMask.GetMask("Player", "Default") & 1 << other.gameObject.layer) != 0)
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
                    Debug.Log("Parry Triggered" + other.gameObject);
                    //gameObject.GetComponent<MeshRenderer>().material.color = Color.red;

                    Player player = gameObject.GetComponentInParent<Player>();
                    player.parrySuccessful = true;

                    // Adding chip damage back to player health
                    player.Health += ChipDamageTotal;

                    // Resetting chip damage
                    // This should happen as soon as we exit the parry attempt state anyway
                    // But just to eliminate possible race conditions
                    ResetChipDamage(); 
                }
            }
        }
    }
}