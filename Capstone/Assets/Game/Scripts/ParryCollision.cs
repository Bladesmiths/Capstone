using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone 
{
    public class ParryCollision : MonoBehaviour
    {
        private Player player;

        public ObjectController ObjectController { get; set; }
        public float ChipDamageTotal { get; set; }

        // Start is called before the first frame update
        public void Start()
        {
            ObjectController = GameObject.Find("ObjectController").GetComponent<ObjectController>();
            player = gameObject.transform.root.gameObject.GetComponent<Player>();
        }

        // Update is called once per frame
        void Update() { }

        /// <summary>
        /// Method hooked to block event that updates fields when a block occurs
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newChipDamageTotal"></param>
        public void BlockOccured(float newChipDamageTotal)
        {
            ChipDamageTotal = newChipDamageTotal;
            player.ResetProvisionalDamageTimers();
        }

        /// <summary>
        /// Resets the chip damage field
        /// </summary>
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
                    !player.DamagingObjectIDs.Contains(damagingObject.ID))
                {
                    //gameObject.GetComponent<MeshRenderer>().material.color = Color.red;

                    player.parrySuccessful = true;

                    // Adding the damaging ID 
                    player.AddDamagingID(damagingObject.ID); 

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