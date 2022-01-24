using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class CallOneShot : MonoBehaviour
    {
        public FMODUnity.EventReference swordMiss;
        public FMODUnity.EventReference footstep;


        public void OneShot_SwordMiss()
        {
            FMODUnity.RuntimeManager.PlayOneShot(swordMiss);

        }

        public void OneShot_Footstep()
        {
            if (gameObject.GetComponent<Player>().Inputs.move != Vector2.zero)
            {
                FMODUnity.RuntimeManager.PlayOneShot(footstep);
            }
        }
    }
}
