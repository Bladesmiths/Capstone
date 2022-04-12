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
using Bladesmiths.Capstone.Enums;
using DG.Tweening;

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
        bool bossBarAnimationStarted;
        bool bossBarAnimationFinished;
        public bool gainingSword;
        private SwordType newSword;
        private bool swordSelected;

        [SerializeField] private PlayerInput playerInput;

        [TitleGroup("HUD")]
        [SerializeField] private TextMeshProUGUI pointsText;

        [HorizontalGroup("HUD/FirstRow")]
        [BoxGroup("HUD/FirstRow/Health Bar Objects")]

        //Health chunk objects
        [OdinSerialize]
        private List<HealthChunk> playerHealthBarChunks = new List<HealthChunk>();

        [HorizontalGroup("HUD/FirstRow")]
        [BoxGroup("HUD/FirstRow/Health Bar Objects")]
        [OdinSerialize]
        private List<HealthChunk> bossHealthBarChunks = new List<HealthChunk>();

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
        [SerializeField] private GameObject pauseMenuObject;

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

        //Health bar colors
        private Color32 topazChunkColor = new Color32(255, 218, 87, 255);
        private Color32 rubyChunkColor = new Color32 (237, 11, 0, 255);
        private Color32 sapphireChunkColor = new Color32 (82, 110, 255, 255);
        private Color32 amethystColor = new Color32(162, 0, 255, 255);

        private Color32 goalColor;
        private Coroutine switchSwordColorRoutine;

        private Color32 bossChunkColor;

        public bool rainbow = true;

        private PauseMenu pauseMenu;

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
        
        public float MaxSpeedX
        {
            get { return maxSpeedX; }
            set { maxSpeedX = value; }
        }
        
        public PlayerInput Inputs { get => playerInput; }
        public PauseMenu PauseMenu { get => pauseMenu; }

        // Start is called before the first frame update
        void Start()
        {
            // Initialize variables
            isPaused = false;
            gainingSword = false;
            swordSelected = false;
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
            playerHealthBarChunks.Reverse();
            bossHealthBarChunks.Reverse();

            boss = Boss.instance;

            pauseMenu = pauseMenuObject.GetComponent<PauseMenu>();
            pauseMenu.SwapControls(playerInput.currentControlScheme);

            //Set player health bar to topaz
            StartCoroutine(ColorChunks(playerHealthBarChunks, topazChunkColor));
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
            if (boss != null && bossHealthBar.activeSelf)
            {
                if (bossPrevHealth != boss.Health)
                {
                    UpdateBossHealthBar(false);
                }
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

                    pauseMenuObject.SetActive(false);
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

                pauseMenu.SwapFormInfo(player.Inputs.currentSwordType);
                pauseMenu.SwapControls(playerInput.currentControlScheme);

                // Stops the camera from moving and stops the recentering
                camera.m_XAxis.m_MaxSpeed = 0f;
                camera.m_YAxis.m_MaxSpeed = 0f;
                camera.m_RecenterToTargetHeading.m_enabled = false;
                camera.m_YAxisRecentering.m_enabled = false;

                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;

                pauseMenuObject.SetActive(true);

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
            ShatterChunks(remainingChunks, chipChunks, playerHealthBarChunks);
            ChipChunks(remainingChunks, chipChunks, playerHealthBarChunks);
            HealChunks(remainingChunks, chipChunks, playerHealthBarChunks, prevPlayerHealthChunks);
            UnChipChunks(remainingChunks, chipChunks, playerHealthBarChunks);

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
        public void UpdateBossHealthBar(bool reset)
        {
            float currentHealth = boss.Health;
            float maxHealth = boss.MaxHealth;

            //Pretend the boss has 0 HP for future animations
            if (reset)
            {
                currentHealth = 0;
            }

            bossPrevHealth = currentHealth;
            //This commented out code converts the player's raw health values into percentages so the UI can work with any max health value, not just 100.
            //For now the raw health values are being used directly because the player has 100 health and the UI has 100 chunks.

            //float healthAndChipPercentage = (currentHealth + currentChipDamage) / maxHealth;
            //float currentHealthPercentage = currentHealth / maxHealth;
            //float chipHealthPercentage = healthAndChipPercentage - currentHealthPercentage;

            //Determine how many chunks remain in the health bar after taking damage
            //int remainingChunks = (int)(currentHealthPercentage * healthBarObjects.Count);
            //int chipChunks = (int)(chipHealthPercentage * healthBarObjects.Count); 

            int remainingChunks = (int)(currentHealth / (maxHealth / bossHealthBarChunks.Count));

            int totalChunks = remainingChunks;

            //Modify chunk status
            ShatterChunks(remainingChunks, 0, bossHealthBarChunks);
            HealChunks(remainingChunks, 0, bossHealthBarChunks, prevBossHealthChunks);
            

            //Save values for future comparison
            prevBossHealthChunks = remainingChunks;
        }

        //Shatter any health chunks that have an index higher than the remaining chunk count
        private void ShatterChunks(int healthChunks, int chipChunks, List<HealthChunk> characterHealthBarObjects)
        {
            for (int i = healthChunks + chipChunks; i < characterHealthBarObjects.Count; i++)
            {
                characterHealthBarObjects[i].Shatter();
            }
        }

        //Chip any health chunks that have an index higher than the remaining chunk count
        private void ChipChunks(int healthChunks, int chipChunks, List<HealthChunk> characterHealthBarObjects)
        {
            for (int i = healthChunks; i < healthChunks + chipChunks; i++)
            {
                characterHealthBarObjects[i].Chip();
            }
        }

        //Return chipped health chunks to their normal appearance
        //This behavior applies when successfully parrying
        private void UnChipChunks(int healthChunks, int chipChunks, List<HealthChunk> characterHealthBarObjects)
        {
            for (int i = 0; i < healthChunks; i++)
            {
                characterHealthBarObjects[i].UnChip();
            }
        }

        //Restore health
        //This behavior applies when lifestealing or resetting after death
        //This means invisible chunks need to be made visible
        private void HealChunks(int healthChunks, int chipChunks, List<HealthChunk> characterHealthBarObjects, int prevChunks)
        {
            for (int i = prevChunks; i < healthChunks; i++)
            {
                characterHealthBarObjects[i].Restore();
            }
        }

        //Reset all chunks to original positions / sizes (usually when player respawns)
        public void ResetChunks()
        {
            foreach (HealthChunk chunk in playerHealthBarChunks)
            {
                chunk.FullReset();
            }

            if (bossHealthBar.activeSelf)
            {
                foreach (HealthChunk chunk in bossHealthBarChunks)
                {
                    chunk.FullReset();
                }
            }
        }


        /// <summary>
        /// Set up the coroutine to change player health bar chunk colors
        /// </summary>
        /// <param name="currentSword"></param>
        public void SwitchSwordHealthBar(SwordType currentSword)
        {
            //Stop any color changing coroutine already running
            if(switchSwordColorRoutine != null)
            {
                StopCoroutine(switchSwordColorRoutine);
            }

            //Set goal color based on current sword
            switch (currentSword)
            {
                case SwordType.Topaz:
                    goalColor = topazChunkColor;
                    break;

                case SwordType.Ruby:
                    goalColor = rubyChunkColor;
                    break;

                case SwordType.Sapphire:
                    goalColor = sapphireChunkColor;
                    break;

                default:
                    goalColor = Color.white;
                    break;
            }

            switchSwordColorRoutine = StartCoroutine(ColorChunks(playerHealthBarChunks, goalColor));
        }

        /// <summary>
        /// Coroutine that updates health bar chunk colors
        /// Chunks lerp from their current color to a specified goal color
        /// Used to match health bar color to current sword form
        /// </summary>
        /// <param name="goalColor"></param>
        /// <returns></returns>
        public IEnumerator ColorChunks(List<HealthChunk> characterHealthBarChunks, Color goalColor)
        {
            //Number of loops to reach goal color
            //More loops means a smoother transition
            float loops = 25f;

            for (float t = 0; t <= 1; t += (1 / loops))
            {
                //Change each chunk's color
                foreach (HealthChunk chunk in characterHealthBarChunks)
                {
                    chunk.SetColor(goalColor, t);
                }

                //Time between loops
                yield return new WaitForSeconds(0.02f);
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
                    //backgroundImagesTwoSwords[currentSwordSelect].color = Color.white;
                    backgroundImagesTwoSwords[currentSwordSelect].enabled = false;

                    // Set the background of the new one to its color
                    //backgroundImagesTwoSwords[currentSwordType].color = backgroundColors[currentSwordType];
                    backgroundImagesTwoSwords[currentSwordType].enabled = true;
                }
                else if (swordSelectObject.activeInHierarchy)
                {
                    // Set the background of the old one to white
                    //backgroundImages[currentSwordSelect].color = Color.white;
                    backgroundImages[currentSwordSelect].enabled = false;

                    // Set the background of the new one to its color
                    //backgroundImages[currentSwordType].color = backgroundColors[currentSwordType];
                    backgroundImages[currentSwordType].enabled = true;
                }

                // Update the field
                currentSwordSelect = currentSwordType;
            }
        }

        /// <summary>
        /// Called when the Player gains a new sword
        /// </summary>
        /// <param name="type"></param>
        public void GainNewSword(SwordType type)
        {
            StartCoroutine(GetNewSword(type));
        }

        /// <summary>
        /// The main Corutine that runs when the Player gains a new sword
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerator GetNewSword(SwordType type)
        {
            gainingSword = true;
            swordSelected = false;

            if (player.currentSwords.Count == 2)
            {
                gemImagesTwoSwords[type].color = new Color(
                    gemImagesTwoSwords[type].color.r, 
                    gemImagesTwoSwords[type].color.b, 
                    gemImagesTwoSwords[type].color.g, 0f);

                // Opens the sword switching menu
                ToggleTwoSwordsRadialMenu(true);
                player.GetComponent<PlayerInputsScript>().switchingSwords = true;
                player.ResetAnimationParameters();
                camera.GetComponent<CustomCinemachineInputProvider>().InputEnabled = false;

                backgroundImagesTwoSwords[currentSwordSelect].enabled = true;
                yield return new WaitForSeconds(1f);
                
                // Fades in the new sword
                gemImagesTwoSwords[type].DOFade(1f, 2f);
                yield return new WaitForSeconds(3f);

                // Pulses the gem and waits until the Player selects it
                player.Inputs.swordSelectActive = true;
                StartCoroutine(PulseGem(type, Color.red, gemImagesTwoSwords));
                yield return new WaitUntil(rubySwordSelected());
                swordSelected = true;

                StopCoroutine(PulseGem(type, Color.red, gemImagesTwoSwords));
                player.Inputs.swordSelectActive = false;                
                gemImagesTwoSwords[type].DOColor(Color.red, 1f);
                yield return new WaitForSeconds(1f);

                // Switches the sword to the new sword
                player.SwitchSword(SwordType.Ruby);
                ToggleTwoSwordsRadialMenu(false);

                gemImagesTwoSwords[type].DOColor(Color.white, 0.1f);
                yield return new WaitForSeconds(0.1f);

                camera.GetComponent<CustomCinemachineInputProvider>().InputEnabled = true;
                player.GetComponent<PlayerInputsScript>().switchingSwords = false;
            }
            else if (player.currentSwords.Count > 2)
            {
                gemImages[type].color = new Color(
                    gemImages[type].color.r,
                    gemImages[type].color.b,
                    gemImages[type].color.g, 0f);

                // Opens the sword switching menu
                ToggleRadialMenu(true);
                player.GetComponent<PlayerInputsScript>().switchingSwords = true;
                player.ResetAnimationParameters();
                camera.GetComponent<CustomCinemachineInputProvider>().InputEnabled = false;

                backgroundImages[currentSwordSelect].enabled = true;
                yield return new WaitForSeconds(1f);

                // Fades in the new sword
                gemImages[type].DOFade(1f, 2f);
                yield return new WaitForSeconds(3f);

                // Pulses the gem and waits until the Player selects it
                player.Inputs.swordSelectActive = true;
                StartCoroutine(PulseGem(type, Color.blue, gemImages));
                yield return new WaitUntil(sapphireSwordSelected());

                swordSelected = true;
                StopCoroutine(PulseGem(type, Color.blue, gemImages));
                player.Inputs.swordSelectActive = false;
                gemImages[type].DOColor(Color.blue, 1f);
                yield return new WaitForSeconds(1f);

                // Switches the sword to the new sword
                player.SwitchSword(SwordType.Sapphire);
                ToggleRadialMenu(false);

                gemImages[type].DOColor(Color.white, 0.1f);
                yield return new WaitForSeconds(0.1f);

                camera.GetComponent<CustomCinemachineInputProvider>().InputEnabled = true;
                player.GetComponent<PlayerInputsScript>().switchingSwords = false;
            }

            gainingSword = false;
        }

        /// <summary>
        /// Checks to see if the current sword is Ruby
        /// </summary>
        /// <returns></returns>
        public Func<bool> rubySwordSelected()=> () => player.Inputs.currentSwordType == SwordType.Ruby;

        /// <summary>
        /// Checks to see if the current sword is Sapphire
        /// </summary>
        /// <returns></returns>
        public Func<bool> sapphireSwordSelected() => () => player.Inputs.currentSwordType == SwordType.Sapphire;

        /// <summary>
        /// Pulses the current gem
        /// </summary>
        /// <param name="type"></param>
        /// <param name="c"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public IEnumerator PulseGem(SwordType type, Color c, Dictionary<SwordType, Image> dict)
        {
            dict[type].DOColor(c, 0.5f);
            yield return new WaitForSeconds(0.5f);
            dict[type].DOColor(Color.white, 0.5f);
            yield return new WaitForSeconds(0.5f);

            if (swordSelected == false)
            {
                StartCoroutine(PulseGem(type, c, dict));
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
                        //backgroundImages[swordType].enabled = true;
                        //gemImages[swordType].rectTransform.sizeDelta = new Vector2(activeSize, activeSize);
                        //backgroundImages[swordType].color = backgroundColors[swordType];
                    }
                    // Otherwise set it to inactive values
                    else
                    {
                        //backgroundImages[swordType].enabled = false;
                        //gemImages[swordType].sprite = inactiveGemSprites[swordType];
                        //gemImages[swordType].rectTransform.sizeDelta = new Vector2(inactiveSize, inactiveSize);
                        //backgroundImages[swordType].color = Color.white;
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
                        //backgroundImagesTwoSwords[swordType].enabled = true;
                        //gemImages[swordType].rectTransform.sizeDelta = new Vector2(activeSize, activeSize);
                        //backgroundImages[swordType].color = backgroundColors[swordType];
                    }
                    // Otherwise set it to inactive values
                    else
                    {
                        //backgroundImagesTwoSwords[swordType].enabled = false;
                        //gemImages[swordType].sprite = inactiveGemSprites[swordType];
                        //gemImages[swordType].rectTransform.sizeDelta = new Vector2(inactiveSize, inactiveSize);
                        //backgroundImages[swordType].color = Color.white;
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
            //Set boss health bar color to amethyst
            bossHealthBar.SetActive(show);

            if (show)
            {
                foreach (HealthChunk chunk in bossHealthBarChunks)
                {
                    if (chunk.image != null)
                    {
                        chunk.image.color = amethystColor;
                    }
                }
            }

            //Animation currently disabled due to visual bugs
            //StartCoroutine(AnimateGrowingHealthBar(bossHealthBarChunks));
        }

        /// <summary>
        /// Coroutine that reveals each chunk in a health bar one at a time for an animated growing effect
        /// </summary>
        /// <param name="characterHealthBarChunks"></param>
        /// <returns></returns>
        public IEnumerator AnimateGrowingHealthBar(List<HealthChunk> characterHealthBarChunks)
        {
            if (!bossBarAnimationStarted)
            {
                bossBarAnimationStarted = true;
                //Start by ensuring all chunks are hidden
                foreach (HealthChunk chunk in characterHealthBarChunks)
                {
                    chunk.InvisibleReset();
                }

                //Reveal each chunk one at a time
                for (int i = 0; i < characterHealthBarChunks.Count; i += 3)
                {
                    characterHealthBarChunks[i].FullReset();

                    if (i + 2 < characterHealthBarChunks.Count)
                    {
                        characterHealthBarChunks[i + 2].FullReset();
                        characterHealthBarChunks[i + 1].FullReset();
                    }
                    else if (i + 1 < characterHealthBarChunks.Count)
                    {
                        characterHealthBarChunks[i + 1].FullReset();
                    }

                    //Wait for as little time as possible
                    yield return new WaitForSeconds(0.00001f);
                }

                bossBarAnimationFinished = true;
            }
        }
    }
}

