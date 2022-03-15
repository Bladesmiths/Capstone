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

        private Player player;

        [Tooltip("How long until the projectile is destroyed")]
        [SerializeField]
        public float timeTillDestruction;

        [SerializeField]
        private ObjectController objectController;

        // The event to call when damaging is finished
        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;
        public event IIdentified.OnDestructionDelegate OnDestruction;

        // Testing for damaging system
        [Header("Damaging Timer Fields (Testing)")]
        [SerializeField]
        private float damagingTimerLimit;
        private float damagingTimer;
        private bool damaging;

        public bool canMove = true;
        #endregion

        // Gives access to the velocity field
        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public GameObject GameObject { get; set; }
        public int ID { get; set; }
        public ObjectController ObjectController { get => objectController; set => objectController = value; }
        public Enums.Team ObjectTeam { get; set; }
        public float Damage { get => damage; }
        public bool Damaging { get => damaging; set => damaging = value; }

        /// <summary>
        /// Sets the projectile to its starting position
        /// And starts a coroutine that destroys the projectile after a certain time
        /// </summary>
        void Start()
        {
            startingPosition = transform.position;
            StartCoroutine(Util.DestroyTimer(timeTillDestruction, gameObject));
            player = GameObject.Find("Player").GetComponent<Player>();
            GameObject = gameObject;
        }

        /// <summary>
        /// Updates the position of the projectile according to velocity
        /// </summary>
        void Update()
        {
            // As long as velocity is not 0,
            // Move it according to velocity
            if (velocity != Vector3.zero && canMove)
            {
                transform.position = transform.position + velocity * Time.deltaTime;
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
        /// If it is the player, damage them 
        /// If it is the parry detector, change the projectile's velocity
        /// If it is the boss, damage them
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == "Parry Detector")
            {
                // Testing
                //((TestDataInt)GameObject.Find("TestingController").GetComponent<TestingController>().ReportedData["numBlocks"]).Data.CurrentValue++;

                Velocity = new Vector3(-Velocity.x, 0, -Velocity.z);
                ObjectTeam = Enums.Team.Player;
                ObjectController[ID].ObjectTeam = ObjectTeam;

                return;
            }

            //if(other.gameObject.name == "Block Detector")
            //{
            //    Destroy(gameObject);

            //}

            if (other.gameObject.GetComponent<IDamageable>() != null)
            {
                if (other.gameObject.GetComponent<Player>())
                {
                    // Check if the object in the collision is the player
                    if ((player.GetPlayerFSMState() != Enums.PlayerCondition.F_Blocking) &&
                        (player.GetPlayerFSMState() != Enums.PlayerCondition.F_ParryAttempt))
                    {
                        // Damage the player
                        //player.TakeDamage(ID, damage);
                        ((IDamageable)ObjectController[player.ID].IdentifiedObject).TakeDamage(ID, damage);
                    }
                }
                else
                {
                    ((IDamageable)ObjectController[other.gameObject.GetComponent<IDamageable>().ID].IdentifiedObject).TakeDamage(ID, damage);
                }
                Destroy(gameObject, 2f);
            }
            else
            {
                Destroy(gameObject, 15f);
            }
        }

        private void OnDestroy()
        {
            if (DamagingFinished != null)
            {
                DamagingFinished(ID);
            }

            if (OnDestruction != null)
            {
                OnDestruction(ID);
            }
        }
    }
}