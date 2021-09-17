using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public class TestingEnemy : Enemy
    {
        #region Fields
        // The health of the enemy
        [SerializeField]
        private float health = 1000;
        #endregion

        // Keeping start in case things are added later
        void Start() { }

        // Keeping update so that parent's update does not run
        public override void Update() { }
        
        /// <summary>
        /// Subtract damage from the enemy's health and react to damage
        /// </summary>
        /// <param name="damage">The amount of damage to subtract</param>
        public override void TakeDamage(float damage)
        {
            // Testing Value
            // Needs to be changed later so that it reflects the damage of the sword)
            damage = 5;
            health = Mathf.Max(health - damage, 0);


            StartCoroutine(Util.DamageMaterialTimer(gameObject.GetComponentInChildren<MeshRenderer>()));
        }
    }
}