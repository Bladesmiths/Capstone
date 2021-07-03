using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    [CreateAssetMenu(fileName = "PlayerTransition", menuName = "ScriptableObjects/PlayerTransition")]
    public class PlayerTransition : Transition
    {
        [EnumFlagAttribute] CharacterState playerCondiitons;
        public List<Func<bool>> Conditions { get; set; }
        public Dictionary<CharacterState, Func<bool>> playerConditionsRef;


        public PlayerTransition(IState to, IState from, Func<bool> condition)
        {
            To = to;
            From = from;
            Condition = condition;

        }

        private void Awake()
        {
            
        }



    }
}
