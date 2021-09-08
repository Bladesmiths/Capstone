using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bladesmiths.Capstone
{
    public class WalkRunToggle : MonoBehaviour
    {
        // Controller reference. Will change once starter assets controller is integrated
        public ThirdPersonController characterController;

        // Is the character toggled to walk or run?
        private bool walking = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        // Detect input to toggle walking / running
        public void OnWalkToggle(InputValue value)
        {
            walking = !walking;
            //Debug.Log(walking);

            WalkToggle();
        }

        // Walk / run toggle logic independent from input
        public void WalkToggle()
        {
            //Set character speed based on toggle status
            if(walking)
            {
                characterController.MoveSpeedCurrentMax = characterController.WalkSpeed;
            }
            else
            {
                characterController.MoveSpeedCurrentMax = characterController.RunSpeed;
            }
        }
    }
}
