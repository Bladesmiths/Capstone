using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public abstract class Character : MonoBehaviour, IDamageable
    {
        // Fields
        protected ThirdPersonController characterController;

        private bool isAlive;
        private float currentHealth;
        private float maxHealth;

        // Properties
        public bool IsAlive { get => isAlive; set => isAlive = value; }
        public float Health { get => currentHealth; set => currentHealth = value; }
        public float MaxHealth { get => maxHealth; set => maxHealth = value; }

        // Public Methods
        public void TakeDamage(float damage)
        {
            throw new System.NotImplementedException();
        }

        // Protected Methods
        protected virtual void Attack() { }
        protected virtual void ActivateAbility() { }
        protected virtual void Block() { }
        protected virtual void Parry() { }
        protected virtual void Dodge() { }
        protected virtual void SwitchWeapon(int weaponSelect) { }
        protected virtual void Die() { }
    }
}
