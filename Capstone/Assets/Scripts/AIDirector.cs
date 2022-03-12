using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Bladesmiths.Capstone
{
    public struct EnemyList
    {
        public int groupNumber;
        public List<Enemy> enemies;

    }

    public class AIDirector : SerializedMonoBehaviour
    {
        public GameObject[] enemyPrefabs;
        public List<GameObject> enemyGroups;
        private GameObject currentGroup;
        private int groupCount;
        public LinkedList<EnemyList> allEnemyGroups;
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
                //enemyGroups = new List<GameObject>();
                allEnemyGroups = new LinkedList<EnemyList>();
            }
        }

        private void Start()
        {
            //allEnemyGroups.Add(enemyGroup);
            currentGroup = enemyGroups[0];
        }

        private void Update()
        {
            if(attackQueue.Count > 0)
            {
                CheckForPossibleAttacker();
            }

            //if(allEnemyGroups.First.Value.enemies.Count == 0)
            //{
            //    groupCount++;
            //    if (groupCount != enemyGroups.Count)
            //    {                    
            //        currentGroup.SetActive(false);
            //        currentGroup = enemyGroups[groupCount];
            //        currentGroup.SetActive(true);
            //        allEnemyGroups.RemoveFirst();                    
            //    }
            //    else if(groupCount == enemyGroups.Count)
            //    {
            //        currentGroup.SetActive(false);
            //    }
            //}
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

        public bool CheckEnemyGroup(Enemy e)
        {
            foreach (EnemyList group in allEnemyGroups)
            {
                if (group.enemies.Contains(e))
                {
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// Gets the Enemy group that the parameter is in
        /// </summary>
        /// <param name="e">The Enemy we are checking</param>
        /// <returns></returns>
        public List<Enemy> GetEnemyGroup(Enemy e)
        {
            foreach (EnemyList group in allEnemyGroups)
            {
                if (group.enemies.Contains(e))
                {
                    return group.enemies;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds the Enemy to the enemyGroup
        /// </summary>
        /// <param name="e"></param>
        public void AddToEnemyGroup(Enemy e, int groupNum)
        {
            bool check = false;

            if(e == null)
            {
                return;
            }

            foreach(EnemyList list in allEnemyGroups)
            {
                if(list.groupNumber == groupNum)
                {
                    list.enemies.Add(e);
                    check = true;
                }
            }

            if(!check)
            {
                EnemyList eList = new EnemyList();
                eList.enemies = new List<Enemy>();
                eList.enemies.Add(e);
                eList.groupNumber = groupNum;
                allEnemyGroups.AddLast(eList);
            }         

        }

        /// <summary>
        /// Removes the Enemy from the enemyGroup and the attackQueue
        /// </summary>
        /// <param name="e"></param>
        public void RemoveFromGroups(Enemy e)
        {
            foreach (EnemyList list in allEnemyGroups)
            {
                if (list.enemies.Contains(e))
                {
                    list.enemies.Remove(e);
                }
            }
                        
            //enemyGroup.Sort();
            attackQueue.Remove(e);
        }

        public void ResetBlocks()
        {
            foreach (EnemyList list in allEnemyGroups)
            {
                foreach(Enemy e in list.enemies)
                {
                    e.blocked = false;

                }
            }            
        }

        /// <summary>
        /// Adds the Enemy to the attackQueue
        /// </summary>
        /// <param name="e"></param>
        public void PopulateAttackQueue(Enemy e)
        {
            if (!attackQueue.Contains(e))
            {
                attackQueue.AddLast(e);
            }

        }

        /// <summary>
        /// Allows for the Enemy to attack
        /// </summary>
        /// <param name="e"></param>
        public void AttackPlayer(Enemy e)
        {
            e.CanHit = true;
            e.attackTimer = e.attackTimerMax;
        }

        /// <summary>
        /// Checks to see if there is currently an Enemy attacking
        /// </summary>
        /// <returns></returns>
        public bool EnemyCurrentlyAttacking()
        {
            foreach(EnemyList list in allEnemyGroups)
            {
                foreach(Enemy e in list.enemies)
                {
                    if (e.CanHit)
                    {
                        return true;
                    }
                }                
            }

            return false;
        }

    }
}
