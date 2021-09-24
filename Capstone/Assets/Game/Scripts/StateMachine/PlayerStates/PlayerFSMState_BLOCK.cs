using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the Player is blocking enemy attacks
    /// </summary>
    public class PlayerFSMState_BLOCK : PlayerFSMState
    {
        private GameObject playerBlockBox;
        public PlayerFSMState_BLOCK(GameObject playerBlockDetector)
        {
            playerBlockBox = playerBlockDetector;
        }

        public override void Tick()
        {

        }

        public override void OnEnter()
        {
            // Turns the block detector box on
            playerBlockBox.SetActive(true);
        }

        public override void OnExit()
        {
            // Turns the block detector box off
            playerBlockBox.SetActive(false);

            // Change the color back to white
            playerBlockBox.GetComponent<MeshRenderer>().material.color = Color.white;
        }

    }

}
