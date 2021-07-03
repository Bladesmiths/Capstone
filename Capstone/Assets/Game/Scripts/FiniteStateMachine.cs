using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Bladesmiths.Capstone
{
    public abstract class FiniteStateMachine : MonoBehaviour
    {
        // Fields
        private IState currentState;
        protected Dictionary<int, IState> states = new Dictionary<int, IState>();


        // Methods
        public virtual void SetCurrentState(IState state)
        {
            if (state == currentState)
            {
                return;
            }

            if (currentState != null)
            {
                currentState.OnExit();
            }

            currentState = state;

            currentState.OnEnter();


        }

        public virtual IState GetCurrentState()
        {
            return currentState;

        }

        public virtual IState GetState(int key)
        {
            return states[key];

        }

        public virtual void Tick()
        {
            if(currentState != null)
            {
                currentState.Tick();
            }

        }

        //public Transition GetTransition()
        //{


        //}

        public void AddAnyTransition(IState state, Func<bool> predicate)
        {


        }

        public virtual void AddTransition(IState from, IState to, Func<bool> predicate)
        {


        }


    }
}
