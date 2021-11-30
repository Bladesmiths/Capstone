using Bladesmiths.Capstone.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class EnemySword : MonoBehaviour, IDamaging
    {
        private Enemy enemy;

        public event IDamaging.OnDamagingFinishedDelegate DamagingFinished;
        public event IIdentified.OnDestructionDelegate OnDestruction;

        public float Damage { get; set; }

        public bool Damaging { get; set; }
        public int ID { get; set; }
        public Team ObjectTeam { get; set; }
        public ObjectController ObjectController { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            enemy = gameObject.transform.root.GetComponent<Enemy>();
            ID = enemy.ID;
            ObjectTeam = enemy.ObjectTeam;

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
            }
        }
    }
}
