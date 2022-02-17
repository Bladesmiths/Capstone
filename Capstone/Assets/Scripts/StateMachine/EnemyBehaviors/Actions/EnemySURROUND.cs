using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone
{
    public class EnemySURROUND : Action
    {
        public float maxSpeed = 10f;
        public float steerForce = .2f;
        public float lookAhead = 10;
        public int numRays = 8;

        public Vector3[] allDirections;    // The array of possible directions the Enemy can move in
        public float[] intrest;     // The array of magnitudes that are multiplied by the allDirections array to make the Enemy move in a specific direction
        public float[] danger;      // The array of magnitudes that are multiplied by the allDirections array to make the Enemy move away from specific objects

        private Vector3 velocity;

        private NavMeshAgent agent;

        [SerializeField]
        private GameObject seekTarget;
        private GameObject currentSeek;
        [SerializeField]
        private GameObject fleeTarget;
        public List<GameObject> fleeList;
        public List<Vector3> seekList;

        private bool targetExists;

        [SerializeField]
        private Player player;
        private Enemy enemy;
        private NavMeshPath currentPath;

        private Vector3 target;
        public Vector3 desiredPos;
        public Vector3 chosenDir;
        public Vector3 movemntPos;
        public Vector3 enemyPos;
        private int dir;
        private float seekAgainTimer;
        private float seekAgainTimerMax;

        public override void OnStart()
        {
            enemy = GetComponent<Enemy>();
            enemy.surrounding = true;
            player = Player.instance;
            allDirections = new Vector3[numRays];
            intrest = new float[numRays];
            danger = new float[numRays];
            fleeList = new List<GameObject>();
            seekList = new List<Vector3>();
            seekAgainTimerMax = 2f;
            seekAgainTimer = seekAgainTimerMax;

            for (int i = 0; i < numRays; i++)
            {
                float angle = i * 2 * Mathf.PI / numRays;
                allDirections[i] = Quaternion.Euler(0, Mathf.Rad2Deg * angle, 0) * Vector3.forward;

            }
            enemy.InCombat = true;
            enemy.canMove = true;
            enemy.moveVector = Vector3.zero;
            dir = Random.Range(-1, 2);
            Debug.Log("1: "+dir);

            //Vector3 dist = player.transform.position - transform.position;


            // Get the perpendicular vector to the distance between the Player and the Enemy
            //desiredPos = (Vector3.Cross(Vector3.up, dist) * dir) + transform.position;

            //desiredPos = player.transform.position;

            foreach (Enemy element in AIDirector.Instance.GetEnemyGroup(enemy))
            {
                if(element != enemy)
                {
                    fleeList.Add(element.gameObject);
                }
            }

            seekList.Add(desiredPos);
            //seekList.Add(player.transform.position);

        }

        private void AttackTimer()
        {
            enemy.attackTimer -= Time.deltaTime;
            if (enemy.attackTimer <= 0)
            {
                AIDirector.Instance.PopulateAttackQueue(enemy);
                enemy.attackTimer = enemy.attackTimerMax;
            }

        }

        public override TaskStatus OnUpdate()
        {            

            Vector3 dist = player.transform.position - transform.position;

            // Get the perpendicular vector to the distance between the Player and the Enemy
            seekList[0] = (Vector3.Cross(Vector3.up, dist).normalized * dir) + transform.position;
            PopulateAll();

            //if ((seekList[0] - transform.position).magnitude <= 0.5f)
            //{
            //    // Rest everything
            //    seekAgainTimer -= Time.deltaTime;
            //    Debug.Log("2: " + dir);

            seekAgainTimer -= Time.deltaTime;
            //}
            //Debug.Log(seekAgainTimer);
            AttackTimer();


            if (seekAgainTimer <= 0)
            {
                seekAgainTimer = seekAgainTimerMax;

                dir = Random.Range(-1, 2);
                Debug.Log("3: " + dir);

            }

            Vector3 movePos = chosenDir + transform.position;

            if (dir == 0)
            {
                movePos = transform.position;
            }
            //Debug.Log("Move Position: " + movePos);

            enemy.moveVector = movePos;
            enemy.rotateVector = dist;

            return TaskStatus.Running;

        }

        public override void OnEnd()
        {
            base.OnEnd();
            enemy.surrounding = false;
            //desiredPos = Vector3.zero;
            //movemntPos = Vector3.zero;
            //chosenDir = Vector3.zero;
            //enemyPos = Vector3.zero;
            //allDirections = null;
            //intrest = null;
            //danger = null;
            enemy.canMove = false;

        }

        /// <summary>
        /// Helper method for combining populate methods
        /// </summary>
        public void PopulateAll()
        {
            IntrestPopulate();
            DangerPopulate();
            ChooseDirection();
        }

        /// <summary>
        /// Populates the intrest array with the magnitudes for 
        /// </summary>
        private void IntrestPopulate()
        {
            Vector3 pathDir = Vector3.zero;
            //pathDir = desiredPos - transform.position;
            float mag = 0;

            foreach (Vector3 vec in seekList)
            {
                mag = (vec - transform.position).magnitude;
                mag = 1 / mag;
                pathDir += (vec - transform.position) * mag;
            }

            //mag = 1 / mag;
            pathDir.Normalize();

            for (int i = 0; i < numRays; i++)
            {
                // Gets the Dot product between the desired position(the Player's pos) and all of the possible directions
                float d = Vector3.Dot((Quaternion.Euler(0, transform.eulerAngles.y, 0) * allDirections[i]).normalized, pathDir.normalized);
                intrest[i] = Mathf.Max(0, d);// * mag;
            }
        }

        /// <summary>
        /// Populates the danger array from the fleeList of GameObjects
        /// </summary>
        private void DangerPopulate()
        {
            Vector3 pathDir = Vector3.zero;

            // Combines all of the dangers into one vector
            foreach (GameObject gO in fleeList)
            {
                pathDir += gO.transform.position - transform.position;
            }

            float mag = pathDir.magnitude;
            mag = 1 / mag;
            pathDir.Normalize();

            for (int i = 0; i < numRays; i++)
            {
                if (pathDir == Vector3.zero)
                {
                    danger[i] = 0;
                }
                else
                {
                    Vector3 direction = Quaternion.Euler(0, transform.eulerAngles.y, 0) * allDirections[i].normalized;

                    // Gets the Dot Product between the possible directions
                    float d = Vector3.Dot(direction, pathDir);
                    danger[i] = Mathf.Max(0, d) * mag;
                }
            }
        }

        /// <summary>
        /// Chooses the direction for the Player to move towards
        /// </summary>
        private void ChooseDirection()
        {
            chosenDir = Vector3.zero;
            for (int i = 0; i < numRays; i++)
            {
                if (danger[i] == 0)
                {
                    chosenDir += Quaternion.Euler(0, transform.eulerAngles.y, 0) * allDirections[i].normalized * intrest[i];
                }
                else if(danger[i] != 0 && intrest[i] != 0)
                {
                    Vector3 combo = Quaternion.Euler(0, transform.eulerAngles.y, 0) * allDirections[i].normalized * intrest[i] -
                        Quaternion.Euler(0, transform.eulerAngles.y, 0) * allDirections[i].normalized * danger[i];

                    chosenDir += combo;
                }
                else
                {
                    chosenDir -= Quaternion.Euler(0, transform.eulerAngles.y, 0) * allDirections[i].normalized * danger[i];
                }
            }

            chosenDir.Normalize();
        }

        /// <summary>
        /// Draws a UI element around the Enemy
        /// </summary>
        public override void OnDrawGizmos()
        {
            //Debug.Log(allDirections);
            //if (allDirections == null ||
            //    intrest == null ||
            //    danger == null || 
            //    allDirections[0] == null)
            //{
            //    return;
            //}

            //for (int i = 0; i <= numRays - 1; i++)
            //{
            //    Gizmos.color = Color.red;
            //    Gizmos.DrawRay(transform.position, Quaternion.Euler(0, transform.eulerAngles.y, 0) * allDirections[i].normalized * danger[i] * 2f);
            //    Gizmos.color = Color.green;
            //    Gizmos.DrawRay(transform.position, Quaternion.Euler(0, transform.eulerAngles.y, 0) * allDirections[i].normalized * intrest[i] * 2f);               

            //}
            //Gizmos.color = Color.blue;
            //Gizmos.DrawRay(transform.position, chosenDir * 2f);
            //Gizmos.color = Color.cyan;
            //Gizmos.DrawSphere(chosenDir + transform.position, .3f);
            //Gizmos.color = Color.magenta;
            //Gizmos.DrawSphere(seekList[0], .3f);
            //Gizmos.color = Color.gray;
            //DrawCircle();
        }

        public void DrawCircle()
        {
            float corners = 30; // How many corners the circle should have
            float size = 2; // How wide the circle should be
            Vector3 origin = transform.position; // Where the circle will be drawn around
            Vector3 startRotation = transform.right * size; // Where the first point of the circle starts
            Vector3 lastPosition = origin + startRotation;
            float angle = 0;
            while (angle <= 360)
            {
                angle += 360 / corners;
                Vector3 nextPosition = origin + (Quaternion.Euler(0, angle, 0) * startRotation);
                Gizmos.DrawLine(lastPosition, nextPosition);
                //Gizmos.DrawSphere(nextPosition, 1);

                lastPosition = nextPosition;
            }
        }
    }
}
