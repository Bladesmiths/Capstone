using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    //[CreateAssetMenu(fileName = "PlayerTransition", menuName = "ScriptableObjects/PlayerTransition")]
    public class PlayerTransition : Transition
    {
        public PlayerTransition(IState to, IState from, Func<bool> condition)
            : base (to, from, condition)
        {
            To = to;
            From = from;
            Condition = condition;

        }

       



    }
}
