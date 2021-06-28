using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public abstract class FiniteStateMachine : MonoBehaviour
    {
        // Fields
        private IState currentState;
        protected Dictionary<int, IState> states = new Dictionary<int, IState>();


        // Methods
        public virtual void AddState(int key, IState newState)
        {
            states.Add(key, newState);

        }

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

        public virtual void Update()
        {
            if(currentState != null)
            {
                currentState.Tick();
            }

        }


    }
}
