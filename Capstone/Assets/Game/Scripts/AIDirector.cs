using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class AIDirector : MonoBehaviour
    {
        public GameObject[] enemyPrefabs;
        private List<Enemy> enemyGroup;
        private Queue<Enemy> attackQueue;
        private static AIDirector instance;
        private bool enemyAttacking;


        public static AIDirector Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        private void Start()
        {

        }

        private void Update()
        {
            
        }

        public void AddToEnemyGroup(Enemy e)
        {

        }

        public void RemoveFromEnemyGroup(Enemy e)
        {

        }

        public void PopulateAttackQueue()
        {

        }

        public void AttackPlayer()
        {

        }

    }
}
