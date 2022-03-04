using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    public class Enemy_Shield : Enemy
    {
        public GameObject shield;

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

        protected override void Die()
        {

        }
        public override void Respawn()
        {

        }

        /// <summary>
        /// Checks to see if the Enemy can see the Player
        /// </summary>
        /// <param name="target">The Player</param>
        /// <param name="fov">The Field of View</param>
        /// <returns></returns>
        public bool InSight(Transform target, float fov)
        {
            Vector3 dir = target.position - transform.position;
            return Vector3.Angle(dir, transform.forward) < fov;
        }

        /// <summary>
        /// Subtract an amount of damage from the character's health
        /// </summary>
        /// <param name="damagingID">The id of the damaging object that is damaging this character</param>
        /// <param name="damage">The amount of damage to be subtracted</param>
        /// <returns>Returns a boolean indicating whether damage was taken or not</returns>
        public override float TakeDamage(int damagingID, float damage)
        {
            // The resullt of Character's Take Damage
            // Was damage taken or not
            if(shield.GetComponent<Shield>().BlockTriggered)
            {
                damage = 0;
            }

            float damageResult = base.TakeDamage(damagingID, damage);

            // If damage was taken
            // Change the object to red and set damaged to true
            if (damageResult > 0)
            {
                //damaged = true;
            }

            // Return whether damage was taken or not
            return damageResult;
        }
    }
}
