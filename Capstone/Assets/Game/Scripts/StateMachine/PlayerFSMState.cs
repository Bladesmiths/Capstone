using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using StarterAssets;

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
        public float timer;
        StarterAssetsInputs _inputs;

        public PlayerFSMState_MOVING(StarterAssetsInputs inputs)
        {
            _inputs = inputs;
        }

        public override void Tick()
        {
            if(_inputs.move == Vector2.zero)
            {
                timer += Time.deltaTime;

            }
        }

        public override void OnEnter()
        {
            timer = 0;
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

    public class PlayerFSMState_PARRY : PlayerFSMState
    {
        public float timer;
        private GameObject _playerParryBox;
        public PlayerFSMState_PARRY(GameObject playerParryBox)
        {
            _playerParryBox = playerParryBox;
        }

        public override void Tick()
        {
            timer += Time.deltaTime;

            _playerParryBox.SetActive(true);
        }

        public override void OnEnter()
        {
            timer = 0;
        }

        public override void OnExit()
        {
            _playerParryBox.SetActive(false);
        }

    }

}
