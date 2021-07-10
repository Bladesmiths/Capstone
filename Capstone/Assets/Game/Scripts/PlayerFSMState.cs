using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    public class PlayerFSMState : IState
    {
        protected CharacterState id;

        public CharacterState ID { get { return id; } }

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
        private GameObject targetGO;

        public PlayerFSMState_MOVING(GameObject target)
        {
            targetGO = target;
        }
        public override void OnEnter()
        {
            Debug.Log("Moving!");
        }

        public override void OnExit()
        {
            Debug.Log("Not Moving!");
        }

    }

    public class PlayerFSMState_IDLE : PlayerFSMState
    {
        private GameObject targetGO;
        private GameObject playerGO;

        public PlayerFSMState_IDLE(GameObject target, GameObject player)
        {
            targetGO = target;
            playerGO = player;
        }

        public override void Tick()
        {
            
        }

        public override void OnEnter()
        {
            Debug.Log("Idle!");
        }

        public override void OnExit()
        {
            Debug.Log("Not Idle");
        }

    }
}
