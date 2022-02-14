using Bladesmiths.Capstone.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    /// <summary>
    /// Represents the player's sword and its behaviors
    /// </summary>
    public class Sword : MonoBehaviour, IDamaging
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
        [SerializeField]
        private FMODUnity.EventReference SwordHitEvent;
        public bool sfxPlay;

        private bool damaging;
        private float damagingTimerLimit = 1f;
        private float damagingTimer;

        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;
        public event IIdentified.OnDestructionDelegate OnDestruction;
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
        public bool Damaging { get; set; }
        public int ID { get; set; }
        public Team ObjectTeam { get; set; }
        public ObjectController ObjectController { get; set; }
        #endregion

        void Start()
        {
            player = gameObject.transform.root.GetComponent<Player>();
            sfxPlay = false;
        }

        void Update() 
        {
            if (damaging)
            {
                // Update the timer
                damagingTimer += Time.deltaTime;

                // If the timer is equal to or exceeds the limit
                if (damagingTimer >= damagingTimerLimit)
                {
                    // If the damaging finished event has subcribing delegates
                    // Call it, running all subscribing delegates
                    if (DamagingFinished != null)
                    {
                        DamagingFinished(ID);
                    }
                    // If the damaging finished event doesn't have any subscribing events
                    // Something has gone wrong because damaging shouldn't be true otherwise
                    else
                    {
                        Debug.Log("Damaging Finished Event was not subscribed to correctly");
                    }

                    // Reset fields
                    damagingTimer = 0.0f;
                    damaging = false;
                }
            }

        }

        public void TriggerVFX()
        {
            // TODO: Implement TriggerVFX
        }

        void OnCollisionEnter(Collision col)
        {
            if(col.gameObject.GetComponent<BreakableBox>())
            {
                FMODUnity.RuntimeManager.PlayOneShot(SwordHitEvent);
            }


            if(col.gameObject.GetComponent<Shield>())
            {
                FMODUnity.RuntimeManager.PlayOneShot(SwordHitEvent);

            }
            else if (col.gameObject.GetComponent<Enemy>())
            {
                if (!col.gameObject.GetComponent<Enemy>().blocked)
                {
                    FMODUnity.RuntimeManager.PlayOneShot(SwordHitEvent);
                    player.SwordAttack(col.gameObject.GetComponent<Enemy>().ID);
                }
                
            }
        }
    }
}
