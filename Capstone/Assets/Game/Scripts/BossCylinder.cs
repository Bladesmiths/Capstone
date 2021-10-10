using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class BossCylinder : MonoBehaviour
    {
        [SerializeField] private GameObject well;
        public int id = -1;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.RotateAround(well.transform.position, well.transform.up, 180 * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            // If the cylinder hits the block detector, then don't deal any damage
            if (other.gameObject.CompareTag("PreventDmg") == false)
            {
                // If it hits the player
                if (other.gameObject.transform.root.CompareTag("Player") == true)
                {
                    Player player = other.gameObject.transform.root.gameObject.GetComponent<Player>();
                    // Check if the player has already been hit by this object
                    if (player.damagingIds.Contains(id) == false)
                    {
                        // If not then take damage and add it's id to the list
                        player.TakeDamage(1);
                        player.damagingIds.Add(id);
                    }
                }
            }
        }
    }
}
