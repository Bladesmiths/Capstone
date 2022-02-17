namespace Bladesmiths.Capstone
{
    using System.Collections.Generic;
    using UnityEngine;

    public class VFXManager : MonoBehaviour
    {
        private List<ParticleSystem> particleSystems;
        public GameObject vfxObject;
        public float duration;

        private void Start()
        {
            particleSystems = new List<ParticleSystem>();

            if (vfxObject != null)
            {
                for (int i = 0; i < vfxObject.transform.childCount; i++)
                {
                    particleSystems.Add(vfxObject.transform.GetChild(i).GetComponent<ParticleSystem>());
                }
            }

            DisableVFX();

            //SetVFXDuration(duration);
        }

        /// <summary>
        /// Enables the VFX attached to this script.
        /// </summary>
        public void EnableVFX()
        {
            if (vfxObject.activeSelf)
            {
                return;
            }

            vfxObject.SetActive(true);
        }

        /// <summary>
        /// Disables the VFX attached to this script
        /// </summary>
        public void DisableVFX()
        {
            if (!vfxObject.activeSelf)
            {
                return;
            }

            vfxObject.SetActive(false);
        }

        /// <summary>
        /// Loops through each particle system and sets a new duration. Can NOT be executed during runtime.
        /// </summary>
        /// <param name="time">The animation duration.</param>
        private void SetVFXDuration(float time)
        {
            if (time > 0 && particleSystems.Count > 0)
            {
                foreach (ParticleSystem ps in particleSystems)
                {
                    //var mainModule = ps.main;
                    //mainModule.duration = time;
                }
            }
        }
    }
}