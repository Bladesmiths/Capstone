using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bladesmiths.Capstone
{
    //[CreateAssetMenu(fileName = "TransitionBase", menuName = "ScriptableObjects/Transition")]
    public class Transition
    {
        public Func<bool> Condition { get; set; }
        public IState From { get; set; }
        public IState To { get; set; }

        public Transition(IState to, IState from, Func<bool> condition)
        {
            To = to;
            From = from;
            Condition = condition;

        }


    }
}