using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public class TestingEnemy : Enemy
    {
        // Keeping start in case things are added later
        void Start() 
        {
            Health = 1000;
        }

        // Keeping update so that parent's update does not run
        public override void Update() { }
        
        /// <summary>
        /// Subtract damage from the enemy's health and react to damage
        /// </summary>
        /// <param name="damage">The amount of damage to subtract</param>
        public override bool TakeDamage(int damagingID, float damage)
        {
            // Testing Value
            // Needs to be changed later so that it reflects the damage of the sword)
            damage = 5;
            bool damageResult = base.TakeDamage(damagingID, damage);

            if (damageResult)
            {
                StartCoroutine(Util.DamageMaterialTimer(gameObject.GetComponentInChildren<MeshRenderer>()));

                // Testing
                //((TestDataInt)GameObject.Find("TestingController").GetComponent<TestingController>().ReportedData["numAttacks"]).Data.CurrentValue++;
            }

            return damageResult;
        }
    }
}