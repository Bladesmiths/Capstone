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
        private ObjectController objectController;

        public event IIdentified.OnDestructionDelegate OnDestruction;

        // Properties
        public int ID { get => id; set => id = value; }
        public Team ObjectTeam { get; set; }
        public bool IsAlive { get => isAlive; set => isAlive = value; }
        public float Health 
        { 
            get => currentHealth; 
            set => currentHealth = Mathf.Max(value, 0);
        }
        public float MaxHealth { get => maxHealth; set => maxHealth = value; }
        public List<int> DamagingObjectIDs { get => damagingObjectIDs; }
        public ObjectController ObjectController { get => objectController; set => objectController = value; }

        // Public Methods
        /// <summary>
        /// Subtract an amount of damage from the character's health
        /// </summary>
        /// <param name="damagingID">The id of the damaging object that is damaging this character</param>
        /// <param name="damage">The amount of damage to be subtracted</param>
        /// <returns>Returns a boolean indicating whether damage was taken or not</returns>
        public virtual bool TakeDamage(int damagingID, float damage)
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
            Health -= damage;

            // If damage was taken
            // Add the damaging object's id to the damaging id list
            // And subscribe to that object's DamagingFinished event
            // Stopping it from hurting this character again right away
            if (damage != 0)
            {
                AddDamagingID(damagingID); 
            }

            // Log the amount of damage taken
            Debug.Log($"DAMAGE TAKEN: {damage}");

            // Return true if damage is greater than 0
            // Return false if damage is 0 or less
            return damage > 0;
        }

        // Protected Methods
        protected abstract void Attack();
        protected abstract void ActivateAbility();
        protected abstract void Block();
        protected abstract void Parry();
        protected abstract void Dodge();
        protected abstract void SwitchWeapon(int weaponSelect);
        protected abstract void Die();

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
