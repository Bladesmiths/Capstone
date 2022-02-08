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
            if(attackQueue.Count > 0)
            {
                CheckForPossibleAttacker();
            }
        }

        /// <summary>
        /// Checks to see if there is already an Enemy attacking
        /// </summary>
        public void CheckForPossibleAttacker()
        {
            if(!EnemyCurrentlyAttacking())
            {
                AttackPlayer(attackQueue.First.Value);
                attackQueue.RemoveFirst();
            }
            
        }

        /// <summary>
        /// Adds the Enemy to the enemyGroup
        /// </summary>
        /// <param name="e"></param>
        public void AddToEnemyGroup(Enemy e)
        {
            if(e == null)
            {
                return;
            }

            enemyGroup.Add(e);

        }

        /// <summary>
        /// Removes the Enemy from the enemyGroup and the attackQueue
        /// </summary>
        /// <param name="e"></param>
        public void RemoveFromGroups(Enemy e)
        {
            if (!enemyGroup.Remove(e))
            {
                return;
            }
            
            enemyGroup.Sort();
            attackQueue.Remove(e);
        }

        public void ResetBlocks()
        {
            foreach(Enemy e in enemyGroup)
            {
                e.blocked = false;
            }
        }

        /// <summary>
        /// Adds the Enemy to the attackQueue
        /// </summary>
        /// <param name="e"></param>
        public void PopulateAttackQueue(Enemy e)
        {            
            attackQueue.AddLast(e);

        }

        /// <summary>
        /// Allows for the Enemy to attack
        /// </summary>
        /// <param name="e"></param>
        public void AttackPlayer(Enemy e)
        {
            e.CanHit = true;
        }

        /// <summary>
        /// Checks to see if there is currently an Enemy attacking
        /// </summary>
        /// <returns></returns>
        public bool EnemyCurrentlyAttacking()
        {
            foreach(Enemy e in enemyGroup)
            {
                if(e.CanHit)
                {
                    return true; 
                }
            }

            return false;
        }

    }
}
