using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Bladesmiths.Capstone
{
    public class IntroCutscene : MonoBehaviour
    {
        [SerializeField]
        private List<Sprite> introSlides = new List<Sprite>();
        [SerializeField]
        private int currentSlideIndex = 0;
        [SerializeField]
        private Image slideImage;
        [SerializeField]
        private float fadePause;
        [SerializeField]
        private float fadeSpeed;

        [Header("Special Cases")]
        [SerializeField]
        private float dramaticFadePause;
        [SerializeField] [Tooltip("This list should contain slides that a dramatic pause should happen before")]
        private List<int> whenToDramaticPause = new List<int>();

        private MainMenuFunctions mainMenu; 

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Fade(slideImage, 1));

            mainMenu = gameObject.GetComponent<MainMenuFunctions>(); 
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnAdvanceSlide(InputValue value)
        {
            if (!mainMenu.Paused && currentSlideIndex <= introSlides.Count - 1)
            {
                if (currentSlideIndex == introSlides.Count - 1)
                {
                    StartCoroutine(Fade(slideImage, 0));
                    currentSlideIndex++; 
                    return; 
                }
                StartCoroutine(FadeInOut());
            }
        }

        // src: https://stackoverflow.com/questions/64510141/using-a-coroutine-in-unity3d-to-fade-a-game-object-out-and-back-in-looping-inf
        IEnumerator FadeInOut()
        {
            // fade out
            yield return Fade(slideImage, 0);
            currentSlideIndex++; 
            slideImage.sprite = introSlides[currentSlideIndex];
            // wait
            yield return new WaitForSeconds(whenToDramaticPause.Contains(currentSlideIndex) ? dramaticFadePause : fadePause);
            // fade in
            yield return Fade(slideImage, 1);
        }

        IEnumerator Fade(Image image, float targetAlpha)
        {
            while (image.color.a != targetAlpha)
            {
                var newAlpha = Mathf.MoveTowards(image.color.a, targetAlpha, fadeSpeed * Time.deltaTime);
                image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
                yield return null;
            }

            if (currentSlideIndex == introSlides.Count)
            {
                mainMenu.EndReached(); 
            }
        }
    }
}