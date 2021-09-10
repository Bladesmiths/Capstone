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

        [TagSelector]
        [SerializeField]
        private string tagToCollideWith;

        [SerializeField]
        private UnityEvent onEnterMethod;
        [SerializeField]
        private UnityEvent onExitMethod; 
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (activateOnEnter && other.CompareTag(tagToCollideWith))
            {
                onEnterMethod.Invoke(); 
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (activateOnExit && other.CompareTag(tagToCollideWith))
            {
                onExitMethod.Invoke(); 
            }
        }
    }
}