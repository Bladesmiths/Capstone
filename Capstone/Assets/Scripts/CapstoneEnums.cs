using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Bladesmiths.Capstone.Enums
{
    public enum Team
    {
        NeutralObject,
        Player, 
        Enemy
    }
    public enum SwordType
    {
        Topaz, 
        Ruby, 
        Sapphire
    }

    [Flags]
    public enum PlayerCondition
    {
        F_Idle = 1,
        F_Moving = 2,
        F_Attacking = 4,
        F_Blocking = 8,
        F_ParryAttempt = 16,
        F_Dodging = 32,
        F_Jumping = 64,
        F_Falling = 128,
        F_TakingDamage = 256,
        F_SwitchingWeapon = 512,
        F_Dead = 1024,
        F_ParrySuccess = 2048,
        F_Dashing = 4096, 
        F_Dying = 8192
    }

    [Flags]
    public enum EnemyCondition
    {
        Idle = 1,
        Moving = 2,
        Attacking = 4,
        TakingDamage = 8,
        Dead = 16
    }

    public enum CharacterState
    {
        Idle,
        Moving,
        Attacking,
        Blocking,
        Parrying,
        Dodging,
        Jumping,
        Falling,
        TakingDamage,
        SwitchingWeapon,
        Dead
    }

    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
        TakingDamage,
        Dead
    }
}
