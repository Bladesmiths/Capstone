using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bladesmiths.Capstone.Enums;

namespace Bladesmiths.Capstone
{
    public class BossSword : MonoBehaviour, IDamaging
    {
        #region IDamaging Necessaries
        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;
        public event IIdentified.OnDestructionDelegate OnDestruction;
        public float Damage { get; set; }
        public bool Damaging { get; set; }
        public int ID { get; set; }
        public Team ObjectTeam { get; set; }
        public ObjectController ObjectController { get; set; }
        public GameObject GameObject { get => gameObject; }
        #endregion

        [SerializeField] private int damagingID;
        private float timer;

        private void Awake()
        {
            ObjectController = ObjectController.Instance;
            ObjectTeam = Team.Enemy;
            ObjectController.Instance.AddIdentifiedObject(ObjectTeam, this);
        }

        // Start is called before the first frame update
        void Start()        {
            
            Damage = 20;
            //ID = damagingID;
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                other.gameObject.GetComponent<Player>().TakeDamage(ID, Damage);
                DamagingFinished += other.gameObject.GetComponent<Player>().RemoveDamagingID;
                Damaging = true;
            }
        }

        public void ClearDamaging()
        {
            // If the Boss is currently damaging an object
            if (Damaging)
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
                    //Debug.Log("Damaging Finished Event was not subscribed to correctly");
                }

                // Reset fields
                Damaging = false;
            }
        }
    }
}
