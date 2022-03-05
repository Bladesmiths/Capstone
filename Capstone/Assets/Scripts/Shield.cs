using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    public class Shield : MonoBehaviour
    {
        private List<int> blockedObjectIDs = new List<int>();
        private Player player;
        private Enemy enemy;
        private GameObject shieldChunks;
        private int chunksRemoved;

        public ObjectController ObjectController { get; set; }
        public bool BlockTriggered { get; private set; }
        public bool Active { get; set; }

        public bool IsEmpty { get { return transform.childCount == 0; } }

        public delegate void OnBlockDelegate(float chipDamageTotal);

        // Event declaration
        public event OnBlockDelegate OnBlock;

        // Start is called before the first frame update
        void Start()
        {
            ObjectController = GameObject.Find("ObjectController").GetComponent<ObjectController>();
            player = Player.instance;
            enemy = gameObject.GetComponentInParent<Enemy>();
            Active = true;
            shieldChunks = gameObject;
            chunksRemoved = 1;
        }

        // Update is called once per frame
        void Update() { }

        /// <summary>
        /// Removes an ID from the list of blocked IDs
        /// If there are no more blocked IDs, then block is no longer triggered
        /// </summary>
        /// <param name="blockedID"></param>
        public void RemoveBlockedID(int blockedID)
        {
            blockedObjectIDs.Remove(blockedID);

            if (blockedObjectIDs.Count == 0)
            {
                BlockTriggered = false;
            }
        }

        public void RemoveRandomChunk()
        {           
            if(shieldChunks.transform.childCount <= 0)
            {
                Debug.Log("NO MORE CHILDREN IN SHIELD");
                shieldChunks.transform.parent = null;
                Destroy(shieldChunks);
                return;
            }

            GameObject remover = shieldChunks;

            GameObject removedChunk = remover.transform.GetChild(UnityEngine.Random.Range(0, remover.transform.childCount)).gameObject;
            removedChunk.transform.parent = null;
            removedChunk.AddComponent<BoxCollider>();
            removedChunk.AddComponent<Rigidbody>();
            removedChunk.AddComponent<EnemyChunk>();
        }

        public int NumChunks()
        {
            int dmg = (int)player.CurrentSword.Damage;
            if (player.CurrentSword.SwordType == SwordType.Ruby)
            {
                dmg *= 2;
            }

            return chunksRemoved * (dmg / 5);
        }

        public void RemoveChunks()
        {
            int num = NumChunks();
            for (int i = 0; i < num; i++)
            {
                RemoveRandomChunk();
            }

        }

        private void OnCollisionEnter(Collision other)
        {
            //// Exits the method if the colliding object is in Enemy or Default
            //// This will probably need to be added to as we go on
            //if (!Active || (LayerMask.GetMask("Enemy", "Default", "TargetLock") & 1 << other.gameObject.layer) != 0)
            //{
            //    Debug.Log("Colliding Object is in either 'Enemy', 'Default', or 'TargetLock'!");
            //    return;
            //}

            //// Retrieves the IDamaging Object
            IDamaging damagingObject = other.gameObject.GetComponent<IDamaging>();
            if(other.gameObject.GetComponent<Sword>())
            {
                if(ObjectController[damagingObject.ID].ObjectTeam == Enums.Team.Player &&
                    !enemy.DamagingObjectIDs.Contains(damagingObject.ID))
                {
                    Debug.Log("Collision Entered! : " + other.gameObject);
                    // Block has been triggered
                    enemy.blocked = true;
                    //ObjectController[damagingObject.ID].IdentifiedObject
                    //enemy.AddDamagingID(damagingObject.ID);
                    // Add the ID of the damaging object to the blocked ID list
                    // And subscribe to the DamagingFinished event
                    //blockedObjectIDs.Add(damagingObject.ID);
                    //damagingObject.DamagingFinished += RemoveBlockedID;

                    // Calculate the chip damage and make the Player take that damage
                    //float blockedDamage = 0;//damagingObject.Damage * player.ChipDamagePercentage;
                    //enemy.TakeDamage(damagingObject.ID, blockedDamage);

                    //OnBlock(blockedDamage);
                }



            }
            // If there is a damaging object
            //if (damagingObject != null)
            //{
            //    // If the damaging object is not on the same team as the Enemy
            //    // And its ID has not already been blocked
            //    if (ObjectController[damagingObject.ID].ObjectTeam != Enums.Team.Enemy &&
            //        !enemy.DamagingObjectIDs.Contains(damagingObject.ID))
            //    {
            //        // Block has been triggered
            //        BlockTriggered = true;
            //        Debug.Log("Block Triggered!");

            //        // Add the ID of the damaging object to the blocked ID list
            //        // And subscribe to the DamagingFinished event
            //        blockedObjectIDs.Add(damagingObject.ID);
            //        damagingObject.DamagingFinished += RemoveBlockedID;

            //        // Calculate the chip damage and make the Player take that damage
            //        //float blockedDamage = damagingObject.Damage * enemy.ChipDamagePercentage;
            //        enemy.TakeDamage(damagingObject.ID, damagingObject.Damage);

            //        //OnBlock(damagingObject.Damage);

            //        // Debug stuff
            //        //Debug.Log($"Block Triggered by: {other.gameObject}");
            //    }
            //}
        }
    }
}

