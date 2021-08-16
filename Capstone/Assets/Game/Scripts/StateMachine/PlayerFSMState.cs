using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Base class for possible states
    /// </summary>
    public class PlayerFSMState : IState
    {
        protected PlayerCondition id;

        public PlayerCondition ID { get; set; }

        public virtual void Tick()
        {

        }

        public virtual void OnEnter()
        {
            
        }

        public virtual void OnExit()
        {

        }


    }

    public class PlayerFSMState_MOVING : PlayerFSMState
    {       
        public PlayerFSMState_MOVING()
        {
            
        }
        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

    }

    public class PlayerFSMState_IDLE : PlayerFSMState
    {      
        public PlayerFSMState_IDLE()
        {
            
        }

        public override void Tick()
        {
            
        }

        public override void OnEnter()
        {
            
        }

        public override void OnExit()
        {
            
        }

    }

    public class PlayerFSMState_ATTACK : PlayerFSMState
    {
        private Player _player;
        private PlayerInputsScript _input;

        public PlayerFSMState_ATTACK(Player player, PlayerInputsScript input)
        {
            _player = player;
            _input = input;

        }

        public override void Tick()
        {
            //int layerMask = 1 << 8;

            
            //layerMask = ~layerMask;
            RaycastHit hit;

            if (Physics.Raycast(_player.transform.position + Vector3.up, _player.transform.TransformDirection(Vector3.forward), out hit, 2f))
            {
                Debug.DrawRay(_player.transform.position + Vector3.up, _player.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
                if (hit.collider.gameObject.GetComponent<Enemy>())
                {
                    hit.collider.gameObject.GetComponent<Enemy>().Damaged();

                }
            }
            else
            {
                Debug.DrawRay(_player.transform.position, _player.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }

            _input.attack = false;

        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

    }
}
