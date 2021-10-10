using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class BlockCollision : MonoBehaviour
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
            if (other.transform.root.gameObject.tag == "Damaging")
            {
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
    }
}
