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
    private float chipDamagePercentage;

    [SerializeField] [Range(0.0f, 1.0f)]
    [Tooltip("The percentage of damage that is converted to health")]
    private float lifeStealPercentage;

    [SerializeField] [Tooltip("The multiplier to apply to the player's speed when moving")]
    private float playerMovementMultiplier;

    [SerializeField] [Tooltip("The percentage that damage is reduced by when hitting the player while wielding the sword")]
    private float damageModifier;

    [SerializeField] [Tooltip("The percentage that damage is reduced by when hitting the player while blocking")]
    private float blockedDamageModifier;

    [SerializeField] [Tooltip("How much damage must be done to the player before they are knocked back")]
    private float knockboackThreshold;

    [SerializeField] [Tooltip("Length of time betwen releasing block and parry becoming active")]
    private float parryDelay;

    [SerializeField] [Tooltip("Length of time parry is active")]
    private float parryLength;

    [SerializeField] [Tooltip("Length of time after parry ceases to be active that the player cannot act")]
    private float parryCooldown;
    #endregion

    #region Properties
    public float Damage { get => parryDelay; }
    public float ChipDamagePercentage { get => chipDamagePercentage; }
    public float LifeStealPercentage { get => lifeStealPercentage; }
    public float PlayerMovementMultiplier { get => playerMovementMultiplier; }
    public float DamageModifier { get => damageModifier; }
    public float BlockedDamageModifier { get => blockedDamageModifier; }
    public float KnockbackTreshold { get => knockboackThreshold; }
    public float ParryDelay { get => parryDelay; }
    public float ParryLength { get => parryLength; }
    public float ParryCooldown { get => parryCooldown; }
    #endregion
}
