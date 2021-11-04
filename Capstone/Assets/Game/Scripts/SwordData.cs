using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SwordData", menuName = "ScriptableObjects/SwordData")]
public class SwordData : SerializedScriptableObject
{
    #region Fields
    [SerializeField] [Tooltip("The damage of the sword")]
    private float damage;

    [SerializeField] [Range(0.0f, 1.0f)]
    [Tooltip("The percentage of damage that gets through the sword's block")]
    private float chipDamagePercentage = 0.4f;

    [SerializeField] [Range(0.0f, 1.0f)]
    [Tooltip("The percentage of damage that is converted to health")]
    private float lifeStealPercentage = 0.1f;

    [SerializeField] [Range(0.0f, 2.0f)]
    [Tooltip("The percentage that damage is reduced by when hitting the player while wielding the sword")]
    private float damageTakenModifier = 1.0f;

    [SerializeField] [Tooltip("The multiplier to apply to the player's speed when moving")]
    private float playerMovementMultiplier = 1.0f;

    [SerializeField] [Tooltip("How much damage must be done to the player before they are knocked back")]
    private float knockboackThreshold;

    [SerializeField] [Tooltip("Length of time betwen releasing block and parry becoming active")]
    private float parryDelay = 0.1f;

    [SerializeField] [Tooltip("Length of time parry is active")]
    private float parryLength = 0.1f;

    [SerializeField] [Tooltip("Length of time after parry ceases to be active that the player cannot act")]
    private float parryCooldown = 0.1f;
    #endregion

    #region Properties
    public float Damage { get => damage; }
    public float ChipDamagePercentage { get => chipDamagePercentage; }
    public float LifeStealPercentage { get => lifeStealPercentage; }
    public float PlayerMovementMultiplier { get => playerMovementMultiplier; }
    public float DamageTakenModifier { get => damageTakenModifier; }
    public float KnockbackTreshold { get => knockboackThreshold; }
    public float ParryDelay { get => parryDelay; }
    public float ParryLength { get => parryLength; }
    public float ParryCooldown { get => parryCooldown; }
    #endregion
}
