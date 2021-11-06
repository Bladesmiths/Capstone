using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// A scriptable object to hold various data values changed frequently when balancing
    /// </summary>
    [CreateAssetMenu(fileName = "BalancingData", menuName = "ScriptableObjects/BalancingData")]
    public class BalancingData : SerializedScriptableObject
    {
        #region Player Fields
        [Header("Player")]
        [OdinSerialize] [Tooltip("The player's swords")]
        private Dictionary<Enums.SwordType, SwordData> swordData = new Dictionary<Enums.SwordType, SwordData>(); 
        #endregion

        #region Enemy Fields
        //[Header("Enemy")]
        #endregion

        #region Boss Fields
        //[Header("Boss")]
        #endregion

        #region Player Properties
        public Dictionary<Enums.SwordType, SwordData> SwordData { get => swordData; }
        #endregion
    }
}