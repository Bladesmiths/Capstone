using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    
    public class Transition
    {
        [SerializeField] public Func<bool> condition;
        [SerializeField] public IState from;
        [SerializeField] public IState to;


        //public Func<bool> Condition { get { return condition; } set { condition = value; } }
        //public IState From { get { return from; } set { from = value; } }
        //public IState To { get { return to; } set { to = value; } }


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