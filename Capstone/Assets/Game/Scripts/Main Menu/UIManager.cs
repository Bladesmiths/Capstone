using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bladesmiths.Capstone.UI
{
    public class UIManager : MonoBehaviour
    {
        // Fields
        
        [TitleGroup("Player")] 
        [SerializeField] private Player player;
        
        [TitleGroup("HUD")]
        [HorizontalGroup("HUD/Split")]
        [VerticalGroup("HUD/Split/Left")] [BoxGroup("HUD/Split/Left/Health Bar")]
        [LabelWidth(85)]
        [SerializeField] private Image healthBarFill;
        
        [VerticalGroup("HUD/Split/Right")] [BoxGroup("HUD/Split/Right/Points UI")]
        [LabelWidth(70)]
        [SerializeField] private TextMeshProUGUI pointsText;

        [TitleGroup("Menus")] 
        [BoxGroup("Menus/Pause Menu")]
        [SerializeField] private GameObject pauseMenu;

        private bool isPaused;
        
        // Start is called before the first frame update
        void Start()
        {
            // Initialize variables
            isPaused = false;
            UpdateScore(0);
        }

        void LateUpdate()
        {
            if (player != null)
            {
                UpdateHealth(player.Health, player.MaxHealth);
                // UpdateScore(0);
            }
        }

        public void Pause()
        {
            isPaused = true;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }

        public void Unpause()
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
        
        
        // Private Methods
        
        private void UpdateHealth(float currentHealth, float maxHealth)
        {
            float fillPercentage = Mathf.Clamp(currentHealth / maxHealth, 0, 1);
            healthBarFill.fillAmount = fillPercentage;
        }

        private void UpdateScore(int currentScore)
        {
            pointsText.text = currentScore.ToString().Trim();
        }

    }
}

