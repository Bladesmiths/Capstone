using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    public abstract class Character : SerializedMonoBehaviour, IDamageable
    {
        // Fields
        protected ThirdPersonController characterController;

        [SerializeField]
        private int id;

        private bool isAlive;

        [Header("Character Fields")]

        [SerializeField]
        private float maxHealth;

        [SerializeField]
        private float currentHealth;

        [SerializeField]
        private List<int> damagingObjectIDs = new List<int>();

        [SerializeField]
        protected ObjectController objectController;

        public event IIdentified.OnDestructionDelegate OnDestruction;

        // Properties
        public GameObject GameObject { get => gameObject; }
        public int ID { get => id; set => id = value; }
        public Team ObjectTeam { get; set; }
        public bool IsAlive { get => isAlive; set => isAlive = value; }
        public float Health 
        { 
            get => currentHealth; 
            //Make sure current health is never below zero or over max health
            set => currentHealth = Mathf.Min(Mathf.Max(value, 0), MaxHealth);
        }
        public float MaxHealth { get => maxHealth; set => maxHealth = value; }
        public List<int> DamagingObjectIDs { get => damagingObjectIDs; }
        public virtual ObjectController ObjectController { get => objectController; set => objectController = value; }

        /// <summary>
        /// Subtract an amount of damage from the character's health
        /// </summary>
        /// <param name="damagingID">The id of the damaging object that is damaging this character</param>
        /// <param name="damage">The amount of damage to be subtracted</param>
        /// <returns>Returns how much damage was taken</returns>
        public virtual float TakeDamage(int damagingID, float damage)
        {
            // If the damaging object belongs to the same team as this character
            // Or if the damaging object has already hurt this character recently
            // Don't take any damage
            if (objectController[damagingID].ObjectTeam == ObjectTeam || 
                DamagingObjectIDs.Contains(damagingID))
            {
                damage = 0; 
            }
            
            // Subtract damage from health
            // Round damage to an int to avoid rounding isues with health bar chunk display
            Health -= (int)damage;

            if (Health <= 0)
            {
                Die();
            }
            else
            {
                // If damage was taken
                // Add the damaging object's id to the damaging id list
                // And subscribe to that object's DamagingFinished event
                // Stopping it from hurting this character again right away
                if (damage != 0)
                {
                    AddDamagingID(damagingID);
                }
            }

            // Log the amount of damage taken
            //Debug.Log($"DAMAGE TAKEN: {damage}");

            // Return damage
            return damage;
        }

        // Protected Methods
        protected virtual void Die()
        {
            if (OnDestruction != null)
            {
                OnDestruction(id);
            }

            isAlive = false;
        }
        public abstract void Respawn();

        /// <summary>
        /// Add a damaging ID to the object and subscribe to that object's damaging event
        /// </summary>
        /// <param name="damagingID">The id of the damaging object to be added</param>
        public void AddDamagingID(int damagingID)
        {
            damagingObjectIDs.Add(damagingID);
            ((IDamaging)objectController[damagingID].IdentifiedObject).DamagingFinished += RemoveDamagingID;
            ((IDamaging)objectController[damagingID].IdentifiedObject).Damaging = true;
        }

        /// <summary>
        /// Remove an id from the damaging id list
        /// </summary>
        /// <param name="damagingID">The id to be removed</param>
        public void RemoveDamagingID(int damagingID)
        {
            damagingObjectIDs.Remove(damagingID);
        }
    }
}
