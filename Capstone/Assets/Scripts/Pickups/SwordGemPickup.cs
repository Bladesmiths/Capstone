using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

namespace Bladesmiths.Capstone
{
    [CreateAssetMenu(fileName = "SwordGemPickup", menuName = "ScriptableObjects/SwordGemPickup")]
    public class SwordGemPickup : SerializedScriptableObject
    {
        [OdinSerialize]
        public Dictionary<SwordType, GameObject> swords = new Dictionary<SwordType, GameObject>();

        /// <summary>
        /// Adds the picked up sword to the Player's list of currently obtained swords
        /// </summary>
        /// <param name="sword"></param>
        public void Pickup(SwordType sword)
        {            
            Player.instance.currentSwords.Add(sword, swords[sword]);
        }
    }
}
