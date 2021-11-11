using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Bladesmiths.Capstone.Testing
{
    public class TestingCannon : MonoBehaviour 
    {
        #region Fields
        [Tooltip("The Prefab for the projectile enemy to be fired")]
        [SerializeField]
        private GameObject projectilePrefab;

        [Tooltip("The Location for the projectile enemy to be fired")]
        [SerializeField]
        private GameObject projectileSpawn;

        [SerializeField]
        private GameObject shrinkLocation;

        // Coordinates of top left and bottom right
        // To be used for boundaries of random position to shoot from
        [SerializeField]
        private Vector3 topLeft;
        [SerializeField]
        private Vector3 bottomRight;

        [Tooltip("How often should the cannon fire a projectile")]
        [SerializeField]
        private float timerLimit;
        private float timerMax;

        // Whether or not the player is in the second area
        private bool inSecondArea = true; 

        [Tooltip("How fast should the projectiles be moving")]
        [SerializeField]
        private Vector3 projectileVelocity;

        [SerializeField] [Required]
        private ObjectController objectController;


        private bool isBroken = false;
        private float fadeOutTimer = 0f;
        private float fadeOutLength = 1f;
        private float shrinkSpeed = 1.0f;
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
            StartCoroutine("CountdownFireTimer");

        }

        private void Update()
        {
            if (isBroken)
            {
                // If there is a block then start fading out
                fadeOutTimer += Time.deltaTime;
                GetComponent<BoxCollider>().enabled = false;
            }
            

            // When the object should fade out
            if (fadeOutTimer >= fadeOutLength)
            {
                // Shrink the cubes
                shrinkLocation.transform.localScale = new Vector3(
                    shrinkLocation.transform.localScale.x - (shrinkSpeed * Time.deltaTime),
                    shrinkLocation.transform.localScale.y - (shrinkSpeed * Time.deltaTime),
                    shrinkLocation.transform.localScale.z - (shrinkSpeed * Time.deltaTime));

                // After the cubes are shrunk, destroy it
                if (shrinkLocation.transform.localScale.x <= 0 &&
                    shrinkLocation.transform.localScale.y <= 0 &&
                    shrinkLocation.transform.localScale.z <= 0)
                {
                    Destroy(shrinkLocation);
                }
            }
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
            if (isBroken != true)
            {
                GameObject newProjectile = Instantiate(projectilePrefab, projectileSpawn.transform.position, Quaternion.identity);
                newProjectile.transform.rotation = Quaternion.Euler(0, transform.parent.eulerAngles.y, 0);

                TestingProjectile projectileComponent = newProjectile.GetComponent<TestingProjectile>();
                projectileComponent.Velocity = Quaternion.Euler(0, transform.parent.eulerAngles.y, 0) * projectileVelocity;

                objectController.AddIdentifiedObject(Enums.Team.Enemy, projectileComponent);
            }
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


        private void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.GetComponent<TestingProjectile>() == true)
            {
                isBroken = true;
            }

        }

       

        
    }
}
