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

        [SerializeField] private List<AnimationClip> swordSwings = new List<AnimationClip>();
        [SerializeField] private List<GameObject> vfx = new List<GameObject>();
        [SerializeField] private GameObject swordModel;
        [SerializeField] private Enums.SwordType swordType;
        [SerializeField] private Transform offset;
        private Player player;
        [SerializeField] private FMODUnity.EventReference SwordHitEvent;
        public bool sfxPlay;

        public bool damaging;
        private float damagingTimerLimit = 1f;
        private float damagingTimer;

        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;
        public event IIdentified.OnDestructionDelegate OnDestruction;

        #endregion

        #region Properties

        public float Damage
        {
            get => BalancingData.SwordData[swordType].Damage;
        }

        public float ChipDamagePercentage
        {
            get => BalancingData.SwordData[swordType].ChipDamagePercentage;
        }

        public float LifeStealPercentage
        {
            get => BalancingData.SwordData[swordType].LifeStealPercentage;
        }

        public float PlayerMovementMultiplier
        {
            get => BalancingData.SwordData[swordType].PlayerMovementMultiplier;
        }

        public float DamageTakenModifier
        {
            get => BalancingData.SwordData[swordType].DamageTakenModifier;
        }

        public float KnockbackTreshold
        {
            get => BalancingData.SwordData[swordType].KnockbackTreshold;
        }

        public float ParryDelay
        {
            get => BalancingData.SwordData[swordType].ParryDelay;
        }

        public float ParryLength
        {
            get => BalancingData.SwordData[swordType].ParryLength;
        }

        public float ParryCooldown
        {
            get => BalancingData.SwordData[swordType].ParryCooldown;
        }

        public bool IsActive { get; set; }

        public Enums.SwordType SwordType
        {
            get => swordType;
            set => swordType = value;
        }

        public Transform Offset
        {
            get => offset;
        }

        private BalancingData BalancingData
        {
            get => player.CurrentBalancingData;
        }

        public bool Damaging { get; set; }
        public int ID { get; set; }
        public Team ObjectTeam { get; set; }
        public ObjectController ObjectController { get; set; }

        public GameObject GameObject
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        #endregion

        void Start()
        {
            player = gameObject.transform.root.GetComponent<Player>();
            ID = player.ID; 
            sfxPlay = false;
            damagingTimerLimit = 1f;
            damagingTimer = 0f;
            ObjectController = ObjectController.Instance;
            ObjectController.Instance.AddIdentifiedObject(Team.Player, this);
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

        public void ToggleTrailVFX(bool isEnabled)
        {
            // TODO: Implement TriggerVFX
            if (vfx.Count > 0)
            {
                if (isEnabled)
                {
                    vfx[1].GetComponent<VFXManager>().DisableVFX();
                    vfx[2].GetComponent<VFXManager>().DisableVFX();
                    vfx[0].GetComponent<VFXManager>().EnableVFX();
                }
                else
                {
                    vfx[0].GetComponent<VFXManager>().DisableVFX();
                    vfx[1].GetComponent<VFXManager>().DisableVFX();
                    vfx[2].GetComponent<VFXManager>().DisableVFX();
                }
            }
        }

        public void AddToDamaging(int swordID)
        {
            player.AddDamagingID(swordID);

        }

        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.GetComponent<BreakableBox>())
            {
                FMODUnity.RuntimeManager.PlayOneShot(SwordHitEvent);
            }

            if (col.gameObject.GetComponent<Shield>())
            {
                FMODUnity.RuntimeManager.PlayOneShot(SwordHitEvent);
                col.gameObject.transform.parent.GetComponent<Enemy>().AddDamagingID(ID);
                col.gameObject.GetComponent<Shield>().RemoveChunks();
                damaging = true;
            }
            else if (col.gameObject.GetComponent<IDamageable>() != null)
            {
                if (col.gameObject.GetComponent<Enemy>() || col.gameObject.GetComponent<Boss>())
                {
                    if(col.gameObject.GetComponent<Enemy_Shield>())
                    {
                        if (col.gameObject.GetComponent<Enemy_Shield>().shield != null)
                        {
                            if (col.gameObject.GetComponent<Enemy_Shield>().InSight(player.transform, 55) &&
                                !col.gameObject.GetComponent<Enemy_Shield>().shield.GetComponent<Shield>().IsEmpty)
                            {
                                FMODUnity.RuntimeManager.PlayOneShot(SwordHitEvent);
                                col.gameObject.transform.GetComponent<Enemy_Shield>().AddDamagingID(ID);

                                damaging = true;
                            }
                            else
                            {
                                //Debug.Log(col.gameObject);
                                FMODUnity.RuntimeManager.PlayOneShot(SwordHitEvent);
                                //Debug.Log(col.gameObject.GetComponent<IDamageable>().ID);
                                player.AddDamagingID(ID);
                                player.SwordAttack(col.gameObject.GetComponent<IDamageable>().ID);
                            }

                        }
                        else
                        {
                            //Debug.Log(col.gameObject);
                            FMODUnity.RuntimeManager.PlayOneShot(SwordHitEvent);
                            //Debug.Log(col.gameObject.GetComponent<IDamageable>().ID);
                            player.AddDamagingID(ID);
                            player.SwordAttack(col.gameObject.GetComponent<IDamageable>().ID);
                        }
                    }
                    else
                    {
                        //Debug.Log(col.gameObject);
                        FMODUnity.RuntimeManager.PlayOneShot(SwordHitEvent);
                        //Debug.Log(col.gameObject.GetComponent<IDamageable>().ID);
                        player.AddDamagingID(ID);
                        player.SwordAttack(col.gameObject.GetComponent<IDamageable>().ID);

                    }                   
                                       

                    if (vfx.Count > 0 && swordType == SwordType.Ruby)
                    {
                        vfx[0].GetComponent<VFXManager>()
                            .PlayCollisionOneShotVFX(3.0f, col.transform.position, col.transform.rotation);
                    }

                }
            }
        }
    }
}