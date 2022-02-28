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
        public override void Awake()
        {
            base.Awake();

        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();


        }

        void OnCollisionEnter(Collision collision)
        {

        }       

        public override void Respawn()
        {

        }        
    }
}
