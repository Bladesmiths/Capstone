using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CapstoneEnums
{
    public enum SwordType
    {
        Base,
        Sword1,
        Sword2,
        Sword3,
        Sword4
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
