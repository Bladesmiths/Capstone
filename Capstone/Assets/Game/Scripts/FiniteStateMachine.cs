using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Bladesmiths.Capstone
{
    public class FiniteStateMachine
    {
        // Fields
        protected IState currentState;
        private Dictionary<Type, List<Transition>> filteredPlayerTransitions = new Dictionary<Type, List<Transition>>();
        private List<Transition> anyTransitions = new List<Transition>();
        private List<Transition> currentTransitions = new List<Transition>();

        private TransitionManager transitionManagerFSM;
        private static List<Transition> emptyTransitions = new List<Transition>(0);


        public FiniteStateMachine(TransitionManager transitionManager)
        {
            transitionManagerFSM = transitionManager;
            filteredPlayerTransitions = transitionManager.conditionsRef;
            SetCurrentState(transitionManager.cState);

        }

        // Methods
        public void SetCurrentState(IState state)
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

            filteredPlayerTransitions.TryGetValue(currentState.GetType(), out currentTransitions);
            if (currentTransitions == null)
            {
                currentTransitions = emptyTransitions;
            }

            currentState.OnEnter();


        }

        public IState GetCurrentState()
        {
            return currentState;

        }

        public void Tick()
        {
            Debug.Log(filteredPlayerTransitions.Values);
            Transition transition = GetTransition();
            if (transition != null)
                SetCurrentState(transition.To);

            if (currentState != null)
            {
                currentState.Tick();
            }

        }

        public Transition GetTransition()
        {
            foreach (Transition transition in anyTransitions)
                if (transition.Condition())
                    return transition;

            foreach (Transition transition in currentTransitions)
                if (transition.Condition())
                    return transition;

            return null;

        }


        //public virtual void AddAnyTransition(IState state, Func<bool> predicate)
        //{
        //    anyTransitions.Add(new Transition(state, predicate));

        //}

        public void AddTransition(IState from, IState to, Func<bool> predicate)
        {
            if (filteredPlayerTransitions.TryGetValue(from.GetType(), out List<Transition> transitions) == false)
            {
                transitions = new List<Transition>();
                filteredPlayerTransitions[from.GetType()] = transitions;
            }

            transitions.Add(new Transition(to, from, predicate));

        }

        public void GetConditions(TransitionManager manager)
        {
            filteredPlayerTransitions = manager.conditionsRef;

        }


    }
}
