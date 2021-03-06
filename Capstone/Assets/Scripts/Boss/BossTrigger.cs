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
        public static BossTrigger instance;
        private bool fightingBoss = false;

        // Start is called before the first frame update
        void Start()
        {
            if (instance == null)
            {
                instance = this;
            }

            player = Player.instance;
            player.bossTrigger = this;

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
            if (other.name == player.name && !fightingBoss)
            {
                player.transform.Find("TargetLockManager").GetComponent<SphereCollider>().radius = 20;
                uiManager.ToggleBossHealthBar(true);
                fightingBoss = true;
            }
        }

        //Reset everything changed by entering the boss trigger
        //Occurs on player respawn
        public void BossTriggerReset()
        {
            player.transform.Find("TargetLockManager").GetComponent<SphereCollider>().radius = 8;

            //Only reset if boss was active
            if(fightingBoss)
            {
                uiManager.UpdateBossHealthBar(true);
                uiManager.ToggleBossHealthBar(false);

                fightingBoss = false;
            }
        }
    }
}
