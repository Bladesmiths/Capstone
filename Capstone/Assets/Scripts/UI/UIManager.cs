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
using UnityEngine.SceneManagement;

namespace Bladesmiths.Capstone.UI
{
    public class UIManager : SerializedMonoBehaviour
    {
        // Fields

        [TitleGroup("Player")]
        [SerializeField] private Player player;
        [SerializeField] float playerPrevHealth;
        [SerializeField] private Cinemachine.CinemachineFreeLook camera;
        private float maxSpeedX;
        private float maxSpeedY;

        private Boss boss;
        float bossPrevHealth;

        [SerializeField] private PlayerInput playerInput;

        [TitleGroup("HUD")]
        [SerializeField] private TextMeshProUGUI pointsText;

        [HorizontalGroup("HUD/FirstRow")]
        [BoxGroup("HUD/FirstRow/Health Bar Objects")]

        //Health chunk objects
        [OdinSerialize]
        private List<GameObject> playerHealthBarObjects = new List<GameObject>();

        [HorizontalGroup("HUD/FirstRow")]
        [BoxGroup("HUD/FirstRow/Health Bar Objects")]
        [OdinSerialize]
        private List<GameObject> bossHealthBarObjects = new List<GameObject>();

        #region Sword Select Fields
        [HorizontalGroup("HUD/SecondRow")]
        [BoxGroup("HUD/SecondRow/Sword Select")]
        [SerializeField]
        private GameObject swordSelectObject;
        [BoxGroup("HUD/SecondRow/Sword Select")]
        [SerializeField]
        private GameObject swordSelectTwoSwordsObject;

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
        private Dictionary<Enums.SwordType, Image> gemImagesTwoSwords = new Dictionary<Enums.SwordType, Image>();
        [HorizontalGroup("HUD/SecondRow")] [BoxGroup("HUD/SecondRow/Sword Select")]
        [OdinSerialize]
        private Dictionary<Enums.SwordType, Color> backgroundColors = new Dictionary<Enums.SwordType, Color>();
        [HorizontalGroup("HUD/SecondRow")] [BoxGroup("HUD/SecondRow/Sword Select")]
        [OdinSerialize]
        private Dictionary<Enums.SwordType, Image> backgroundImages = new Dictionary<Enums.SwordType, Image>();
        [HorizontalGroup("HUD/SecondRow")] [BoxGroup("HUD/SecondRow/Sword Select")]
        [OdinSerialize]
        private Dictionary<Enums.SwordType, Image> backgroundImagesTwoSwords = new Dictionary<Enums.SwordType, Image>();

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
        private int prevPlayerHealthChunks = 100;
        private int prevPlayerChipChunks = 0;

        private int prevBossHealthChunks;

        [SerializeField] private GameObject settingsButton;
        [SerializeField] private SettingsManager settingsManager;
        [SerializeField] private GameObject bossHealthBar;

        public float MaxSpeedX
        {
            get { return maxSpeedX; }
            set { maxSpeedX = value; }
        }

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
            //This is really confusing and would ideally be changed in the source PSB file,
            //But the PSB importer seems to remember layer orders and never let go. For now, we reverse the list.
            playerHealthBarObjects.Reverse();
            bossHealthBarObjects.Reverse();

            boss = GameObject.Find("Boss").GetComponent<Boss>();
        }

        void LateUpdate()
        {
            if (player != null)
            {
                //Update player health bar when their health changes
                if (playerPrevHealth != player.Health)
                {
                    UpdatePlayerHealthBar(player.Health, player.ChipDamageTotal, player.MaxHealth);
                }
                UpdateScore(player.Points, player.MaxPoints);

                if (swordSelectTwoSwordsObject.activeInHierarchy)
                {
                    UpdateSwordSelect(player.Inputs.currentSwordType);
                }
                else if (swordSelectObject.activeInHierarchy)
                {
                    UpdateSwordSelect(player.Inputs.currentSwordType);
                }
            }

            //Update boss health bar when their health changes
            if (boss != null && bossHealthBar.activeSelf && bossPrevHealth != boss.Health)
            {
                UpdateBossHealthBar(boss.Health, boss.MaxHealth);
            }
        }

        public void Unpause()
        {
            if (isPaused)
            {
                if (settingsManager.onPauseScreenOnly)
                {
                    isPaused = false;

                    settingsManager.UnPause();
                    resumeButton.SetActive(true);

                    playerInput.SwitchCurrentActionMap("Player");
                    //Debug.Log("Current Action Map: " + playerInput.currentActionMap);

                    pauseMenu.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;

                    // Allows the camera fto move and recenter
                    camera.m_XAxis.m_MaxSpeed = maxSpeedX;
                    camera.m_YAxis.m_MaxSpeed = maxSpeedY;
                    camera.m_RecenterToTargetHeading.m_enabled = true;
                    camera.m_YAxisRecentering.m_enabled = true;

                    Time.timeScale = 1;
                }
                else
                {
                    settingsManager.CloseActiveSettingsPanel();
                }
            }
        }

        // Private Methods
        public void Pause()
        {
            if (!isPaused)
            {
                isPaused = true;
                playerInput.SwitchCurrentActionMap("UI");
                //EventSystem.current.SetSelectedGameObject(resumeButton);
                //Debug.Log("Current Action Map: " + playerInput.currentActionMap);

                // Stops the camera from moving and stops the recentering
                camera.m_XAxis.m_MaxSpeed = 0f;
                camera.m_YAxis.m_MaxSpeed = 0f;
                camera.m_RecenterToTargetHeading.m_enabled = false;
                camera.m_YAxisRecentering.m_enabled = false;

                //Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;

                pauseMenu.SetActive(true);

                EventSystem.current.SetSelectedGameObject(settingsButton);
                settingsManager.onPauseScreenOnly = true;
            }
        }
        
        /// <summary>
        /// Update player health bar UI to reflect state of player's health
        /// </summary>
        /// <param name="currentHealth"></param>
        /// <param name="currentChipDamage"></param>
        /// <param name="maxHealth"></param>
        private void UpdatePlayerHealthBar(float currentHealth, float currentChipDamage, float maxHealth)
        {
            playerPrevHealth = currentHealth;
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
            ShatterChunks(remainingChunks, chipChunks, playerHealthBarObjects);
            ChipChunks(remainingChunks, chipChunks, playerHealthBarObjects);
            HealChunks(remainingChunks, chipChunks, playerHealthBarObjects, prevPlayerHealthChunks);
            UnChipChunks(remainingChunks, chipChunks, playerHealthBarObjects);

            //Save values for future comparison
            prevPlayerHealthChunks = remainingChunks;
            prevPlayerChipChunks = chipChunks;
        }

        /// <summary>
        /// Update boss health bar UI to reflect state of boss's health
        /// The boss doesn't regain or chip health chunks, so its logic is simpler than the player's
        /// </summary>
        /// <param name="currentHealth"></param>
        /// <param name="maxHealth"></param>
        public void UpdateBossHealthBar(float currentHealth, float maxHealth)
        {
            bossPrevHealth = currentHealth;
            //This commented out code converts the player's raw health values into percentages so the UI can work with any max health value, not just 100.
            //For now the raw health values are being used directly because the player has 100 health and the UI has 100 chunks.

            //float healthAndChipPercentage = (currentHealth + currentChipDamage) / maxHealth;
            //float currentHealthPercentage = currentHealth / maxHealth;
            //float chipHealthPercentage = healthAndChipPercentage - currentHealthPercentage;

            //Determine how many chunks remain in the health bar after taking damage
            //int remainingChunks = (int)(currentHealthPercentage * healthBarObjects.Count);
            //int chipChunks = (int)(chipHealthPercentage * healthBarObjects.Count); 

            int remainingChunks = (int)(currentHealth / (maxHealth / bossHealthBarObjects.Count));

            int totalChunks = remainingChunks;

            //Modify chunk status
            ShatterChunks(remainingChunks, 0, bossHealthBarObjects);
            HealChunks(remainingChunks, 0, bossHealthBarObjects, prevBossHealthChunks);

            //Save values for future comparison
            prevBossHealthChunks = remainingChunks;
        }

        //Shatter any health chunks that have an index higher than the remaining chunk count
        private void ShatterChunks(int healthChunks, int chipChunks, List<GameObject> characterHealthBarObjects)
        {
            for (int i = healthChunks + chipChunks; i < characterHealthBarObjects.Count; i++)
            {
                characterHealthBarObjects[i].GetComponent<HealthChunk>().Shatter();
            }
        }

        //Chip any health chunks that have an index higher than the remaining chunk count
        private void ChipChunks(int healthChunks, int chipChunks, List<GameObject> characterHealthBarObjects)
        {
            for (int i = healthChunks; i < healthChunks + chipChunks; i++)
            {
                characterHealthBarObjects[i].GetComponent<HealthChunk>().Chip();
            }
        }

        //Return chipped health chunks to their normal appearance
        //This behavior applies when successfully parrying
        private void UnChipChunks(int healthChunks, int chipChunks, List<GameObject> characterHealthBarObjects)
        {
            for (int i = 0; i < healthChunks; i++)
            {
                characterHealthBarObjects[i].GetComponent<HealthChunk>().UnChip();
            }
        }

        //Restore health
        //This behavior applies when lifestealing or resetting after death
        //This means invisible chunks need to be made visible
        private void HealChunks(int healthChunks, int chipChunks, List<GameObject> characterHealthBarObjects, int prevChunks)
        {
            for (int i = prevChunks; i < healthChunks; i++)
            {
                characterHealthBarObjects[i].GetComponent<HealthChunk>().Restore();
            }
        }

        //Reset all chunks to original positions / sizes (usually when player respawns)
        public void ResetChunks()
        {
            foreach (GameObject chunk in playerHealthBarObjects)
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
            if ((currentSwordSelect != currentSwordType))// && (player.currentSwords.Count > (int)currentSwordType))
            {
                if (swordSelectTwoSwordsObject.activeInHierarchy)
                {
                    // Set the background of the old one to white
                    backgroundImagesTwoSwords[currentSwordSelect].color = Color.white;

                    // Set the background of the new one to its color
                    backgroundImagesTwoSwords[currentSwordType].color = backgroundColors[currentSwordType];
                }
                else if (swordSelectObject.activeInHierarchy)
                {
                    // Set the background of the old one to white
                    backgroundImages[currentSwordSelect].color = Color.white;

                    // Set the background of the new one to its color
                    backgroundImages[currentSwordType].color = backgroundColors[currentSwordType];
                }
                
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

        /// <summary>
        /// Toggle the radial menu on or off
        /// </summary>
        /// <param name="active">Whether the radial menu should be active or not0</param>
        public void ToggleTwoSwordsRadialMenu(bool active)
        {
            // Only update appearance of sword select if it should be active
            if (active)
            {
                // Loop through all gem types
                foreach (Enums.SwordType swordType in gemImagesTwoSwords.Keys)
                {
                    // If the sword type is the currently selected one
                    // Set it to the active values
                    if (swordType == currentSwordSelect)
                    {
                        gemImagesTwoSwords[swordType].sprite = activeGemSprites[swordType];
                        gemImagesTwoSwords[swordType].rectTransform.sizeDelta = new Vector2(activeSize, activeSize);
                        backgroundImagesTwoSwords[swordType].color = backgroundColors[swordType];
                    }
                    // Otherwise set it to inactive values
                    else
                    {
                        gemImagesTwoSwords[swordType].sprite = inactiveGemSprites[swordType];
                        gemImagesTwoSwords[swordType].rectTransform.sizeDelta = new Vector2(inactiveSize, inactiveSize);
                        backgroundImagesTwoSwords[swordType].color = Color.white;
                    }
                }
            }

            swordSelectTwoSwordsObject.SetActive(active);
        }

        /// <summary>
        /// Show or hide the boss health bar UI elements.
        /// Triggered when the player crosses the bridge for the boss fight (in BossTrigger script)
        /// </summary>
        public void ToggleBossHealthBar(bool show)
        {
            bossHealthBar.SetActive(show);
        }
    }
}

