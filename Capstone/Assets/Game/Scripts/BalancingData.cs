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
        [SerializeField] [Tooltip("Length of time betwen releasing block and parry becoming active")]
        private float parryDelay;

        [SerializeField] [Tooltip("Length of time parry is active")]
        private float parryLength;

        [SerializeField] [Tooltip("Length of time after parry ceases to be active that the player cannot act")]
        private float parryCooldown;
        #endregion

        #region Enemy Fields
        //[Header("Enemy")]
        #endregion

        #region Boss Fields
        //[Header("Boss")]
        #endregion

        #region Player Properties
        public float ParryDelay { get => parryDelay; }
        public float ParryLength { get => parryLength; }
        public float ParryCooldown { get => parryCooldown; }
        #endregion
    }
}