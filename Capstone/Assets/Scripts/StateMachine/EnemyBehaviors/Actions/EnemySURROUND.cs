using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone
{
    public struct SeekPoint
    {
        public Vector3 point;
        public float weight;
        public SeekPoint(Vector3 pos, float w)
        {
            point = pos;
            weight = w;
        }
        
    }


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
        public List<SeekPoint> seekList;
        public Vector3 rotVec;

        private bool targetExists;

        [SerializeField]
        private Player player;
        private Enemy enemy;
        public float seekSpeed;

        private Vector3 target;
        public Vector3 desiredPos;
        public Vector3 chosenDir;
        public Vector3 lookPos;
        private int dir;

        [SerializeField]
        private float sideDist;

        public override void OnStart()
        {
            enemy = GetComponent<Enemy>();
            agent = GetComponent<NavMeshAgent>();
            enemy.surrounding = true;
            player = Player.instance;
            allDirections = new Vector3[numRays];
            intrest = new float[numRays];
            danger = new float[numRays];
            fleeList = new List<GameObject>();
            seekList = new List<SeekPoint>();
            enemy.moveTimerMax = Random.Range(0.5f, 1f);
            enemy.moveTimer = enemy.moveTimerMax;
            seekSpeed = 2f;
            enemy.CanHit = false;
            agent.speed = 3.5f;
            enemy.InCombat = true;

            for (int i = 0; i < numRays; i++)
            {
                float angle = i * 2 * Mathf.PI / numRays;
                allDirections[i] = Quaternion.Euler(0, Mathf.Rad2Deg * angle, 0) * Vector3.forward;
            }

            //enemy.InCombat = true;
            enemy.canMove = true;
            enemy.moveVector = Vector3.zero;
            dir = Random.Range(-1, 2);
            dir = dir == 0 ? 1 : dir;

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

            seekList.Add(new SeekPoint(player.transform.position, seekSpeed));

        }

        private void AttackTimer()
        {
            if(enemy.InCombat)
            {
                enemy.attackTimer -= Time.deltaTime;
            }
            if (enemy.attackTimer <= 0)
            {
                Debug.Log("Attack");
                enemy.attackTimer = enemy.attackTimerMax;
                AIDirector.Instance.PopulateAttackQueue(enemy);
            }
        }

        public override TaskStatus OnUpdate()
        {
            Vector3 dist = player.transform.position - transform.position;
            lookPos = dist;
            seekList.Clear();
            //Debug.Log("AttackTimer" + enemy.attackTimer);
            //Debug.Log("AttackTimerMax" + enemy.attackTimerMax);

            if (dist.magnitude > 2.5f)
            {
                seekList.Add(new SeekPoint(player.transform.position, seekSpeed));
            }
            else if (dist.magnitude <= 2.3f)
            {
                seekList.Add(new SeekPoint((dist * -1), 1f));

            }
            else if (dist.magnitude > 2.3f && dist.magnitude < 2.5f)
            {
                seekList.Add(new SeekPoint(transform.position, 1f));
            }

            // Get the perpendicular vector to the distance between the Player and the Enemy
            seekList.Add(new SeekPoint((Vector3.Cross(Vector3.up, dist).normalized * dir) + transform.position, 1f));
            desiredPos = player.transform.position;

            PopulateAll();            
            AttackTimer();

            if (dist.magnitude <= 2.5f)
            {
                enemy.moveTimer -= Time.deltaTime;
            }
            if (enemy.moveTimer <= 0)
            {
                enemy.moveTimer = enemy.moveTimerMax;
                return TaskStatus.Success;
            }

            sideDist = dist.magnitude > 2.5f ? 10f : 0.3f;

            Vector3 movePos = (chosenDir * sideDist) + transform.position;

            if (agent != null)
            {
                agent.SetDestination(movePos);                
            }

            //dist.Normalize();
            Vector3 lookRotVec = new Vector3(dist.x + 0.001f, 0f, dist.z);
            rotVec = lookRotVec;
            if (lookRotVec.magnitude > Mathf.Epsilon)
            {
                Quaternion q = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookRotVec),
                    Time.deltaTime * 5f);
                transform.rotation = q;
            }
            
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            enemy.surrounding = false;            
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
            float mag = 1;

            foreach (SeekPoint vec in seekList)
            {
                mag = (vec.point - transform.position).magnitude;
                //mag = 1 / mag;
                pathDir += (vec.point - transform.position) * vec.weight;
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
            //Gizmos.DrawRay(transform.position, rotVec);

            //if (allDirections == null ||
            //    intrest == null ||
            //    danger == null)
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
            //Gizmos.DrawSphere(seekList[0].point, .3f);
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
