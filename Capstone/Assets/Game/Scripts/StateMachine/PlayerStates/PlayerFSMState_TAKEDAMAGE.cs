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
        private Animator _animator;
        private int _animIDDamaged;

        public float Timer { get; set; }

        public PlayerFSMState_TAKEDAMAGE(Player player, Animator animator)
        {
            _player = player;
            id = PlayerCondition.F_TakingDamage;
            _animator = animator;

            // Assign damaged paramater id
            _animIDDamaged = Animator.StringToHash("Damaged");
        }

        public override void Tick()
        {
            Timer += Time.deltaTime;
        }

        public override void OnEnter()
        {
            _player.damaged = false;
            Timer = 0;
            _player.inState = true;

            _animator.SetTrigger(_animIDDamaged); 
        }

        public override void OnExit()
        {
            Timer = 0; 
            _player.inState = false;
        }

    }

}
