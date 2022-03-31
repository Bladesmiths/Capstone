using Bladesmiths.Capstone.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class BossTrigger : MonoBehaviour
    {
        private Player player;
        private UIManager uiManager;

        // Start is called before the first frame update
        void Start()
        {
            player = Player.instance;
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //The player has entered the boss arena
        //This is effectively the "start of boss fight" trigger
        private void OnTriggerEnter(Collider other)
        {
            player.transform.Find("TargetLockManager").GetComponent<SphereCollider>().radius = 20;
            uiManager.ToggleBossHealthBar(true);

        }
        private void OnTriggerExit(Collider other)
        {
            player.transform.Find("TargetLockManager").GetComponent<SphereCollider>().radius = 8;
            uiManager.ToggleBossHealthBar(false);
        }
    }
}
