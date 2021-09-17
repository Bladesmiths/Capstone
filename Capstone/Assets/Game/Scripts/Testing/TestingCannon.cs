using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public class TestingCannon : MonoBehaviour
    {
        #region Fields
        [Tooltip("The Prefab for the projectile enemy to be fired")]
        [SerializeField]
        private GameObject projectilePrefab;

        // Coordinates of top left and bottom right
        // To be used for boundaries of random position to shoot from
        [SerializeField]
        private Vector3 topLeft;
        [SerializeField]
        private Vector3 bottomRight;

        [Tooltip("How often should the cannon fire a projectile")]
        [SerializeField]
        private float timerLimit;

        // Whether or not the player is in the second area
        private bool inSecondArea; 

        [Tooltip("How fast should the projectiles be moving")]
        [SerializeField]
        private Vector3 projectileVelocity; 
        #endregion

        /// <summary>
        /// Sets the top left and bottom right fields
        /// </summary>
        void Start()
        {
            topLeft = new Vector3(transform.position.x + transform.localScale.x / 2,
                                  transform.position.y + transform.localScale.y / 2,
                                  transform.position.z - transform.localScale.z / 2);
            bottomRight = new Vector3(transform.position.x + transform.localScale.x / 2,
                                      transform.position.y - transform.localScale.y / 2,
                                      transform.position.z + transform.localScale.z / 2);
        }

        /// <summary>
        /// Marks the In Second Area bool as true and
        /// starts a coroutine to start firing the projectiles
        /// </summary>
        public void PlayerEnterSecondArea()
        {
            inSecondArea = true; 
            StartCoroutine("CountdownFireTimer");
        }

        /// <summary>
        /// Marks the in second area bool as false
        /// </summary>
        public void PlayerLeaveSecondArea()
        {
            inSecondArea = false; 
        }

        /// <summary>
        /// Fires a projectile from a random point on the "cannon's" face
        /// </summary>
        private void FireProjectile()
        {
            Vector3 randomPosition;

            randomPosition.x = topLeft.x + projectilePrefab.transform.localScale.x;
            randomPosition.y = Random.Range(topLeft.y, bottomRight.y);
            randomPosition.z = Random.Range(topLeft.z, bottomRight.z);

            GameObject newProjectile = Instantiate(projectilePrefab, randomPosition, Quaternion.identity);

            newProjectile.GetComponent<TestingProjectile>().Velocity = projectileVelocity; 
        }

        /// <summary>
        /// Coroutine to continue firing a projectile every x seconds as long as a boolean is true
        /// </summary>
        /// <returns>Coroutine variable</returns>
        IEnumerator CountdownFireTimer()
        {
            while(inSecondArea)
            {
                FireProjectile();
                yield return new WaitForSeconds(timerLimit);
            }
        }
    }
}
