using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;
using Bladesmiths.Capstone.Testing;
using StarterAssets;


namespace Bladesmiths.Capstone
{    
    public class Enemy_Attack : StateMachineBehaviour
    {
        private Enemy enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy = animator.gameObject.GetComponent<Enemy>();
            enemy.axeCollider.enabled = true;

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.axeCollider.enabled = true;

        }
    }








    
}
