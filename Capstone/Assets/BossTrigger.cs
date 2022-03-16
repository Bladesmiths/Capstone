using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class BossTrigger : MonoBehaviour
    {
        private Player player;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            player.transform.Find("TargetLockManager").GetComponent<SphereCollider>().radius = 20; 
        }
        private void OnTriggerExit(Collider other)
        {
            player.transform.Find("TargetLockManager").GetComponent<SphereCollider>().radius = 8;
        }
    }
}
