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
        // TODO: Implement Sword Switching Delay
        [SerializeField] [Tooltip("The time it takes to switch swords")]
        private float swordSwitchingTime; 

        [OdinSerialize] [Tooltip("The player's swords")]
        private Dictionary<Enums.SwordType, SwordData> swordData = new Dictionary<Enums.SwordType, SwordData>();

        [OdinSerialize] [Tooltip("The speed of each sword's attack animation")]
        private Dictionary<Enums.SwordType, float> attackAnimSpeeds = new Dictionary<Enums.SwordType, float>(); 
        #endregion

        #region Enemy Fields
        //[Header("Enemy")]
        #endregion

        #region Boss Fields
        //[Header("Boss")]
        #endregion

        #region Player Properties
        public float SwordSwitchingTime => SwordSwitchingTime;
        public Dictionary<Enums.SwordType, SwordData> SwordData => swordData;
        public Dictionary <Enums.SwordType, float> AttackAnimSpeeds => attackAnimSpeeds;
        #endregion
    }
}