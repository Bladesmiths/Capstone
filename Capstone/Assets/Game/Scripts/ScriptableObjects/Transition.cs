using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    
    public class Transition
    {
        // The conditions for going between the 'from' and 'to' state
        public Func<bool> Condition { get; set; }

        // The reference to the 'from' state
        public IState From { get; set; }

        // The reference to the 'to' state
        public IState To { get; set; }


        // The constructor for the Transitions class
        public Transition(IState to, IState from, Func<bool> condition)
        {
            To = to;
            From = from;
            Condition = condition;

        }

        // The constructor for the Transitions class
        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;

        }


    }
}