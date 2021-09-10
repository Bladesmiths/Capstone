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

        private bool inSecondArea; 

        [Tooltip("How fast should the projectiles be moving")]
        [SerializeField]
        private Vector3 projectileVelocity; 
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            topLeft = new Vector3(transform.position.x + transform.localScale.x / 2,
                                  transform.position.y + transform.localScale.y / 2,
                                  transform.position.z - transform.localScale.z / 2);
            bottomRight = new Vector3(transform.position.x + transform.localScale.x / 2,
                                      transform.position.y - transform.localScale.y / 2,
                                      transform.position.z + transform.localScale.z / 2);

            // Testing
            PlayerInSecondArea(); 
        }

        public void PlayerInSecondArea()
        {
            inSecondArea = true; 
            StartCoroutine("CountdownFireTimer");
        }

        public void PlayerLeavingSecondArea()
        {
            inSecondArea = false; 
        }

        private void FireProjectile()
        {
            Vector3 randomPosition;

            randomPosition.x = topLeft.x + projectilePrefab.transform.localScale.x;
            randomPosition.y = Random.Range(topLeft.y, bottomRight.y);
            randomPosition.z = Random.Range(topLeft.z, bottomRight.z);

            GameObject newProjectile = Instantiate(projectilePrefab, randomPosition, Quaternion.identity);

            newProjectile.GetComponent<TestingProjectile>().SetValues(projectileVelocity); 
        }

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
