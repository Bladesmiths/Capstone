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
        public virtual void TakeDamage(float damage)
        {
            currentHealth -= damage;
        }

        // Protected Methods
        protected abstract void Attack();
        protected abstract void ActivateAbility();
        protected abstract void Block();
        protected abstract void Parry();
        protected abstract void Dodge();
        protected abstract void SwitchWeapon(int weaponSelect);
        protected abstract void Die();
    }
}