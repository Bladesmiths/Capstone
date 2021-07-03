using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bladesmiths.Capstone
{
    [CreateAssetMenu(fileName = "TransitionBase", menuName = "ScriptableObjects/Transition")]
    public abstract class Transition : ScriptableObject
    {
        public Func<bool> Condition { get; set; }
        public IState From { get; set; }
        public IState To { get; set; }




    }
}