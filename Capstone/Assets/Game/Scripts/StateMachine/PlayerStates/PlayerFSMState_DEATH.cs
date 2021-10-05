using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the Player is dead
    /// </summary>
    public class PlayerFSMState_DEATH : PlayerFSMState
    {
        Player _player;
        public PlayerFSMState_DEATH(Player player)
        {
            _player = player;
            id = PlayerCondition.F_Dead;
        }

        public override void Tick()
        {

        }

        public override void OnEnter()
        {
            _player.inState = true;
        }

        public override void OnExit()
        {
            _player.inState = false;
        }

    }

}
