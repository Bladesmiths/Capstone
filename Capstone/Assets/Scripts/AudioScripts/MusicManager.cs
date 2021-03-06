using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

namespace Bladesmiths.Capstone
{
    public class MusicManager : MonoBehaviour
    {
        private FMOD.Studio.EventInstance instance;

        [SerializeField]
        [Range(0f, 1f)]
        private float reverb, delay;

        [SerializeField]
        [Range(0f, 5000f)]
        private float delayTime;

        [SerializeField]
        [Range(-12f, 12f)]
        private float pitch;

        private bool CanFade;

        [SerializeField]
        [Range(0f, 1f)]
        private float Threat;

        void Start()
        {
            instance = GetComponent<StudioEventEmitter>().EventInstance;
            instance.start();
            CanFade = true;
        }

        void Update()
        {
            //instance.setParameterByName("Reverb", reverb);
            //instance.setParameterByName("Delay", delay);
            //instance.setParameterByName("DelayTime", delayTime);
            //instance.setParameterByName("Pitch", pitch);
            instance.getParameterByName("Threat", out float temp);
            Threat = temp;

            if (Vector3.Distance(Boss.instance.transform.position, Player.instance.transform.position) <= 50f)
            {
                StartCoroutine(FadeInMusic(temp));
            }
            else
            {
                if (AIDirector.Instance.GetClosestEnemyDist() <= 7.5f && CanFade && temp != 1)
                {
                    CanFade = false;
                    StartCoroutine(FadeInMusic(temp));
                }
                else if (AIDirector.Instance.GetClosestEnemyDist() > 7.5f && CanFade && temp != 0)
                {
                    CanFade = false;
                    StartCoroutine(FadeOut(temp));
                }
            }
        }

        /// <summary>
        /// Fades in the music when close to an Enemy
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public IEnumerator FadeInMusic(float level)
        {
            StopCoroutine(FadeOut(level));
            
            while(level < 1)
            {
                instance.getParameterByName("Threat", out level);
                Threat = level;
                level += Time.deltaTime;
                instance.setParameterByName("Threat", level);

                yield return new WaitForSeconds(Time.deltaTime);
            }
            instance.setParameterByName("Threat", 1f);

            CanFade = true;
        }

        /// <summary>
        /// Fades out the music when far from an Enemy
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public IEnumerator FadeOut(float level)
        {
            StopCoroutine(FadeInMusic(level));
            while (level > 0)
            {
                instance.getParameterByName("Threat", out level);
                Threat = level;
                level -= Time.deltaTime;
                instance.setParameterByName("Threat", level);

                yield return new WaitForSeconds(Time.deltaTime);
            }
            instance.setParameterByName("Threat", 0f);

            CanFade = true;
        }
    }
}
