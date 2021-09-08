using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine;
using Cinemachine;
using System;

namespace Bladesmiths.Capstone
{
    public class TargetLock : MonoBehaviour
    {
                                                                                                    
        #region Fields
        // List of enemies in the level
        private List<GameObject> targettableList;

        // List of enemies that are visible to the player
        // Updated each time the system is re-enabled
        private List<GameObject> visibleTargets; 

        // The enemy currently being targeted
        private GameObject targetedObject; 

        // Is the target locking system active or not
        private bool targetLock;

        [SerializeField]
        [Tooltip("The Cinemachine Virtual Camera used to target enemies")]
        private CinemachineVirtualCamera targetLockCam;

        [SerializeField]
        private Canvas targetCanvas; 
        #endregion

        void Start()
        {
            // Finds all enemies in the level
            // Should probably be updated eventually so it only gets enemies within a radius
            // And Targettable won't be a useful tag in the future
            targettableList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Targettable"));
        }

        void Update()
        {
            // Debug logic to see where the player's targeting ray will be looking
            if (Application.isEditor && targetedObject != null)
            {
                Vector3 rayDirection = targetedObject.transform.Find("EnemyCameraRoot").position - 
                    transform.Find("PlayerCameraRoot").position;

                Debug.DrawRay(transform.Find("PlayerCameraRoot").position, rayDirection, Color.red);
                Debug.DrawLine(transform.position, transform.position + transform.right, Color.blue);
                Debug.DrawLine(transform.position, transform.position + transform.forward, Color.green); 
            }

            // If target lock is enabled
            if (targetLock)
            {
                RepositionTargetCanvas();

                // Check if the targetted enemy is visible
                if (!IsEnemyVisible(targetedObject))
                {
                    // If it is not, run lock on again to check if there are any other visible
                    // enemies. If not, turn off the target locking system
                    LockOnEnemy(); 
                }
            }
        }

        #region Methods

        #region On Input Methods
        /// <summary>
        /// Input method that runs when the target lock control is hit
        /// </summary>
        /// <param name="value">The value of the control</param>
        public void OnTargetLock(InputValue value)
        {
            // Toggles the target lock state to its opposite value
            targetLock = !targetLock; 

            // Runs the LockOnEnemy method no matter what because it serves both purposes
            LockOnEnemy(); 
        }

        /// <summary>
        /// Input method that runs when the MoveTarget input is used
        /// </summary>
        /// <param name="value">The value of the float input</param>
        public void OnMoveTarget(InputValue value)
        {
            // Checks if target lock is active
            // If not, do nothing
            if (targetLock)
            {
                // Converts the input value to a usable float
                float moveDirection = value.Get<float>();
                
                // If the value is positive
                // Move the target to the right
                if (moveDirection > 0)
                {
                    MoveTarget(1);
                }
                // If the value is negative
                // Move the target to the left7
                else if (moveDirection < 0)
                {
                    MoveTarget(-1); 
                }
            }
        }

        #endregion

        /// <summary>
        /// Finds the closest enemy and locks on to them or untargets them depending on whether the
        /// target locking system is active or not
        /// </summary>
        private void LockOnEnemy()
        {
            // If the target lock system is currently active
            // Find all visible enemies and then find the closest
            if (targetLock)
            {
                // Find all visible enemies and place them in a list
                visibleTargets = FindVisibleEnemies();

                // If there are no visible enemies
                // Turn off the target locking system and disable the target lock camera
                if (visibleTargets == null)
                {
                    targetLock = false;
                    targetLockCam.Priority = 0; 
                    return; 
                }

                // Sets the currently targeted enemy to the first enemy in the list of visible enemies
                targetedObject = visibleTargets[0];

                // Calculates the squared distance to that enemy and sets a variable
                // to that value for use in comparisons with other enemies
                Vector3 displacementVector = visibleTargets[0].transform.position - transform.position;
                float closestDist = displacementVector.sqrMagnitude;

                // Loop through all visible enemies
                foreach (GameObject enemy in visibleTargets)
                {
                    // Calculate squared distance to this enemy
                    displacementVector = enemy.transform.position - transform.position;
                    float sqMag = displacementVector.sqrMagnitude; 

                    // Compare that distance to the current smallest distance
                    if (sqMag < closestDist)
                    {
                        // If it is smaller update the targeted enemy and the closest distance field
                        targetedObject = enemy;
                        closestDist = sqMag; 
                    }
                }

                // Set the target cam's look at to the closest enemy
                targetLockCam.LookAt = targetedObject.transform.Find("EnemyCameraRoot");
             
                // Set the target lock camera to the top priority
                targetLockCam.Priority = 2;

                targetCanvas.GetComponent<Canvas>().enabled = true;
                RepositionTargetCanvas();
            }
            // If the target locking system isn't active
            else
            {
                // Switch back to the free look camera having top priority
                targetLockCam.Priority = 0;

                targetCanvas.GetComponent<Canvas>().enabled = false;
            }
        }

        /// <summary>
        /// Moves the look at of the targeting camera to a different enemy
        /// </summary>
        /// <param name="xDirection">The direction the targeting system should move</param>
        private void MoveTarget(float xDirection)
        { 
            // A placeholder declaration to use to hold a function to be defined later
            // The function will filter from the list of visible enemies any enemies in the wrong direction
            Func<GameObject, bool> filterFunction = x => { return false; } ; 

            // If input is negative move to the next enemy to the right
            if (xDirection < 0)
            {
                filterFunction = x =>
                {
                    // Convert the position of this entry in the list and the currently targeted object
                    // To the targeting camera's local space
                    Vector3 relativeCheckPoint = 
                        targetLockCam.transform.InverseTransformPoint(x.transform.position);
                    Vector3 relativeTargetPoint = 
                        targetLockCam.transform.InverseTransformPoint(targetedObject.transform.position);

                    // Return whether or not the relative check object's position is greater than
                    // the relative position of the currently targeted object
                    // Is it to the right of the current target object?
                    return relativeCheckPoint.x > relativeTargetPoint.x;
                };
            }
            // if input is positive move to the next enemy to the left
            else if (xDirection > 0)
            {
                filterFunction = x =>
                {
                    // Convert the position of this entry in the list and the currently targeted object
                    // To the targeting camera's local space
                    Vector3 relativeCheckPoint = 
                        targetLockCam.transform.InverseTransformPoint(x.transform.position);
                    Vector3 relativeTargetPoint = 
                        targetLockCam.transform.InverseTransformPoint(targetedObject.transform.position);

                    // Return whether or not the relative check object's position is greater than
                    // the relative position of the currently targeted object
                    // Is it to the left of the current target object?
                    return relativeCheckPoint.x < relativeTargetPoint.x;
                };
            }

            // Refresh list of visilble enemies 
            visibleTargets = FindVisibleEnemies(); 

            // Filter to desirable enemies from visible enemies
            List<GameObject> desireableTargets = visibleTargets.Where(x =>  
                                                x != targetedObject && filterFunction(x)).ToList();

            // If there are no targets in the desired direction
            // Go no further
            if (desireableTargets.Count == 0)
            {
                return; 
            }

            // Target the desireable enemy closest along the direction to the current target

            // Sets the first enemy in the list to the closest first for comparison
            GameObject closestEnemyToTarget = desireableTargets[0];

            // Convert the position of this entry in the list and the currently targeted object
            // To the targeting camera's local space
            // Calculates the displacement along the x direction from the currently targeted object
            // to that enemy and sets a variable to that value for use in comparisons with other enemies
            Vector3 relativeCheckPoint = 
                    targetLockCam.transform.InverseTransformPoint(closestEnemyToTarget.transform.position);
            Vector3 relativeTargetPoint = 
                    targetLockCam.transform.InverseTransformPoint(targetedObject.transform.position);
            float closestDist = Mathf.Abs(relativeCheckPoint.x - relativeTargetPoint.x);

            // Loop through all visible enemies
            foreach (GameObject enemy in desireableTargets)
            {
                // Convert the position of this entry in the list and the currently targeted object
                // To the targeting camera's local space
                relativeCheckPoint = 
                        targetLockCam.transform.InverseTransformPoint(enemy.transform.position);
                relativeTargetPoint = 
                        targetLockCam.transform.InverseTransformPoint(targetedObject.transform.position);

                // Calculates the displacement along the x direction from the currently closest enemy
                // to this enemy and sets a variable to that value for comparison
                float displacement = Mathf.Abs(relativeCheckPoint.x - relativeTargetPoint.x);

                // Compare that displacement to the current smallest distance
                if (displacement < closestDist)
                {
                    // If it is smaller update the targeted enemy and the closest distance field
                    closestEnemyToTarget = enemy;
                    closestDist = displacement;
                }
            }

            targetedObject = closestEnemyToTarget;
            targetLockCam.LookAt = targetedObject.transform.Find("EnemyCameraRoot");

            RepositionTargetCanvas(); 
        }

        #region Helper Methods
        /// <summary>
        /// Find all enemies that are visible to the player
        /// </summary>
        /// <returns>Null if there are no visible enemies or the list of visible enemies
        /// if there are some</returns>
        private List<GameObject> FindVisibleEnemies()
        {
            // Filter the list of enemies to only the enemies that are visible
            List<GameObject> visibleFiltered = targettableList.Where(x => IsEnemyVisible(x)).ToList(); 

            // If there aren't any visible enemies return null
            if (visibleFiltered.Count == 0)
            {
                return null; 
            }
            return visibleFiltered;
        }

        /// <summary>
        /// Check if an enemy is visible to the player
        /// </summary>
        /// <param name="enemy">The object to check if visible</param>
        /// <returns>A boolean indicating whether or not the object is visible</returns>
        private bool IsEnemyVisible(GameObject enemy)
        {
            RaycastHit hit;

            return (Physics.Linecast(transform.Find("PlayerCameraRoot").position, 
                enemy.transform.Find("EnemyCameraRoot").position, out hit) && hit.transform == enemy.transform); 
        }

        private void RepositionTargetCanvas()
        {
            Vector3 vecFromTargetToPlayer = transform.position - targetedObject.transform.position;
            vecFromTargetToPlayer.Normalize();
            targetCanvas.transform.position = targetedObject.transform.Find("EnemyCameraRoot").position
                + (vecFromTargetToPlayer * 0.5f);
            targetCanvas.transform.rotation = Quaternion.Euler(targetCanvas.transform.rotation.eulerAngles.x,
                targetLockCam.transform.rotation.eulerAngles.y, targetCanvas.transform.rotation.eulerAngles.z);
        }
        #endregion
        
        #endregion
    }
}
