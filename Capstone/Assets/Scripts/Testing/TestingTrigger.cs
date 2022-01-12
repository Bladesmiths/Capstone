using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events; 

/// <summary>
/// Source: http://www.brechtos.com/tagselectorattribute/
/// </summary>
namespace Bladesmiths.Capstone.Testing
{
    public class TestingTrigger : MonoBehaviour
    {
        #region Fields
        [Tooltip("Should this trigger On Enter")]
        [SerializeField]
        private bool activateOnEnter;

        [Tooltip("Should this trigger On Exit")]
        [SerializeField]
        private bool activateOnExit;

        // What tag should trigger this
        [TagSelector]
        [SerializeField]
        private string tagToCollideWith;

        // The method(s) to call when OnEnter is triggered
        [SerializeField]
        private UnityEvent onEnterMethod;

        // The method(s) to call when OnExit is triggered
        [SerializeField]
        private UnityEvent onExitMethod; 
        #endregion

        /// <summary>
        /// Calls the OnEnter method(s) if it should be active
        /// </summary>
        /// <param name="other">The collider that goes through the trigger</param>
        private void OnTriggerEnter(Collider other)
        {
            // Checks to see if the tag of the object is the correct tag
            if (activateOnEnter && other.CompareTag(tagToCollideWith))
            {
                onEnterMethod.Invoke(); 
            }
        }

        /// <summary>
        /// Calls the OnExit method(s) if it should be active
        /// </summary>
        /// <param name="other">The collider that goes through the trigger</param>
        private void OnTriggerExit(Collider other)
        {
            // Checks to see if the tag of the object is the correct tag
            if (activateOnExit && other.CompareTag(tagToCollideWith))
            {
                onExitMethod.Invoke(); 
            }
        }
    }
}