using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine;
using Cinemachine; 

namespace Bladesmiths.Capstone
{
    public class TargetLock : MonoBehaviour
    {
        private List<GameObject> enemies;
        private GameObject targetedEnemy; 

        private bool targetLock;

        [SerializeField]
        [Tooltip("The Cinemachine Target Group that the Camera should be targeting")]
        private CinemachineTargetGroup targetGroup;

        [SerializeField]
        [Tooltip("The Cinemachine Free Look camera that follows the player")]
        private CinemachineFreeLook playerFreeLook; 

        // Start is called before the first frame update
        void Start()
        {
            enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 rayDirection = enemies[0].transform.Find("EnemyCameraRoot").position - transform.Find("PlayerCameraRoot").position;

            Debug.DrawRay(transform.Find("PlayerCameraRoot").position, rayDirection, Color.red);

            if (targetLock)
            {
                if (!IsEnemyVisible(targetedEnemy))
                {
                    UntargetClosestEnemy(); 
                }
            }
        }

        public void OnTargetLock(InputValue value)
        {
            targetLock = !targetLock; 

            LockOnEnemy(); 
        }

        private void LockOnEnemy()
        {
            if (targetLock)
            {
                List<GameObject> visibleEnemies = FindVisibleEnemies();

                if (visibleEnemies == null)
                {
                    targetLock = false;
                    LockOnEnemy();
                    return; 
                }

                targetedEnemy = visibleEnemies[0];

                Vector3 displacementVector = visibleEnemies[0].transform.position - transform.position;
                float closestDist = displacementVector.sqrMagnitude;

                foreach (GameObject enemy in visibleEnemies)
                {
                    displacementVector = enemy.transform.position - transform.position;
                    float sqMag = displacementVector.sqrMagnitude; 

                    if (sqMag < closestDist)
                    {
                        targetedEnemy = enemy;
                        closestDist = sqMag; 
                    }
                }

                targetGroup.AddMember(targetedEnemy.transform.Find("EnemyCameraRoot"), 0.5f, 0);

                Debug.Log("Adding Enemy"); 
            }
            else
            {
                if (targetGroup.m_Targets.Length >= 2)
                {
                    UntargetClosestEnemy(); 
                }
                else
                {
                    float prevRecenterTime = playerFreeLook.m_RecenterToTargetHeading.m_RecenteringTime;
                    playerFreeLook.m_RecenterToTargetHeading.m_RecenteringTime = 0.5f; 
                    playerFreeLook.m_RecenterToTargetHeading.RecenterNow();
                    playerFreeLook.m_RecenterToTargetHeading.m_RecenteringTime = prevRecenterTime;

                    Debug.Log("No enemies visible"); 
                }
            }
        }

        private void UntargetClosestEnemy()
        {
            targetGroup.RemoveMember(targetedEnemy.transform.Find("EnemyCameraRoot"));

            Debug.Log("Removing Closest Enemy");
        }
        
        private List<GameObject> FindVisibleEnemies()
        {
            List<GameObject> visibleFiltered = enemies.Where(x => IsEnemyVisible(x)).ToList(); 

            if (visibleFiltered.Count == 0)
            {
                return null; 
            }
            return visibleFiltered;
        }

        private bool IsEnemyVisible(GameObject enemy)
        {
            RaycastHit hit;
            Vector3 rayDirection = enemy.transform.Find("EnemyCameraRoot").position - transform.Find("PlayerCameraRoot").position;
            bool castRes = Physics.Raycast(transform.Find("PlayerCameraRoot").position, rayDirection, out hit);

            Debug.DrawRay(transform.Find("PlayerCameraRoot").position, rayDirection, Color.red);

            Debug.Log(hit.transform); 

            //return (Physics.Linecast(transform.position, enemy.transform.position, out hit) && hit.transform == enemy.transform); 

            return castRes && hit.transform == enemy.transform; 
        }
    }
}
