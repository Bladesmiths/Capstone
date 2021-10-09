using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Bladesmiths.Capstone
{
    public class ObjectController : SerializedMonoBehaviour
    {
        [SerializeField]
        private int currentValidId = 1;
        [OdinSerialize]
        private Dictionary<int, DamageableTeamPair> damageableObjects = new Dictionary<int, DamageableTeamPair>();
        [OdinSerialize]
        private Dictionary<int, DamagingTeamPair> damagingObjects = new Dictionary<int, DamagingTeamPair>();

        public Dictionary<int, DamageableTeamPair> DamageableObjects { get => damageableObjects; }
        public Dictionary<int, DamagingTeamPair> DamagingObjects { get => damagingObjects; }

        // Start is called before the first frame update
        void Start()
        {
            VerifyObjects();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public int GenerateID()
        {
            return currentValidId++;
        }

        public void VerifyObjects()
        {
            foreach (KeyValuePair<int, DamageableTeamPair> damageablePair in damageableObjects)
            {
                damageablePair.Value.DamageableObject.ID = damageablePair.Key;
                damageablePair.Value.DamageableObject.ObjectController = this;
            }

            foreach (KeyValuePair<int, DamagingTeamPair> damagingPair in damagingObjects)
            {
                damagingPair.Value.DamagingObject.ID = damagingPair.Key;
                damagingPair.Value.DamagingObject.ObjectController = this;
            }
        }
    }

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