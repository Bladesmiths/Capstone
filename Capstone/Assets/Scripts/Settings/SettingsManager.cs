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
        [SerializeField] private GameObject soundPanel;
        [SerializeField] private GameObject soundBackButton;

        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject gameplayBackButton;

        [SerializeField] private GameplaySettings gameplaySettingsScript;
        [SerializeField] private SoundSettings soundSettingsScript;
        [SerializeField] private GraphicSettings graphicSettingsScript;

        // Start is called before the first frame update
        void Start()
        {
            gameplaySettingsScript.LoadGameplayPrefs();
            soundSettingsScript.LoadSoundPrefs();
            graphicSettingsScript.LoadGraphicPrefs();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UnPause()
        {
            controlsMenu.SetActive(false);
            settingsPanel.SetActive(false);
            graphicsPanel.SetActive(false);
            soundPanel.SetActive(false);
            gameplayPanel.SetActive(false);

            settingsButton.GetComponent<Button>().interactable = true;
            if (graphicsButton.activeSelf == true)
                ToggleSettingsButtons();

        }

        public void ToggleControlsMenu()
        {
            controlsMenu.SetActive(!controlsMenu.activeSelf);

            EventSystem.current.SetSelectedGameObject(moveRebindButton);
        }

        public void ToggleSettingsMenu()
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);

            if (graphicsButton.activeSelf == false)
                ToggleSettingsButtons();

            if (settingsPanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(graphicsButton);
                settingsButton.GetComponent<Button>().interactable = false;
                //resumeButton.SetActive(false);
            }
            else
            {
                settingsButton.GetComponent<Button>().interactable = true;
                //resumeButton.SetActive(true);
            }
        }

        public void ToggleGraphicsSettingsMenu()
        {
            graphicsPanel.SetActive(!graphicsPanel.activeSelf);

            ToggleSettingsButtons();

            if (graphicsPanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(graphicsBackButton);
            }
        }

        public void ToggleSoundSettingsMenu()
        {
            soundPanel.SetActive(!soundPanel.activeSelf);

            ToggleSettingsButtons();

            if (soundPanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(soundBackButton);
            }
        }

        public void ToggleGameplaySettingsMenu()
        {
            gameplayPanel.SetActive(!gameplayPanel.activeSelf);

            ToggleSettingsButtons();

            if (gameplayPanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(gameplayBackButton);
            }
        }

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
            }
        }

        public void QuitToMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
