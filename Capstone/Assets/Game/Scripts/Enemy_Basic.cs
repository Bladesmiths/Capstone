using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    public class Enemy_Basic : Enemy
    {
        private EnemyFSMState_SURROUND_BASIC surround;

        public override void Awake()
        {
            base.Awake();

        }

        public override void Start()
        {
            base.Start();

            // Instantiates the surround state for the basic enemy
            surround = new EnemyFSMState_SURROUND_BASIC();

            FSM.AddTransition(seek, surround, SurroundPlayer());

        }

        /// <summary>
        /// The code for surrounding the enemy
        /// </summary>
        /// <returns></returns>
        public Func<bool> SurroundPlayer() => () => Vector3.Distance(player.transform.position, transform.position) <= 2;

        public override void Update()
        {
            base.Update();

            // Activate the FSM
            FSM.Tick();

        }

        void OnCollisionEnter(Collision collision)
        {

        }       

        protected override void Die()
        {

        }
        public override void Respawn()
        {

        }        
    }
}
