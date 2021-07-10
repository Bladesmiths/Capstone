using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class Player : MonoBehaviour
    {

        private FiniteStateMachine FSM;
        [SerializeField] private TransitionManager playerTransitionManager;

        public GameObject targetGO;
        void Awake()
        {
            //FSM = new FiniteStateMachine();
            //playerTransitionManager.player = this.gameObject;
            //playerTransitionManager.target = targetGO;
            //FSM.transitionManager = playerTransitionManager;

        }

        private void Update()
        {
            FSM.Tick();
        }

    }
}