using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Bladesmiths.Capstone.Enums
{
    [Flags]
    public enum SwordType
    {
        Base,
        Sword1,
        Sword2,
        Sword3,
        Sword4
    }

    [Flags]
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

    [Flags]
    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
        TakingDamage,
        Dead
    }
}
