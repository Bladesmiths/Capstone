using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using TMPro;

namespace Bladesmiths.Capstone
{
    public class IntroCutscene : MonoBehaviour
    {
        // Slides Data
        // [TitleGroup("Slides Data")]
        // [SerializeField]
        // private List<Sprite> introSlides = new List<Sprite>();

        [InfoBox("fading variable sets if we are fading out to next the next slide.")]
        [TableList(ShowIndexLabels = true)]
        [SerializeField] private List<IntroSlideData> introSlideDataList;
        
        [SerializeField] private int currentSlideIndex = 0;
        [SerializeField] private Image slideBackground;

        [SerializeField] private TextMeshProUGUI slideText;

        [ColorPalette("TSS Color Palette")]
        [SerializeField] private Color boldSlideTextColor;
        
        // Fade Properties
        [TitleGroup("Fade Properties")]
        [SerializeField] private float fadePause;
        [SerializeField] private float fadeSpeed;

        // Special Cases
        [TitleGroup("Special Cases")]
        [SerializeField] private float dramaticFadePause;
        [SerializeField] [Tooltip("This list should contain slides that a dramatic pause should happen before")]
        private List<int> whenToDramaticPause = new List<int>();

        private MainMenuFunctions mainMenu;
        private bool isDuringFading = false;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Fade(slideBackground, 1));

            mainMenu = gameObject.GetComponent<MainMenuFunctions>();

            // Add color to bolded text
            string boldColorHex = ColorUtility.ToHtmlStringRGB(boldSlideTextColor);

            foreach (var t in introSlideDataList)
            {
                t.slideText = t.slideText.Replace("<b>", "<b><color=#" + boldColorHex + ">");
                t.slideText = t.slideText.Replace("</b>", "</color></b>");
            }

            UpdateSlideData();

            isDuringFading = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnAdvanceSlide(InputValue value)
        {
            if (isDuringFading) return;
            
            Debug.Log("Next Slide");
            bool fading = introSlideDataList[currentSlideIndex].fading;
            if (!mainMenu.Paused && currentSlideIndex <= introSlideDataList.Count - 1)
            {
                if (currentSlideIndex == introSlideDataList.Count - 1)
                {
                    StartCoroutine(Fade(slideBackground, 0));
                    currentSlideIndex++; 
                    return; 
                }

                if (fading)
                {
                    StartCoroutine(FadeInOut());
                }
                else
                {
                    currentSlideIndex++;
                    UpdateSlideData();
                }
            }
        }

        // src: https://stackoverflow.com/questions/64510141/using-a-coroutine-in-unity3d-to-fade-a-game-object-out-and-back-in-looping-inf
        IEnumerator FadeInOut()
        {
            isDuringFading = true;

            // fade out
            yield return Fade(slideBackground, 0);
            currentSlideIndex++;

            UpdateSlideData();
            
            // wait
            yield return new WaitForSeconds(whenToDramaticPause.Contains(currentSlideIndex) ? dramaticFadePause : fadePause);
            // fade in
            yield return Fade(slideBackground, 1);
            
            isDuringFading = false;
        }

        IEnumerator Fade(Image image, float targetAlpha)
        {
            while (Math.Abs(image.color.a - targetAlpha) > Mathf.Epsilon)
            {
                var newAlpha = Mathf.MoveTowards(image.color.a, targetAlpha, fadeSpeed * Time.deltaTime);
                image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
                yield return null;
            }
            
            image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);

            if (currentSlideIndex >= introSlideDataList.Count)
            {
                mainMenu.EndReached(); 
            }
        }
        
        
        // Private Methods
        private void UpdateSlideData()
        {
            // Set new slide data
            slideBackground.sprite = introSlideDataList[currentSlideIndex].background;
            slideText.text = introSlideDataList[currentSlideIndex].slideText;
        }
    }
    
    [Serializable]
    public class IntroSlideData
    {
        [TableColumnWidth(60, Resizable = false)]
        public bool fading;
        
        [TableColumnWidth(90, Resizable = false)]
        [PreviewField(ObjectFieldAlignment.Center)]
        public Sprite background;

        [MultiLineProperty(3)]
        public string slideText;
    }
}