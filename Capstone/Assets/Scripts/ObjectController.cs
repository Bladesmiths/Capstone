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
        //[SerializeField] [Tooltip("The next ID to assign to an object")]
        private int currentValidId = -1;

        // Dictionaries of damageable and damaging objects
        [OdinSerialize] [Tooltip("IDs mapped to Damageable Objects and their teams")]
        private Dictionary<int, IdentifiedTeamPair> identifiedObjects = new Dictionary<int, IdentifiedTeamPair>();

        // Indexer to return an identified pair from an id
        public IdentifiedTeamPair this[int id]
        {
            get { return identifiedObjects[id]; }
        }

        void Start()
        {
            // We should come up with a way to check the dictionary and
            // making a unique ID to go with the player
            //currentValidId = -1;
            Player player = GameObject.Find("Player").GetComponent<Player>();
            IdentifiedTeamPair playerTeamPair = new IdentifiedTeamPair(Team.Player, player);
            IdentifiedTeamPair playerSwordPair = new IdentifiedTeamPair(Team.Player, player.Sword.GetComponent<Sword>());

            identifiedObjects.Add(GenerateID(), playerTeamPair);
            identifiedObjects.Add(GenerateID(), playerSwordPair);

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

        public void AddIdentifiedObject(Team objectTeam, IIdentified identifiedObject)
        {
            int objectID = GenerateID(); 
            identifiedObjects.Add(objectID, new IdentifiedTeamPair(objectTeam, identifiedObject));

            identifiedObject.ID = objectID;
            identifiedObject.ObjectTeam = objectTeam;
            identifiedObject.ObjectController = this;

            identifiedObject.OnDestruction += RemoveIdentifiedObject;
        }

        public void RemoveIdentifiedObject(int objectID)
        {
            identifiedObjects.Remove(objectID);
        }

        /// <summary>
        /// Verify that all objects in dictionaries have the correct ID
        /// and have a reference to this object
        /// </summary>
        public void VerifyObjects()
        {
            // Verify objects
            foreach (KeyValuePair<int, IdentifiedTeamPair> damageablePair in identifiedObjects)
            {
                damageablePair.Value.IdentifiedObject.ID = damageablePair.Key;
                damageablePair.Value.IdentifiedObject.ObjectTeam = damageablePair.Value.ObjectTeam;
                damageablePair.Value.IdentifiedObject.ObjectController = this;
            }
        }
    }

    /// <summary>
    /// Maintains a reference to a damageable object and its team
    /// </summary>
    public struct IdentifiedTeamPair
    {
        [SerializeField]
        private Team objectTeam;
        [SerializeField]
        private IIdentified identifiedObject;

        public Team ObjectTeam { get => objectTeam; set => objectTeam = value; }
        public IIdentified IdentifiedObject { get => identifiedObject; }

        public IdentifiedTeamPair(Team objectTeam, IIdentified identifiedObject)
        {
            this.objectTeam = objectTeam;
            this.identifiedObject = identifiedObject;
        }
    }
}