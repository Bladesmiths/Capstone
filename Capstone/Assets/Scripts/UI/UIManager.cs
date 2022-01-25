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

        #region Sword Select Fields
        [HorizontalGroup("HUD/SecondRow")]
        [BoxGroup("HUD/SecondRow/Sword Select")]
        [SerializeField]
        private GameObject swordSelectObject;

        [HorizontalGroup("HUD/SecondRow")]
        [BoxGroup("HUD/SecondRow/Sword Select")]
        [SerializeField] private float activeSize;
        [BoxGroup("HUD/SecondRow/Sword Select")]
        [SerializeField] private float inactiveSize;

        [HorizontalGroup("HUD/SecondRow")] [BoxGroup("HUD/SecondRow/Sword Select")] [LabelWidth(70)]
        [OdinSerialize]
        private Dictionary<Enums.SwordType, Sprite> inactiveGemSprites = new Dictionary<Enums.SwordType, Sprite>();
        [HorizontalGroup("HUD/SecondRow")] [BoxGroup("HUD/SecondRow/Sword Select")]
        [OdinSerialize]
        private Dictionary<Enums.SwordType, Sprite> activeGemSprites = new Dictionary<Enums.SwordType, Sprite>();
        [HorizontalGroup("HUD/SecondRow")] [BoxGroup("HUD/SecondRow/Sword Select")]
        [OdinSerialize]
        private Dictionary<Enums.SwordType, Image> gemImages = new Dictionary<Enums.SwordType, Image>();
        [HorizontalGroup("HUD/SecondRow")] [BoxGroup("HUD/SecondRow/Sword Select")]
        [OdinSerialize]
        private Dictionary<Enums.SwordType, Color> backgroundColors = new Dictionary<Enums.SwordType, Color>();
        [HorizontalGroup("HUD/SecondRow")] [BoxGroup("HUD/SecondRow/Sword Select")]
        [OdinSerialize]
        private Dictionary<Enums.SwordType, Image> backgroundImages = new Dictionary<Enums.SwordType, Image>();

        [HorizontalGroup("HUD/SecondRow")] [BoxGroup("HUD/SecondRow/Sword Select")]
        [SerializeField]
        private Enums.SwordType currentSwordSelect;
        #endregion

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

        #region Input Icon Dictionaries
        [TitleGroup("Input Icons")]

        //Dictionaries for referring to onboarding UI info

        [HorizontalGroup("Input Icons/FirstRow")]
        [BoxGroup("Input Icons/FirstRow/Xbox Inputs")] [LabelWidth(70)]
        [OdinSerialize]
        public Dictionary<string, Sprite> xboxInputs = new Dictionary<string, Sprite>();

        [HorizontalGroup("Input Icons/SecondRow")]
        [BoxGroup("Input Icons/SecondRow/PS4 Inputs")]
        [OdinSerialize]
        public Dictionary<string, Sprite> ps4Inputs = new Dictionary<string, Sprite>();

        [HorizontalGroup("Input Icons/ThirdRow")]
        [BoxGroup("Input Icons/ThirdRow/KBM Inputs")]
        [OdinSerialize] 
        public Dictionary<string, Sprite> kbmInputs = new Dictionary<string, Sprite>();
        #endregion

        public PlayerInput Inputs { get => playerInput; }

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

                if (swordSelectObject.activeInHierarchy)
                {
                    UpdateSwordSelect(player.Inputs.currentSwordType);
                }
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
            //This commented out code converts the player's raw health values into percentages so the UI can work with any max health value, not just 100.
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
            //If this happens, reduce the number of chipped chunks to make an even 100 total
            while (totalChunks > 100)
            {
                chipChunks -= 1;
                totalChunks = remainingChunks + chipChunks;
            }

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
        }

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

        /// <summary>
        /// Update the sword select
        /// Made to be called every frame it should be open
        /// </summary>
        /// <param name="currentSwordType"></param>
        private void UpdateSwordSelect(Enums.SwordType currentSwordType)
        {
            // Only update if the new sword type isn't the old one
            if (currentSwordSelect != currentSwordType)
            {
                // Set the background of the old one to white
                backgroundImages[currentSwordSelect].color = Color.white;

                // Set the background of the new one to its color
                backgroundImages[currentSwordType].color = backgroundColors[currentSwordType];

                // Update the field
                currentSwordSelect = currentSwordType;
            }
        }

        /// <summary>
        /// Toggle the radial menu on or off
        /// </summary>
        /// <param name="active">Whether the radial menu should be active or not0</param>
        public void ToggleRadialMenu(bool active)
        {
            // Only update appearance of sword select if it should be active
            if (active)
            {
                // Loop through all gem types
                foreach (Enums.SwordType swordType in gemImages.Keys)
                {
                    // If the sword type is the currently selected one
                    // Set it to the active values
                    if (swordType == currentSwordSelect)
                    {
                        gemImages[swordType].sprite = activeGemSprites[swordType];
                        gemImages[swordType].rectTransform.sizeDelta = new Vector2(activeSize, activeSize);
                        backgroundImages[swordType].color = backgroundColors[swordType];
                    }
                    // Otherwise set it to inactive values
                    else
                    {
                        gemImages[swordType].sprite = inactiveGemSprites[swordType];
                        gemImages[swordType].rectTransform.sizeDelta = new Vector2(inactiveSize, inactiveSize);
                        backgroundImages[swordType].color = Color.white;
                    }
                }
            }

            swordSelectObject.SetActive(active);
        }

        public void ToggleControlsMenu()
        {
            controlsMenu.SetActive(!controlsMenu.activeSelf);

            EventSystem.current.SetSelectedGameObject(moveRebindButton);
        }
    }
}

