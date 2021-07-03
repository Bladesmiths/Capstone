using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class TargetLock : MonoBehaviour
    {
        private List<GameObject> enemies;
        private List<GameObject> visibleEnemies;

        private StarterAssetsInputs _input;

        // Start is called before the first frame update
        void Start()
        {
            enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
            visibleEnemies = FindVisibleEnemies(); 
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LockOnEnemy()
        {
            //if (_input.)
        }

        private List<GameObject> FindVisibleEnemies()
        {
            List<GameObject> visibleFiltered = enemies.Where(x => IsEnemyVisible(x)).ToList(); 

            if (visibleFiltered.Count == 0)
            {
                return null; 
            }
            return visibleFiltered;
        }

        private bool IsEnemyVisible(GameObject enemy)
        {
            RaycastHit hit;
            return (Physics.Linecast(transform.position, enemy.transform.position, out hit) && hit.transform == enemy.transform); 
        }
    }
}
