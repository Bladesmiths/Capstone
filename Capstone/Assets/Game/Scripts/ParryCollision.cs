using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone 
{
    public class ParryCollision : MonoBehaviour
    {
        public Player Player { get; set; }

        public ObjectController ObjectController { get; set; }

        // Start is called before the first frame update
        public void Start()
        {
            ObjectController = GameObject.Find("ObjectController").GetComponent<ObjectController>();
        }

        // Update is called once per frame
        void Update() { }
        
        private void OnTriggerEnter(Collider other)
        {
            // Exits the method if the colliding object is not in the Parryable Layer
            if (LayerMask.LayerToName(other.gameObject.layer) != "Parryable")
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
                    !Player.DamagingObjectIDs.Contains(damagingObject.ID))
                {
                    Player.parrySuccessful = true;
                    // Adding the damaging ID 
                    Player.AddDamagingID(damagingObject.ID);

                    // Adding chip damage back to player health
                    Player.Health += Player.ChipDamageTotal;

                    // Resetting chip damage
                    // This should happen as soon as we exit the parry attempt state anyway
                    // But just to eliminate possible race conditions
                    Player.ResetChipDamage(); 
                }
            }
        }
    }
}