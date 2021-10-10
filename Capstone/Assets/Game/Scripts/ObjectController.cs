using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Manages all damageable and damaging objects in the scene
    /// </summary>
    public class ObjectController : SerializedMonoBehaviour
    {
        [SerializeField] [Tooltip("The next ID to assign to an object")]
        private int currentValidId = 1;

        // Dictionaries of damageable and damaging objects
        [OdinSerialize] [Tooltip("IDs mapped to Damageable Objects and their teams")]
        private Dictionary<int, DamageableTeamPair> damageableObjects = new Dictionary<int, DamageableTeamPair>();
        [OdinSerialize] [Tooltip("IDs mapped to Damaging Objects and their teams")]
        private Dictionary<int, DamagingTeamPair> damagingObjects = new Dictionary<int, DamagingTeamPair>();

        public Dictionary<int, DamageableTeamPair> DamageableObjects { get => damageableObjects; }
        public Dictionary<int, DamagingTeamPair> DamagingObjects { get => damagingObjects; }

        void Start()
        {
            VerifyObjects();
        }

        void Update() { }

        /// <summary>
        /// Generate the next valid ID for an object and updates the valid ID field
        /// </summary>
        /// <returns>Returns the next valid ID</returns>
        public int GenerateID()
        {
            return currentValidId++;
        }

        /// <summary>
        /// Verify that all objects in dictionaries have the correct ID
        /// and have a reference to this object
        /// </summary>
        public void VerifyObjects()
        {
            // Verify damageable objects
            foreach (KeyValuePair<int, DamageableTeamPair> damageablePair in damageableObjects)
            {
                damageablePair.Value.DamageableObject.ID = damageablePair.Key;
                damageablePair.Value.DamageableObject.ObjectController = this;
            }

            // Verify damaging objects
            foreach (KeyValuePair<int, DamagingTeamPair> damagingPair in damagingObjects)
            {
                damagingPair.Value.DamagingObject.ID = damagingPair.Key;
                damagingPair.Value.DamagingObject.ObjectController = this;
            }
        }
    }

    /// <summary>
    /// Maintains a reference to a damageable object and its team
    /// </summary>
    public struct DamageableTeamPair
    {
        [SerializeField]
        private Team objectTeam;
        [SerializeField]
        private IDamageable damageableObject;

        public Team ObjectTeam { get => objectTeam; set => objectTeam = value; }
        public IDamageable DamageableObject { get => damageableObject; }

        public DamageableTeamPair(Team objectTeam, IDamageable damageableObject)
        {
            this.objectTeam = objectTeam;
            this.damageableObject = damageableObject;
        }
    }

    /// <summary>
    /// Maintains a reference to a damaging object and its team
    /// </summary>
    public struct DamagingTeamPair
    {
        [SerializeField]
        private Team objectTeam;
        [SerializeField]
        private IDamaging damagingObject;

        public Team ObjectTeam { get => objectTeam; set => objectTeam = value; }
        public IDamaging DamagingObject { get => damagingObject; }

        public DamagingTeamPair(Team objectTeam, IDamaging damagingObject)
        {
            this.objectTeam = objectTeam;
            this.damagingObject = damagingObject;
        }
    }
}