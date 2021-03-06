using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.UI;

namespace Bladesmiths.Capstone
{
    public class TargetLock : MonoBehaviour
    {

        #region Fields
        private bool active;

        [SerializeField] [Tooltip("Which layers should get in the way of a visibility check?")]
        private LayerMask obscuringLayers;

        [SerializeField]
        private SphereCollider targetingSphere;

        // List of enemies in the level
        [SerializeField]
        private Dictionary<int, GameObject> targetableDict = new Dictionary<int, GameObject>();

        // List of enemies that are visible to the player
        // Updated each time the system is re-enabled
        private List<GameObject> visibleTargets; 

        // The enemy currently being targeted
        private GameObject targetedObject; 

        [SerializeField]
        [Tooltip("The Cinemachine Virtual Camera used to target enemies")]
        private CinemachineVirtualCamera targetLockCam;

        // The image of the target on the screen
        [SerializeField]
        private Image targetImage; 

        [SerializeField] [Tooltip("The transform from which the visibility checking" +
                                    "raycast to a potential target should begin")]
        private Transform playerCamRoot;

        private Player player; 
        #endregion

        public bool Active 
        { 
            get => active; 
            set
            {
                if (targetableDict.Count > 0)
                {
                    active = value;
                }
                else
                {
                    active = false;
                }
            }
        }

        public ObjectController ObjectController { get; set; }

        void Start() {
            player = transform.parent.GetComponent<Player>();
        }

        void Update()
        {
            // Debug logic to see where the player's targeting ray will be looking
            if (Application.isEditor && targetedObject != null)
            {
                Vector3 rayDirection = targetedObject.GetComponent<Collider>().bounds.center -
                    playerCamRoot.position;

                Debug.DrawRay(playerCamRoot.position, rayDirection, Color.red);
                Debug.DrawLine(transform.position, transform.position + transform.right, Color.blue);
                Debug.DrawLine(transform.position, transform.position + transform.forward, Color.green); 
            }

            // If target lock is enabled
            if (Active && targetableDict.Count != 0 && targetedObject != null)
            {
                // Reposition the target image and make the player look at the target
                RepositionTargetImage();

                // Disables look at while player is dodging
                if (player.shouldLookAt)
                {
                    transform.parent.LookAt(new Vector3(targetedObject.transform.position.x, transform.parent.position.y, targetedObject.transform.position.z));
                }

                // Left Over in case we want target lock to turn if
                // something is obscuring the ray
                //// Check if the targetted enemy is visible
                //if (!IsEnemyVisible(targetedObject))
                //{
                //    // Turn off Target Lock
                //    // Switch back to the other camera having top priority
                //    targetLockCam.Priority = 0;

                //    targetImage.gameObject.SetActive(false);

                //    // Update the player follow camera's target so it doesn't move in weird directions
                //    Player playerComp = gameObject.GetComponent<Player>();
                //}
            }
            else
            {
                DisableTargetLock();
            }
        }

        #region Methods

        /// <summary>
        /// Finds the closest enemy and locks on to them or untargets them depending on whether the
        /// target locking system is active or not
        /// </summary>
        public void LockOnEnemy()
        {
            // If the target lock system is currently active
            // Find all visible enemies and then find the closest
            if (Active && targetableDict.Count != 0)
            {
                // Find all visible enemies and place them in a list
                visibleTargets = FindVisibleEnemies();

                // If there are no visible enemies
                // Turn off the target locking system and disable the target lock camera
                if (visibleTargets == null)
                {
                    DisableTargetLock();
                    return; 
                }

                // Sets the currently targeted enemy to the first enemy in the list of visible enemies
                targetedObject = visibleTargets[0];

                if (visibleTargets.Count > 1)
                {
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
                }

                // Disables Look At while dodging
                //if (player.shouldLookAt)
                //{
                    // Set the target cam's look at to the closest enemy
                    targetLockCam.LookAt = targetedObject.transform;
                //}

                // Subscribe to OnDestruction Event
                targetedObject.transform.parent.GetComponent<IIdentified>().OnDestruction += RemoveTargetedEnemy;

                // Set the target lock camera to the top priority
                targetLockCam.Priority = 2;

                targetImage.gameObject.SetActive(true);
                RepositionTargetImage();
            }
            // If the target locking system isn't active
            else
            {
                DisableTargetLock();
            }
        }

        /// <summary>
        /// Moves the look at of the targeting camera to a different enemy
        /// </summary>
        /// <param name="xDirection">The direction the targeting system should move</param>
        public void MoveTarget(float xDirection)
        {
            // A placeholder declaration to use to hold a function to be defined later
            Func<GameObject, bool> filterFunction = x => { return false; };
            bool targetsInDirection = true;

            // The function will filter from the list of visible enemies any enemies in the wrong direction
            Func<GameObject, bool> leftFunction = x =>
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
            Func<GameObject, bool> rightFunction = x =>
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

            // If input is negative move to the next enemy to the right
            if (xDirection < 0)
            {
                filterFunction = rightFunction;
            }
            // if input is positive move to the next enemy to the left
            else if (xDirection > 0)
            {
                filterFunction = leftFunction;
            }

            // Refresh list of visilble enemies 
            visibleTargets = FindVisibleEnemies();

            // If there are no visible enemies
            // Turn off the target locking system and disable the target lock camera
            if (visibleTargets == null)
            {
                DisableTargetLock();
                return;
            }

            // Filter to desirable enemies from visible enemies
            List<GameObject> desireableTargets = visibleTargets.Where(x =>  
                                                x != targetedObject && filterFunction(x)).ToList();

            // If there are no targets in the desired direction
            // Target the furthest target to the opposite direction
            if (desireableTargets.Count == 0)
            {
                // Change the boolean indicating there are no targets in the desired direction
                targetsInDirection = false; 

                // Update the filter function to the opposite one
                filterFunction = (filterFunction == leftFunction) ? rightFunction : leftFunction;

                // Filter to desirable enemies from visible enemies with the new function
                desireableTargets = visibleTargets.Where(x =>
                                                    x != targetedObject && filterFunction(x)).ToList();
            }

            if (desireableTargets.Count == 0)
            {
                return;
            }
            // Target the most desireable enemy along the direction to the current target

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
            float desireableDist = Mathf.Abs(relativeCheckPoint.x - relativeTargetPoint.x);

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

                // Compare that displacement to the current desireable distance
                // If there are targets in the input direction, use the smallest distance
                // If there aren't targest in the input direction, use the largest distance
                if ((targetsInDirection && displacement < desireableDist) || 
                    (!targetsInDirection && displacement > desireableDist))
                {
                    // If it is more desireable update the targeted enemy and the closest distance variables
                    closestEnemyToTarget = enemy;
                    desireableDist = displacement;
                }
            }

            // Unsubscribe to OnDestruction Event
            targetedObject.transform.parent.GetComponent<IIdentified>().OnDestruction -= RemoveTargetedEnemy;

            targetedObject = closestEnemyToTarget;

            // Disables Look At while dodging
            if (player.shouldLookAt)
            {
                targetLockCam.LookAt = targetedObject.transform;
            }

            // Subscribe to OnDestruction Event
            targetedObject.transform.parent.GetComponent<IIdentified>().OnDestruction += RemoveTargetedEnemy;

            RepositionTargetImage(); 
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
            List<GameObject> visibleFiltered = targetableDict.Where(x => 
            IsEnemyVisible(x.Value)).ToDictionary(x => x.Key, x => x.Value).Values.ToList(); 

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
            if (enemy == null)
            {
                return false;
            }

            Func<Transform, Transform, bool> compareXZ = (Transform t1, Transform t2) => 
            { 
                return t1.position.x == t2.position.x && t1.position.z == t2.position.z; 
            };

            RaycastHit hit;
            bool hitBool = Physics.Linecast(playerCamRoot.position, 
                enemy.GetComponent<Collider>().bounds.center, out hit, obscuringLayers) 
                && compareXZ(hit.transform, enemy.transform);

            
            return hitBool;
        }

        /// <summary>
        /// Reposition the target image to the correct point on the screen
        /// </summary>
        private void RepositionTargetImage()
        {
            if (targetedObject != null)
            {
                targetImage.transform.position = Camera.main.WorldToScreenPoint(targetedObject.transform.position);
            }
        }

        /// <summary>
        /// Removes a Targeted Enemy from the targetted list and disables Target Lock
        /// </summary>
        /// <param name="id">The id of the enemy that should be removed</param>
        private void RemoveTargetedEnemy(int id)
        {
            targetableDict.Remove(id);

            LockOnEnemy();
            //DisableTargetLock();
        }

        /// <summary>
        /// Disables target lock and resets to the free look camera
        /// </summary>
        private void DisableTargetLock()
        {
            Active = false;

            // Switch back to the other camera having top priority
            targetLockCam.Priority = 0;

            targetImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// When an object enters the targetting radius, add it to the list of potential targets
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Targetable")
            {
                return;
            }

            targetableDict.Add(other.GetComponent<TargetLockPoint>().ID, other.gameObject); 
        }

        /// <summary>
        /// When an object leaves the targetting radius, remove it from the list of potential targets
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            if (other.tag != "Targetable")
            {
                return;
            }

            targetableDict.Remove(other.GetComponent<TargetLockPoint>().ID);
            
            if (targetableDict.Count == 0)
            {
                // Switch back to the other camera having top priority
                targetLockCam.Priority = 0;

                targetImage.gameObject.SetActive(false);

                Active = false;
            }
        }
        #endregion

        #endregion
    }
}
