using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Represents the player's sword and its behaviors
    /// </summary>
    public class Sword : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private List<AnimationClip> swordSwings = new List<AnimationClip>();
        [SerializeField]
        private List<GameObject> vfx = new List<GameObject>();
        [SerializeField]
        private GameObject swordModel; 
        [SerializeField]
        private Enums.SwordType swordType;
        [SerializeField]
        private Transform offset;
        private Player player;
        #endregion

        #region Properties
        public float Damage { get => BalancingData.SwordData[swordType].Damage; }
        public float ChipDamagePercentage { get => BalancingData.SwordData[swordType].ChipDamagePercentage; }
        public float LifeStealPercentage { get => BalancingData.SwordData[swordType].LifeStealPercentage; }
        public float PlayerMovementMultiplier { get => BalancingData.SwordData[swordType].PlayerMovementMultiplier; }
        public float DamageTakenModifier { get => BalancingData.SwordData[swordType].DamageTakenModifier; }
        public float KnockbackTreshold { get => BalancingData.SwordData[swordType].KnockbackTreshold; }
        public float ParryDelay { get => BalancingData.SwordData[swordType].ParryDelay; }
        public float ParryLength { get => BalancingData.SwordData[swordType].ParryLength; }
        public float ParryCooldown { get => BalancingData.SwordData[swordType].ParryCooldown; }
        public bool IsActive { get; set; }
        public Enums.SwordType SwordType { get => swordType; }
        public Transform Offset { get => offset; }
        private BalancingData BalancingData { get => player.CurrentBalancingData; }
        #endregion

        void Start()
        {
            player = gameObject.transform.root.GetComponent<Player>(); 
        }

        void Update() { }

        public void TriggerVFX()
        {
            // TODO: Implement TriggerVFX
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.GetComponent<Enemy>())
            {
                player.SwordAttack(col.gameObject.GetComponent<Enemy>().ID);
            }
        }
    }
}
