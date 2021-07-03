using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Bladesmiths.Capstone
{
    public class PlayerTransition : Transition
    {
        //[EnumFlagsAttribute] PlayerCondiiton playerCondiitons;
        public List<Func<bool>> Conditions { get; set; }
        //public Dictionary<PlayerCondition, Func<bool>> playerConditionsRef;


        private void Awake()
        {
            
        }



    }
}
