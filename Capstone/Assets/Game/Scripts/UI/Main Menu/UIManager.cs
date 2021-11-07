using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.Serialization;

namespace Bladesmiths.Capstone.UI
{
    public class UIManager : SerializedMonoBehaviour
    {
        // Fields
        
        [TitleGroup("Player")] 
        [SerializeField] private Player player;

        [SerializeField] private PlayerInput playerInput;
        
        [TitleGroup("HUD")]
        [HorizontalGroup("HUD/Split")]
        [VerticalGroup("HUD/Split/Left")] [BoxGroup("HUD/Split/Left/Health Bar")]
        [LabelWidth(85)]
        [SerializeField] private Image healthBarFill;
        [VerticalGroup("HUD/Split/Middle")] [BoxGroup("HUD/Split/Middle/Chip Damage Bar")]
        [LabelWidth(85)]
        [SerializeField] private Image chipDamageFill;

        [VerticalGroup("HUD/Split/Right")] [BoxGroup("HUD/Split/Right/Points UI")]
        [LabelWidth(85)]
        [SerializeField] private TextMeshProUGUI pointsText;

        [HorizontalGroup("HUD/SecondRow")]
        [VerticalGroup("HUD/SecondRow/Left")]
        [BoxGroup("HUD/SecondRow/Left/Gem Sizes")]
        [SerializeField] private float activeSize;
        [BoxGroup("HUD/SecondRow/Left/Gem Sizes")]
        [SerializeField] private float inactiveSize;

        [HorizontalGroup("HUD/ThirdRow")] [BoxGroup("HUD/ThirdRow/Gem Images")] [LabelWidth(70)]
        [OdinSerialize]
        private Dictionary<Enums.SwordType, Sprite> inactiveGemSprites = new Dictionary<Enums.SwordType, Sprite>();
        [HorizontalGroup("HUD/ThirdRow")] [BoxGroup("HUD/ThirdRow/Gem Images")]
        [OdinSerialize]
        private Dictionary<Enums.SwordType, Sprite> activeGemSprites = new Dictionary<Enums.SwordType, Sprite>();
        [HorizontalGroup("HUD/ThirdRow")] [BoxGroup("HUD/ThirdRow/Gem Images")]
        [OdinSerialize]
        private Dictionary<Enums.SwordType, Image> gemImages = new Dictionary<Enums.SwordType, Image>();

        [SerializeField]
        private Enums.SwordType currentSwordSelect; 

        [TitleGroup("Menus")] 
        [BoxGroup("Menus/Pause Menu")]
        [SerializeField] private GameObject pauseMenu;
        
        [BoxGroup("Menus/Pause Menu")]
        [SerializeField] private GameObject resumeButton;

        [BoxGroup("Menus/Pause Menu")]
        [SerializeField] private bool isPaused;
        
        // Start is called before the first frame update
        void Start()
        {
            // Initialize variables
            isPaused = false;
            if (player != null)
            {
                UpdateScore(0, player.MaxPoints);
                UpdateSwordSelect(player.Inputs.currentSwordType);
            }
        }

        void LateUpdate()
        {
            if (player != null)
            {
                UpdateHealth(player.Health, player.CurrentChipDamage, player.MaxHealth);
                UpdateScore(player.Points, player.MaxPoints);
                UpdateSwordSelect(player.Inputs.currentSwordType);
            }
        }

        public void Unpause()
        {
            if (isPaused)
            {
                isPaused = false;
                playerInput.SwitchCurrentActionMap("Player");
                Debug.Log("Current Action Map: " + playerInput.currentActionMap);

                pauseMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                
                Time.timeScale = 1;
            }
        }

        public void OnPause(InputValue value)
        {
            if (value.isPressed)
                Pause();
        }

        public void OnUnpause(InputValue value)
        {
            if (value.isPressed)
                Unpause();
        }
        
        // Private Methods
        private void Pause()
        {
            if (!isPaused)
            {
                isPaused = true;
                playerInput.SwitchCurrentActionMap("UI");
                EventSystem.current.SetSelectedGameObject(resumeButton);
                Debug.Log("Current Action Map: " + playerInput.currentActionMap);
                
                //Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                
                pauseMenu.SetActive(true);
            }
        }
        
        private void UpdateHealth(float currentHealth, float currentChipDamage, float maxHealth)
        {
            float fillPercentage = Mathf.Clamp(currentHealth / maxHealth, 0, 1);
            healthBarFill.fillAmount = fillPercentage;

            fillPercentage = Mathf.Clamp((currentHealth + currentChipDamage) / maxHealth, 0, 1);
            chipDamageFill.fillAmount = fillPercentage; 
        }

        private void UpdateScore(int currentScore, int maxScore)
        {
            string displayScoreText = currentScore.ToString() + "/" + maxScore.ToString();
            pointsText.text = displayScoreText.Trim();
        }

        private void UpdateSwordSelect(Enums.SwordType currentSwordType)
        {
            if (currentSwordSelect != currentSwordType)
            {
                gemImages[currentSwordSelect].sprite = inactiveGemSprites[currentSwordSelect];
                gemImages[currentSwordSelect].rectTransform.sizeDelta = new Vector2(inactiveSize, inactiveSize);

                gemImages[currentSwordType].sprite = activeGemSprites[currentSwordType];
                gemImages[currentSwordType].rectTransform.sizeDelta = new Vector2(activeSize, activeSize); 

                currentSwordSelect = currentSwordType;
            }
        }

    }
}

