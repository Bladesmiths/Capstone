using Bladesmiths.Capstone.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class EnemySword : MonoBehaviour
    {
        private Enemy enemy;
        // Start is called before the first frame update
        void Start()
        {
            enemy = gameObject.transform.root.GetComponent<Enemy>();


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
