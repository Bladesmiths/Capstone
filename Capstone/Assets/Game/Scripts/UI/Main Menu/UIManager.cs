using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Bladesmiths.Capstone.UI
{
    public class UIManager : MonoBehaviour
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
        [LabelWidth(85)]
        [SerializeField] private Image chipDamageFill;

        [VerticalGroup("HUD/Split/Right")] [BoxGroup("HUD/Split/Right/Points UI")]
        [LabelWidth(70)]
        [SerializeField] private TextMeshProUGUI pointsText;

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
                UpdateScore(0, player.MaxPoints);
        }

        void LateUpdate()
        {
            if (player != null)
            {
                UpdateHealth(player.Health, player.CurrentChipDamage, player.MaxHealth);
                UpdateScore(player.Points, player.MaxPoints);
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

    }
}

