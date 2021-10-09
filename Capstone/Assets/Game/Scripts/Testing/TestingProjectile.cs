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

        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;

        // Testing for damaging system
        private float damagingTimer;
        [SerializeField]
        private float damagingTimerLimit;
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
            if (damaging)
            {
                damagingTimer += Time.deltaTime;

                if (damagingTimer >= damagingTimerLimit)
                {
                    if (DamagingFinished != null)
                    {
                        DamagingFinished(ID);
                    }
                    else
                    {
                        Debug.Log("Damaging Finished Event was not subscribed to correctly");
                    }
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