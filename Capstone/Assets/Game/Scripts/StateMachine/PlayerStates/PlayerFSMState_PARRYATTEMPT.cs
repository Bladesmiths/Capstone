using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// The state for when the Player is parrying enemy attacks
    /// </summary>
    public class PlayerFSMState_PARRYATTEMPT : PlayerFSMState
    {
        // MOVE TO A BETTER PLACE FOR BALANCING;
        private float parryLength = 0.5f;

        public float timer;
        private GameObject _playerParryBox;
        private PlayerInputsScript _input;
        private Player _player;
        private GameObject _sword;
        public PlayerFSMState_PARRYATTEMPT(GameObject playerParryBox, PlayerInputsScript input, Player player)
        {
            _playerParryBox = playerParryBox;
            _input = input;
            _player = player;
            id = PlayerCondition.F_ParryAttempt;
            _sword = _player.transform.GetComponentInChildren<Sword>().gameObject;
        }

        public override void Tick()
        {
            timer += Time.deltaTime;

            if (timer >= parryLength)
            {
                _player.parryEnd = true;

            }
        }

        public override void OnEnter()
        {
            timer = 0;
            _playerParryBox.SetActive(true);
            _sword.GetComponent<Rigidbody>().detectCollisions = false;
        }

        public override void OnExit()
        {
            _input.parry = false;
            _playerParryBox.SetActive(false);
            _playerParryBox.GetComponent<MeshRenderer>().material.color = Color.white;
            timer = 0;
            _player.parryEnd = false;
            _sword.GetComponent<Rigidbody>().detectCollisions = true;

        }

    }


}
