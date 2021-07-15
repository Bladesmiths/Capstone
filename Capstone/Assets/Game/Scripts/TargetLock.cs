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
        private GameObject closestEnemy; 

        private bool targetLock;

        [SerializeField]
        [Tooltip("The Cinemachine Target Group that the Camera should be targeting")]
        private CinemachineTargetGroup targetGroup;

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

                closestEnemy = visibleEnemies[0];

                Vector3 displacementVector = visibleEnemies[0].transform.position - transform.position;
                float closestDist = displacementVector.sqrMagnitude;

                foreach (GameObject enemy in visibleEnemies)
                {
                    displacementVector = enemy.transform.position - transform.position;
                    float sqMag = displacementVector.sqrMagnitude; 

                    if (sqMag < closestDist)
                    {
                        closestEnemy = enemy;
                        closestDist = sqMag; 
                    }
                }

                targetGroup.AddMember(closestEnemy.transform.Find("EnemyCameraRoot"), 0.5f, 0);

                Debug.Log("Adding Enemy"); 
            }
            else
            {
                if (targetGroup.m_Targets.Length >= 2)
                {
                    targetGroup.RemoveMember(closestEnemy.transform.Find("EnemyCameraRoot"));

                    Debug.Log("Removing Closest Enemy"); 
                }
                else
                {
                    // Center Camera

                    Debug.Log("No enemies visible"); 
                }
            }
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
            bool castRes = Physics.Raycast(transform.position, rayDirection, out hit);

            Debug.DrawRay(transform.Find("PlayerCameraRoot").position, rayDirection, Color.red);

            Debug.Log(hit.transform); 
            int test = 0;

            //return (Physics.Linecast(transform.position, enemy.transform.position, out hit) && hit.transform == enemy.transform); 

            return castRes && hit.transform == enemy.transform; 
        }
    }
}
