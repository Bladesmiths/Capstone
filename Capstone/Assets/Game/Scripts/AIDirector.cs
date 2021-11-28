using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class AIDirector : MonoBehaviour
    {
        public GameObject[] enemyPrefabs;
        public List<Enemy> enemyGroup;
        public LinkedList<Enemy> attackQueue;
        private static AIDirector instance;
        private bool enemyAttacking;


        public static AIDirector Instance { get => instance;  }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
                attackQueue = new LinkedList<Enemy>();
                enemyGroup = new List<Enemy>();
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
            if(e == null)
            {
                return;
            }

            enemyGroup.Add(e);

        }

        public void RemoveFromGroups(Enemy e)
        {
            if (!enemyGroup.Remove(e))
            {
                return;
            }
            
            enemyGroup.Sort();
            attackQueue.Remove(e);
        }

        public void PopulateAttackQueue()
        {

        }

        public void AttackPlayer()
        {

        }

    }
}
