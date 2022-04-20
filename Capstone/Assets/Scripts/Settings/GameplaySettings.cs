using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Bladesmiths.Capstone.UI
{
    public class GameplaySettings : MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook freeLookCamera;
        [SerializeField] private UIManager uIManager;

        [SerializeField] private InputActionReference look;

        [SerializeField] private GameObject mouseSensitivitySlider;

        private float xSpeed;
        private float ySpeed;

        // Start is called before the first frame update
        void Start()
        {
            LoadGameplayPrefs();
        }

        // Update is called once per frame
        void Update()
        {
            // Temp fix until editing the input action directly is fixed
            if(SceneManager.GetActiveScene().name != "MainMenu")
            {
                freeLookCamera.m_XAxis.m_MaxSpeed = xSpeed;
                uIManager.MaxSpeedX = xSpeed;

                freeLookCamera.m_YAxis.m_MaxSpeed = ySpeed;
                uIManager.MaxSpeedY = ySpeed;
            }
        }

        public void UpdateMouseSensitivity(float value)
        {
            // X rotation
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                freeLookCamera.m_XAxis.m_MaxSpeed = value;
                uIManager.MaxSpeedX = value;

                freeLookCamera.m_YAxis.m_MaxSpeed = value;
                uIManager.MaxSpeedY = value;
            }

            xSpeed = value;
            ySpeed = value;

            PlayerPrefs.SetFloat("MouseSensitivity", value);

            //uIManager.PlayerInput.

            //Debug.Log(look.action.bindings[0].processors);
            //look.action.bindings[0].processors = string.Format("InvertVector2(invertX = false),ScaleVector2(x = {0}, y = {0})", value);
            //look.action.bindings[0].overrideProcessors = string.Format("InvertVector2(invertX = false),ScaleVector2(x = {0}, y = {0})", value);

            //for (int i = 0; i < look.action.bindings.Count; i++)
            //{
            //    Debug.Log(look.action.bindings[i]);
            //}

            //look.action.ChangeBinding(0).

            //look.action.Disable();
            //
            //InputBinding newBinding = new InputBinding(look.action.bindings[0].effectivePath);
            //newBinding.overrideProcessors = string.Format("InvertVector2(invertX = false),ScaleVector2(x = {0}, y = {0})", value);
            //Debug.Log(newBinding.processors);
            ////look.action.ApplyBindingOverride(newBinding);
            //look.action.ApplyBindingOverride(0, newBinding);
            //
            //

            // Toggle invert on this binding
            //var binding = look.action.bindings[0];
            // ^ this is a list of structs and we just made a copy!

            //if (binding.overrideProcessors != null)
            //{
            //    binding.overrideProcessors = null;
            //    look.action.ChangeBinding(0).To(binding);
            //    Debug.Log("Remove Invert");
            //}
            //else
            //{
            //    binding.overrideProcessors = string.Format("InvertVector2(invertX = false),ScaleVector2(x = {0}, y = {0})", value);
            //    look.action.ChangeBinding(0).To(binding);
            //    Debug.Log("Add Invert");
            //}
            //look.action.Enable();
        }

        public void UpdateControllerSensitivity(float value)
        {

        }

        public void LoadGameplayPrefs()
        {
            // Load mouse sensitivity
            // X rotation
            if (PlayerPrefs.GetFloat("MouseSensitivity") != 0 && SceneManager.GetActiveScene().name != "MainMenu")
            {
                freeLookCamera.m_XAxis.m_MaxSpeed = PlayerPrefs.GetFloat("MouseSensitivity");
                uIManager.MaxSpeedX = PlayerPrefs.GetFloat("MouseSensitivity");
                uIManager.MaxSpeedY = PlayerPrefs.GetFloat("MouseSensitivity");
                mouseSensitivitySlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MouseSensitivity");
            }
        }
    }
}
