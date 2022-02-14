using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Bladesmiths.Capstone
{
    public class MenuInputsScript : MonoBehaviour
    {
        [SerializeField] private UI.SettingsManager settingsManager;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnCancel(InputValue value)
        {
            if (value.isPressed)
            {
                // Makes sure this only happens on the main menu since we don't want to "close" the main menu buttons
                // The onPauseScreenOnly check is kind of stupid but it's easiest, fastest, and least complex way to prevent you from toggling the settings menu with the B button while its closed
                if (SceneManager.GetActiveScene().name == "MainMenu" && !settingsManager.onPauseScreenOnly)
                    settingsManager.CloseActiveSettingsPanel();
            }
        }
    }
}
