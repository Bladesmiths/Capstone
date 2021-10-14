using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the player takes damage
    /// CURRENTLY NOT IN USE
    /// </summary>
    public class PlayerFSMState_TAKEDAMAGE : PlayerFSMState
    {
        private Player _player;
        public float timer;

        public PlayerFSMState_TAKEDAMAGE(Player player)
        {
            _player = player;
            id = PlayerCondition.F_TakingDamage;
        }

        public override void Tick()
        {
            timer += Time.deltaTime;

        }

        public override void OnEnter()
        {
            _player.damaged = false;
            timer = 0;
            //_player.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
            _player.inState = true;

        }

        public override void OnExit()
        {
            //_player.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;
            _player.inState = false;
        }

    }

}
