using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Bladesmiths.Capstone.UI
{
    public class SettingsManager : MonoBehaviour
    {

        [SerializeField] private GameObject controlsMenu;
        [SerializeField] private GameObject controlsButton;

        [SerializeField] private GameObject moveRebindButton;

        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject settingsButton;

        [SerializeField] private GameObject graphicsButton;
        [SerializeField] private GameObject gameplayButton;
        [SerializeField] private GameObject soundButton;
        [SerializeField] private GameObject settingsBackButton;
        [SerializeField] private GameObject quitToMenuButton;

        [SerializeField] private GameObject graphicsPanel;
        [SerializeField] private GameObject graphicsBackButton;
        [SerializeField] private GameObject graphicsPanelFirstButton;
        [SerializeField] private GameObject soundPanel;
        [SerializeField] private GameObject soundBackButton;
        [SerializeField] private GameObject soundPanelFirstButton;

        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject gameplayBackButton;
        [SerializeField] private GameObject gameplayPanelFirstButton;

        [SerializeField] private GameplaySettings gameplaySettingsScript;
        [SerializeField] private SoundSettings soundSettingsScript;
        [SerializeField] private GraphicSettings graphicSettingsScript;

        [SerializeField] private GameObject moveTargetKeyboardButton;
        [SerializeField] private Scrollbar rebindScrollbar;

        [SerializeField] private GameObject rebindBackButton, rebindResetAllButton;
        private GameObject lastSelectedButton;

        public bool onPauseScreenOnly;

        // Start is called before the first frame update
        void Start()
        {
            // Load settings from the manager since the individual panels are default disabled
            gameplaySettingsScript.LoadGameplayPrefs();
            soundSettingsScript.LoadSoundPrefs();
            graphicSettingsScript.LoadGraphicPrefs();
        }

        // Update is called once per frame
        void Update()
        {
            // If the rebind menu is active, the back and reset all buttons aren't selected, AND this button was just selected
            if (controlsMenu.activeSelf && EventSystem.current.currentSelectedGameObject != rebindBackButton && EventSystem.current.currentSelectedGameObject != rebindResetAllButton && lastSelectedButton != EventSystem.current.currentSelectedGameObject)
            {
                // Move the scrollbar to where the button is
                rebindScrollbar.value = (1 - (Mathf.Abs(EventSystem.current.currentSelectedGameObject.transform.parent.localPosition.y) / 1800)) + 0.084f;
            }

            // Save what button was last selected
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;

            Debug.Log(onPauseScreenOnly);
        }

        public void UnPause()
        {
            // Hides any open settings when the game is unpaused
            controlsMenu.SetActive(false);
            settingsPanel.SetActive(false);
            graphicsPanel.SetActive(false);
            soundPanel.SetActive(false);
            gameplayPanel.SetActive(false);

            settingsButton.GetComponent<Button>().interactable = true;
            if (graphicsButton.activeSelf == true)
                ToggleSettingsButtons();

        }

        // Toggles the control menu and selects the first button
        public void ToggleControlsMenu()
        {
            controlsMenu.SetActive(!controlsMenu.activeSelf);

            ToggleSettingsButtons();

            // Select the first button if the panel was revealed
            if (controlsMenu.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(moveRebindButton);
                onPauseScreenOnly = false;
            }

        }

        // Toggle the settings menu
        public void ToggleSettingsMenu()
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);

            // If the settings buttons are hidden, toggle them on
            if (graphicsButton.activeSelf == false)
                ToggleSettingsButtons();

            if (settingsPanel.activeSelf)
            {
                onPauseScreenOnly = false;
                EventSystem.current.SetSelectedGameObject(graphicsButton);
                settingsButton.GetComponent<Button>().interactable = false;
                //resumeButton.SetActive(false);
            }
            else
            {
                settingsButton.GetComponent<Button>().interactable = true;
                onPauseScreenOnly = true;
                //resumeButton.SetActive(true);
            }
        }

        // Toggle the graphics settings menu
        public void ToggleGraphicsSettingsMenu()
        {
            graphicsPanel.SetActive(!graphicsPanel.activeSelf);

            ToggleSettingsButtons();

            // Select the back button if the panel was revealed
            if (graphicsPanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(graphicsPanelFirstButton);
                onPauseScreenOnly = false;
            }
        }

        // Toggle the sound settings menu
        public void ToggleSoundSettingsMenu()
        {
            soundPanel.SetActive(!soundPanel.activeSelf);

            ToggleSettingsButtons();

            // Select the back button if the panel was revealed
            if (soundPanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(soundPanelFirstButton);
                onPauseScreenOnly = false;
            }
        }

        // Toggle the gameplay settings menu
        public void ToggleGameplaySettingsMenu()
        {
            gameplayPanel.SetActive(!gameplayPanel.activeSelf);

            ToggleSettingsButtons();

            // Select the back button if the panel was revealed
            if (gameplayPanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(gameplayPanelFirstButton);
                onPauseScreenOnly = false;
            }
        }

        // Toggles all the individual buttons that you see when you first click on the settings button
        private void ToggleSettingsButtons()
        {
            graphicsButton.SetActive(!graphicsButton.activeSelf);
            soundButton.SetActive(!soundButton.activeSelf);
            gameplayButton.SetActive(!gameplayButton.activeSelf);
            controlsButton.SetActive(!controlsButton.activeSelf);
            settingsBackButton.SetActive(!settingsBackButton.activeSelf);
            quitToMenuButton.SetActive(!quitToMenuButton.activeSelf);

            if (graphicsButton.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(graphicsButton);
                onPauseScreenOnly = false;
            }
        }

        public void CloseActiveSettingsPanel()
        {
            if (graphicsPanel.activeSelf)
            {
                ToggleGraphicsSettingsMenu();
            }
            else if(soundPanel.activeSelf)
            {
                ToggleSoundSettingsMenu();
            }
            else if(controlsMenu.activeSelf)
            {
                ToggleControlsMenu();
            }
            else if(gameplayPanel.activeSelf)
            {
                ToggleGameplaySettingsMenu();
            }
            // If no other panel is open, then only the settings buttons should be left
            else
            {
                ToggleSettingsMenu();
            }


            //onPauseScreenOnly = true;
        }

        public void QuitToMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
