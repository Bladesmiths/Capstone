using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public class TestingProjectile : MonoBehaviour, IDamaging
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

        private int id;

        [SerializeField]
        private ObjectController objectController;

        // The event to call when damaging is finished
        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;

        // Testing for damaging system
        [Header("Damaging Timer Fields (Testing)")]
        [SerializeField]
        private float damagingTimerLimit;
        private float damagingTimer;
        private bool damaging;
        #endregion

        // Gives access to the velocity field
        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public int ID { get => id; set => id = value; }
        public ObjectController ObjectController { get => objectController; set => objectController = value; }
        public float Damage { get => damage; }

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

            // Testing
            // If the enemy is currently damaging an object
            if (damaging)
            {
                // Update the timer
                damagingTimer += Time.deltaTime;

                // If the timer is equal to or exceeds the limit
                if (damagingTimer >= damagingTimerLimit)
                {
                    // If the damaging finished event has subcribing delegates
                    // Call it, running all subscribing delegates
                    if (DamagingFinished != null)
                    {
                        DamagingFinished(ID);
                    }
                    // If the damaging finished event doesn't have any subscribing events
                    // Something has gone wrong because damaging shouldn't be true otherwise
                    else
                    {
                        Debug.Log("Damaging Finished Event was not subscribed to correctly");
                    }

                    // Reset fields
                    damagingTimer = 0.0f;
                    damaging = false;
                }
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
                col.gameObject.GetComponent<Player>().TakeDamage(id, damage);

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
                // Testing
                ((TestDataInt)GameObject.Find("TestingController").GetComponent<TestingController>().ReportedData["numBlocks"]).Data.CurrentValue++;

                Destroy(gameObject);
                return;
            }
        }
    }
}