using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using Bladesmiths.Capstone.UI;

namespace Bladesmiths.Capstone
{
    public class SwordGemPickup : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public Dictionary<SwordType, GameObject> swords = new Dictionary<SwordType, GameObject>();

        public static SwordGemPickup instance;

        public UIManager uiManager;

        private void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// Adds the picked up sword to the Player's list of currently obtained swords
        /// </summary>
        /// <param name="sword"></param>
        [Button("Add Sword")]
        public void Pickup(SwordType sword)
        {            
            Player.instance.currentSwords.Add(sword, swords[sword]);
            uiManager.GainNewSword(sword);
        }
    }
}
