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
        [Tooltip("The Cinemachine Target Group that the Camera should be targeting")]
        private CinemachineTargetGroup targetGroup;

        [SerializeField]
        [Tooltip("The Cinemachine Free Look camera that follows the player")]
        private CinemachineVirtualCamera targetLockCam;

        [SerializeField]
        private ThirdPersonController thirdPersonController;
        #endregion

        void Start()
        {
            // Finds all enemies in the level
            // Should probably be updated eventually so it only gets enemies within a radius
            targettableList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Targettable"));
        }

        void Update()
        {
            // Debug logic to see where the player's targetting ray will be looking
            if (Application.isEditor && targetedObject != null)
            {
                Vector3 rayDirection = targetedObject.transform.Find("EnemyCameraRoot").position - 
                    transform.Find("PlayerCameraRoot").position;

                Debug.DrawRay(transform.Find("PlayerCameraRoot").position, rayDirection, Color.red);
            }

            // If target lock is enabled
            if (targetLock)
            {
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

        public void OnMoveTarget(InputValue value)
        {
            if (targetLock)
            {
                float moveDirection = value.Get<float>();

                Debug.Log(moveDirection);

                if (moveDirection > 0)
                {
                    MoveTarget(1);
                }
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
            // TO-DO: Update this so in keyboard system it moves to next target on mouse scroll
            //        In gamepad, should switch target based on right stick movement
            //        Once target is locked, camera should no longer be free look

            // If the target lock system is currently active
            // Find all visible enemies and then find the closest
            if (targetLock)
            {
                // Find all visible enemies and place them in a list
                visibleTargets = FindVisibleEnemies();

                // If there are no visible enemies
                // Turn off the target locking system, run this method again, then end the method
                if (visibleTargets == null)
                {
                    targetLock = false;
                    LockOnEnemy();
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

                // Add the updated targeted enemy to the target group
                // so the camera takes it into account
                targetGroup.AddMember(targetedObject.transform.Find("EnemyCameraRoot"), 1.5f, 10.0f);

                targetLockCam.Priority = 2;

                thirdPersonController.target = targetedObject;
                transform.LookAt(targetedObject.transform);

                Debug.Log("Adding Enemy"); 
            }
            // If the target locking system isn't active
            else
            {
                // If there are 2 or more objects in the target group
                // untarget the enemy in the target group
                if (targetGroup.m_Targets.Length >= 2)
                {
                    UntargetClosestEnemy(); 
                }
                // If there are less than 2 objects in the target group
                // Recenter the camera to the player's heading
                else
                {
                    //float prevRecenterTime = targetLockCam.m_RecenterToTargetHeading.m_RecenteringTime;
                    //targetLockCam.m_RecenterToTargetHeading.m_RecenteringTime = 0.5f; 
                    //targetLockCam.m_RecenterToTargetHeading.RecenterNow();
                    //targetLockCam.m_RecenterToTargetHeading.m_RecenteringTime = prevRecenterTime;

                    Debug.Log("No enemies visible"); 
                }

                targetLockCam.Priority = 0;

                thirdPersonController.target = null;
            }
        }

        private void MoveTarget(float xDirection)
        { 
            Func<GameObject, bool> filterFunction = x => { return false; } ; 

            // If input is positive move to the next enemy to the right
            if (xDirection > 0)
            {
                filterFunction = x =>
                {
                    return Vector3.Dot(targetedObject.transform.right, x.transform.position) > 0;
                };
            }
            // if input is negative move to the next enemy to the left
            else if (xDirection < 0)
            {
                filterFunction = x =>
                {
                    return Vector3.Dot(targetedObject.transform.right, x.transform.position) > 0;
                };
            }

            // Compare dot product of enemies to current target's position

            // Filter to desirable enemies from visible enemies

            List<GameObject> desireableEnemies = visibleTargets.Where(x =>  x != targetedObject && filterFunction(x)).ToList();

            if (desireableEnemies.Count == 0)
            {
                return; 
            }

            // Target the desireable enemy closest to the current target

            // Sets the first enemy in the list to the closest first for comparison
            GameObject closestEnemyToTarget = desireableEnemies[0];

            // Calculates the squared distance to that enemy and sets a variable
            // to that value for use in comparisons with other enemies
            Vector3 displacementVector = desireableEnemies[0].transform.position - targetedObject.transform.position;
            float closestDist = displacementVector.sqrMagnitude;

            // Loop through all visible enemies
            foreach (GameObject enemy in desireableEnemies)
            {
                // Calculate squared distance to this enemy
                displacementVector = enemy.transform.position - targetedObject.transform.position;
                float sqMag = displacementVector.sqrMagnitude;

                // Compare that distance to the current smallest distance
                if (sqMag < closestDist)
                {
                    // If it is smaller update the targeted enemy and the closest distance field
                    closestEnemyToTarget = enemy;
                    closestDist = sqMag;
                }
            }

            UntargetClosestEnemy(); 
            targetedObject = closestEnemyToTarget;
            targetGroup.AddMember(targetedObject.transform.Find("EnemyCameraRoot"), 1.0f, 0);

            thirdPersonController.target = targetedObject;
            transform.LookAt(targetedObject.transform);
        }

        #region Helper Methods
        /// <summary>
        /// Untarget the closest enemy
        /// </summary>
        private void UntargetClosestEnemy()
        {
            // Remove the targeted enemy from the target group
            targetGroup.RemoveMember(targetedObject.transform.Find("EnemyCameraRoot"));

            Debug.Log("Removing Closest Enemy");
        }
        
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
            //Vector3 rayDirection = enemy.transfor

            //m.Find("EnemyCameraRoot").position - 
            //    transform.Find("PlayerCameraRoot").position;

            //bool castRes = Physics.Raycast(transform.Find("PlayerCameraRoot").position, rayDirection, out hit);

            return (Physics.Linecast(transform.Find("PlayerCameraRoot").position, enemy.transform.Find("EnemyCameraRoot").position, out hit) && hit.transform == enemy.transform); 

            //return castRes && hit.transform == enemy.transform; 
        }
        #endregion
        
        #endregion
    }
}
