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
        [SerializeField] private Cinemachine.CinemachineFreeLook camera;
        private float maxSpeedX;
        private float maxSpeedY;

        [SerializeField] private PlayerInput playerInput;
        
        [TitleGroup("HUD")]
        [SerializeField] private TextMeshProUGUI pointsText;

        [HorizontalGroup("HUD/FirstRow")]
        [BoxGroup("HUD/FirstRow/Health Bar Objects")]

        //Health chunk objects
        [OdinSerialize]
        private List<GameObject> healthBarObjects = new List<GameObject>();

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
        private GameObject swordSelectMask; 
        [SerializeField]
        private Enums.SwordType currentSwordSelect; 

        [TitleGroup("Menus")] 
        [BoxGroup("Menus/Pause Menu")]
        [SerializeField] private GameObject pauseMenu;
        
        [BoxGroup("Menus/Pause Menu")]
        [SerializeField] private GameObject resumeButton;

        [BoxGroup("Menus/Pause Menu")]
        [SerializeField] private bool isPaused;

        //Health chunk counts from last health UI update
        private int prevHealthChunks = 100;
        private int prevChipChunks = 0;

        [SerializeField] private GameObject controlsMenu;
        [SerializeField] private GameObject controlsButton;

        [SerializeField] private GameObject moveRebindButton;

        // Start is called before the first frame update
        void Start()
        {
            // Initialize variables
            isPaused = false;
            if (player != null)
            {
                UpdateScore(0, player.MaxPoints);

                maxSpeedX = camera.m_XAxis.m_MaxSpeed;
                maxSpeedY = camera.m_YAxis.m_MaxSpeed;

                UpdateSwordSelect(player.Inputs.currentSwordType);
            }

            //By default, the health bar is 100 objects ordered from tip to base. 
            //Ex: When the player takes 1 damage, going from 100 to 99 health, the chunk at index 0 shatters.
            //This is really confusing and should probably be changed in the source PSB file, but for now we reverse the list.
            healthBarObjects.Reverse();
        }

        void LateUpdate()
        {
            if (player != null)
            {
                UpdateHealth(player.Health, player.ChipDamageTotal, player.MaxHealth);
                UpdateScore(player.Points, player.MaxPoints);
                UpdateSwordSelect(player.Inputs.currentSwordType);
            }
        }

        public void Unpause()
        {
            if (isPaused)
            {
                isPaused = false;
                controlsMenu.SetActive(false);
                playerInput.SwitchCurrentActionMap("Player");
                Debug.Log("Current Action Map: " + playerInput.currentActionMap);

                pauseMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;

                // Allows the camera fto move and recenter
                camera.m_XAxis.m_MaxSpeed = maxSpeedX;
                camera.m_YAxis.m_MaxSpeed = maxSpeedY;
                camera.m_RecenterToTargetHeading.m_enabled = true;
                camera.m_YAxisRecentering.m_enabled = true;

                Time.timeScale = 1;
            }
        }

        // Private Methods
        public void Pause()
        {
            if (!isPaused)
            {
                isPaused = true;
                playerInput.SwitchCurrentActionMap("UI");
                EventSystem.current.SetSelectedGameObject(resumeButton);
                Debug.Log("Current Action Map: " + playerInput.currentActionMap);

                // Stops the camera from moving and stops the recentering
                camera.m_XAxis.m_MaxSpeed = 0f;
                camera.m_YAxis.m_MaxSpeed = 0f;
                camera.m_RecenterToTargetHeading.m_enabled = false;
                camera.m_YAxisRecentering.m_enabled = false;

                //Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;

                EventSystem.current.SetSelectedGameObject(controlsButton);

                pauseMenu.SetActive(true);
            }
        }
        
        /// <summary>
        /// Update health bar UI to reflect state of player's health
        /// </summary>
        /// <param name="currentHealth"></param>
        /// <param name="currentChipDamage"></param>
        /// <param name="maxHealth"></param>
        private void UpdateHealth(float currentHealth, float currentChipDamage, float maxHealth)
        {
            //This code converts the player's raw health values into percentages so the UI can work with any max health value, not just 100.
            //There are currently some rounding issues where a single chunk will be added to the health bar seemingly at random.
            //For now the raw health values are being used directly because the player has 100 health and the UI has 100 chunks.
            //float healthAndChipPercentage = (currentHealth + currentChipDamage) / maxHealth;
            //float currentHealthPercentage = currentHealth / maxHealth;
            //float chipHealthPercentage = healthAndChipPercentage - currentHealthPercentage;

            //Determine how many chunks remain in the health bar after taking damage
            //int remainingChunks = (int)(currentHealthPercentage * healthBarObjects.Count);
            //int chipChunks = (int)(chipHealthPercentage * healthBarObjects.Count); 

            int remainingChunks = (int)currentHealth > 100 ? 100 : (int)currentHealth;
            int chipChunks = (int)currentChipDamage;

            int totalChunks = remainingChunks + chipChunks;

            //In specific situations involving lifesteal + chip damage, health briefly exceeds 100 and causes errors
            while (totalChunks > 100)
            {
                chipChunks -= 1;
                totalChunks = remainingChunks + chipChunks;
            }

            Debug.Log("Player Health: " + remainingChunks);
            Debug.Log("Player Chip Health " + chipChunks);

            //Modify chunk status (the order of these matters)
            ShatterChunks(remainingChunks, chipChunks);
            ChipChunks(remainingChunks, chipChunks);
            HealChunks(remainingChunks, chipChunks);
            UnChipChunks(remainingChunks, chipChunks);

            //Save values for future comparison
            prevHealthChunks = remainingChunks;
            prevChipChunks = chipChunks;
        }

        //Shatter any health chunks that have an index higher than the remaining chunk count
        private void ShatterChunks(int healthChunks, int chipChunks)
        {
            for (int i = healthChunks + chipChunks; i < healthBarObjects.Count; i++)
            {
                healthBarObjects[i].GetComponent<HealthChunk>().Shatter();
            }
        }

        //Chip any health chunks that have an index higher than the remaining chunk count
        private void ChipChunks(int healthChunks, int chipChunks)
        {
            for (int i = healthChunks; i < healthChunks + chipChunks; i++)
            {
                healthBarObjects[i].GetComponent<HealthChunk>().Chip();
            }
        } //BUG HERE when blocking a hit then lifestealing. Health + chipchunks goes over 100? @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        //Return chipped health chunks to their normal appearance
        //This behavior applies when successfully parrying
        private void UnChipChunks(int healthChunks, int chipChunks)
        {
            for (int i = 0; i < healthChunks; i++)
            {
                healthBarObjects[i].GetComponent<HealthChunk>().UnChip();
            }
        }

        //Restore health
        //This behavior applies when lifestealing
        //This means invisible chunks need to be made visible
        private void HealChunks(int healthChunks, int chipChunks)
        {
            for (int i = prevHealthChunks; i < healthChunks; i++)
            {
                healthBarObjects[i].GetComponent<HealthChunk>().Restore();
            }
        }

        //Reset all chunks to original positions / sizes (usually when player respawns)
        public void ResetChunks()
        {
            foreach (GameObject chunk in healthBarObjects)
            {
                chunk.GetComponent<HealthChunk>().FullReset();
            }
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

        public void SetMaskActive(bool active)
        {
            swordSelectMask.SetActive(active);
        }

        public void ToggleControlsMenu()
        {
            controlsMenu.SetActive(!controlsMenu.activeSelf);

            EventSystem.current.SetSelectedGameObject(moveRebindButton);
        }
    }
}

