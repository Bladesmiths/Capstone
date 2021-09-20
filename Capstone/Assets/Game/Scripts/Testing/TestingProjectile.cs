using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public class TestingProjectile : MonoBehaviour
    {
        #region Fields
        // The velocity of the projectile
        private Vector3 velocity = Vector3.zero;

        // The starting position of the projectile
        private Vector3 startingPosition;

        // The damage the projectile inflicts
        [SerializeField]
        private float damage; 

        [Tooltip("How long until the projectile is destroyed")]
        [SerializeField]
        private float timeTillDestruction;
        #endregion

        // Gives access to the velocity field
        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        /// <summary>
        /// Sets the projectile to its starting position
        /// And starts a coroutine that destroys the projectile after a certain time
        /// </summary>
        void Start()
        {
            startingPosition = transform.position;
            StartCoroutine(Util.DestroyTimer(timeTillDestruction, gameObject));
        }

        /// <summary>
        /// Updates the position of the projectile according to velocity
        /// </summary>
        void Update()
        {
            // As long as velocity is not 0,
            // Move it according to velocity
            if (velocity != Vector3.zero)
            {
                transform.position = transform.position + velocity;
            }
        }

        /// <summary>
        /// When the projectile collides with an object check if it is the player
        /// If it is the player, damage them and show they're being damaged
        /// </summary>
        /// <param name="col">The collision that occurred</param>
        void OnCollisionEnter(Collision col)
        {
            // Check if the object in the collision is the player
            if (col.gameObject.GetComponent<Player>())
            {
                // Damage the player
                col.gameObject.GetComponent<Player>().TakeDamage(damage);

                // Start a coroutine to change the player's material to show they've been damaged
                col.gameObject.GetComponent<Player>().StartCoroutine(
                    Util.DamageMaterialTimer(col.gameObject.GetComponentInChildren<SkinnedMeshRenderer>()));
            }

            // Destroy the projectile once it has collided
            Destroy(gameObject); 
        }

        /// <summary>
        /// When the projectile collides with the player block box, it is destroyed and preventing the player from taking damage
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PreventDmg"))
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}