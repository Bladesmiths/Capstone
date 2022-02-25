using DG.Tweening;

namespace Bladesmiths.Capstone
{
    using System.Collections.Generic;
    using UnityEngine;
    using DG;

    public class VFXManager : MonoBehaviour
    {
        private List<ParticleSystem> particleSystems;

        [Tooltip("The VFX object to manipulate")]
        public GameObject vfxObject;

        [Tooltip("The VFX object to spawn upon collision")]
        public GameObject collisionVFXObject;

        [Tooltip("The emitter duration. Set this to the length of the animation.")]
        public float emitterDuration;

        [Tooltip("The effect duration. Set this to the longest particle lifetime.")]
        public float effectDuration;

        public GameObject bladeObject;
        public float defaultBladeEmissionLevel;
        private Material bladeMaterial;

        private float targetEmissionLevel = 2f;

        private float emissionRiseDuration = 1f;
        private float emissionDropDuration = 0.7f;

        private void Awake()
        {
            //SetEmitterDuration(emitterDuration);
        }

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

            if (bladeObject != null)
            {
                //defaultBladeEmissionLevel = bladeObject.material.GetFloat("_Emission");
                //defaultBladeEmissionLevel = bladeObject.GetComponent<Renderer>().material.GetFloat("_EmissiveIntensity");
                bladeMaterial = bladeObject.GetComponent<Renderer>().material;
                defaultBladeEmissionLevel = bladeMaterial.GetFloat("_EmissiveIntensity");
            }

            DisableVFX();
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
            EnableBladeEmission(bladeMaterial, emissionRiseDuration);
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
            DisableBladeEmission(bladeMaterial, emissionDropDuration);
        }

        public void PlayCollisionOneShotVFX(float duration, Vector3 location, Quaternion rotation)
        {
            if (collisionVFXObject != null)
                PlayOneShotVFX(collisionVFXObject, duration, location, rotation);
        }

        /// <summary>
        /// Plays the VFX as a One-shot.
        /// </summary>
        /// <param name="duration">The duration of the effect. (When to destroy)</param>
        /// <param name="location">The spawn location.</param>
        /// <param name="rotation">The spawn rotation.</param>
        public void PlayOneShotVFX(GameObject vfxObject, float duration, Vector3 location, Quaternion rotation)
        {
            GameObject g = Instantiate(vfxObject, location, rotation);
            Destroy(g, duration);
        }

        /// <summary>
        /// Loops through each particle system and sets a new duration. Can NOT be executed during runtime.
        /// </summary>
        /// <param name="time">The animation duration.</param>
        private void SetEmitterDuration(float time)
        {
            if (time > 0 && particleSystems.Count > 0)
            {
                foreach (ParticleSystem ps in particleSystems)
                {
                    var mainModule = ps.main;
                    mainModule.duration = time;
                }
            }
        }

        public void EnableBladeEmission(Material bladeMaterial, float duration)
        {
            if (bladeMaterial == null)
            {
                return;
            }

            float lerp = duration;
            //bladeMaterial.SetFloat("_EmissiveIntensity", Mathf.SmoothStep(defaultBladeEmissionLevel, 2f, duration));
            bladeMaterial.DOFloat(targetEmissionLevel, "_EmissiveIntensity", duration);
        }

        public void DisableBladeEmission(Material bladeMaterial, float duration)
        {
            if (bladeMaterial == null)
            {
                return;
            }

            //bladeMaterial.SetFloat("_EmissiveIntensity", Mathf.SmoothStep(2f, defaultBladeEmissionLevel, duration));
            bladeMaterial.DOFloat(defaultBladeEmissionLevel, "_EmissiveIntensity", duration);
        }
    }
}