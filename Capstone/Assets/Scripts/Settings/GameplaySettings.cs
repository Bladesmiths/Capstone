using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

namespace Bladesmiths.Capstone.UI
{
    public class GameplaySettings : MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook freeLookCamera;
        [SerializeField] private UIManager uIManager;

        [SerializeField] private InputActionReference look;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateMouseSensitivity(float value)
        {
            // X rotation
            freeLookCamera.m_XAxis.m_MaxSpeed = value;
            uIManager.MaxSpeedX = value;

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
    }
}
