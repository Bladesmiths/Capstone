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
        private Team objectTeam; 

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

        // Properties
        public int ID { get => id; set => id = value; }
        public Team DamageableObjectTeam { get => objectTeam; set => objectTeam = value; }
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
        public virtual bool TakeDamage(int damagingID, float damage)
        {
            if (objectController.DamagingObjects[damagingID].ObjectTeam == objectTeam || 
                DamagingObjectIDs.Contains(damagingID))
            {
                damage = 0; 
            }
            currentHealth -= damage;

            if (damage != 0)
            {
                damagingObjectIDs.Add(damagingID);
                objectController.DamagingObjects[damagingID].DamagingObject.DamagingFinished += RemoveDamagingID; 
            }

            Debug.Log($"DAMAGE TAKEN: {damage}");

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

        public void RemoveDamagingID(int damagingID)
        {
            damagingObjectIDs.Remove(damagingID);
        }
    }
}
