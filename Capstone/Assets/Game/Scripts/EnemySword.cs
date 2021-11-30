using Bladesmiths.Capstone.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class EnemySword : MonoBehaviour, IDamaging
    {
        private Enemy enemy;

        #region IDamaging Necessaries
        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;
        public event IIdentified.OnDestructionDelegate OnDestruction;
        public float Damage { get; set; }
        public bool Damaging { get; set; }
        public int ID { get; set; }
        public Team ObjectTeam { get; set; }
        public ObjectController ObjectController { get; set; }
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            // Gets all of the info from the Enemy class
            enemy = gameObject.transform.root.GetComponent<Enemy>();
            ID = enemy.ID;
            ObjectTeam = enemy.ObjectTeam;
            Damage = enemy.Damage;

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                enemy.SwordAttack(other.gameObject.GetComponent<Player>().ID);
                //other.gameObject.GetComponent<Player>().TakeDamage(other.gameObject.GetComponent<Player>().ID, Damage);
                //other.gameObject.GetComponent<Player>().Damaging = true;
            }
        }
    }
}
