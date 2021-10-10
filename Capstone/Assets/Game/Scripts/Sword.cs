using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class Sword : MonoBehaviour
    {
        private float damage = 5;

        public Player Player { get; set; }

        void Start()
        {
            

        }

        void Update()
        {

        }

        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.GetComponent<Enemy>())
            {
                Player.SwordAttack(col.gameObject.GetComponent<Enemy>().ID, damage);
            }
        }
    }
}
