using Bladesmiths.Capstone.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class BossCylinder : MonoBehaviour, IDamaging
    {
        [SerializeField] private GameObject well;
        [SerializeField] private float speed;
        [SerializeField] private float damage;

        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;
        public event IIdentified.OnDestructionDelegate OnDestruction;

        private Player player;


        // Testing for damaging system
        [Header("Damaging Timer Fields (Testing)")]
        [SerializeField]
        private float damagingTimerLimit;
        private float damagingTimer;
        private bool damaging;

        public int ID { get; set; }
        public float Damage => damage;
        public ObjectController ObjectController { get; set; }
        public Team ObjectTeam { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.RotateAround(well.transform.position, well.transform.up, speed * Time.deltaTime);

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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform.root.CompareTag("Player") == true)
            {
                if (other.gameObject.transform.root.GetComponent<Player>().GetPlayerFSMState().ID != Enums.PlayerCondition.F_Blocking)
                {
                    Player player = other.gameObject.transform.root.gameObject.GetComponent<Player>();
                    // Check if the player has already been hit by this object
                    //player.TakeDamage(id, damage);
                    ((IDamageable)ObjectController[player.ID].IdentifiedObject).TakeDamage(ID, damage);
                }

                damaging = true;
            }
        }
    }
}
