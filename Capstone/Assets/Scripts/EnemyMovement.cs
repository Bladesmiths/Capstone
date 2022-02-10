using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Bladesmiths.Capstone
{
    public class EnemyMovement : Action
    {
        public float maxSpeed = 10f;
        public float steerForce = .2f;
        public float lookAhead = 10;
        public int numRays = 8;

        private Vector3[] allDirections;
        private float[] intrest;
        private float[] danger;

        private Vector3 chosenDir;
        private Vector3 velocity;
        private Vector3 accel;

        private NavMeshAgent agent;

        [SerializeField]
        private GameObject seekTarget;
        private GameObject currentSeek;
        [SerializeField]
        private GameObject fleeTarget;
        private List<GameObject> fleeList;
        private bool targetExists;

        [SerializeField]
        private Player player;
        private NavMeshPath currentPath;

        private Vector3 target;
        private Vector3 desiredPos;



        public override void OnStart()
        {
            player = Player.instance;
            allDirections = new Vector3[numRays];
            intrest = new float[numRays];
            danger = new float[numRays];
            agent = GetComponent<NavMeshAgent>();
            currentPath = new NavMeshPath();
            targetExists = false;
            fleeList = new List<GameObject>();

            for (int i = 0; i < numRays; i++)
            {
                float angle = i * 2 * Mathf.PI / numRays;
                allDirections[i] = Quaternion.Euler(0, Mathf.Rad2Deg * angle, 0) * Vector3.forward;

            }

            desiredPos = player.transform.position;
            targetExists = true;

            //fleeList = 

        }

        public override TaskStatus OnUpdate()
        {
            //Vector3 mousePos = Input.mousePosition;
            //target = Camera.main.ScreenToWorldPoint(mousePos);
            //target.y = transform.position.y;
            PopulateAll();


            // Movement
            //Vector3 desiredVelocity = Quaternion.Euler(0, transform.eulerAngles.y, 0) * chosenDir * maxSpeed;
            //velocity = Vector3.Lerp(transform.position, desiredVelocity, steerForce);

            //if (Input.GetMouseButtonDown(0) && !targetExists)
            //{
            //    //currentSeek = Instantiate(seekTarget, target, Quaternion.identity);
            //    //desiredPos = currentSeek.transform.position;

            //    targetExists = true;
            //}
            //else if (Input.GetMouseButtonDown(1))
            //{
            //    //fleeList.Add(Instantiate(fleeTarget, target, Quaternion.identity));

            //}

            desiredPos = player.transform.position;
            Vector3 movePos = chosenDir + transform.position;
            Debug.Log(desiredPos);




            GetComponent<Enemy>().moveVector = movePos;
            //GetComponent<Enemy>().rotateVector = movePos - transform.position;

            return TaskStatus.Running;

        }

        public override void OnEnd()
        {
            base.OnEnd();
        }

        public void PopulateAll()
        {
            IntrestPopulate();
            DangerPopulate();
            ChooseDirection();

        }

        private void IntrestPopulate()
        {
            Vector3 pathDir = Vector3.zero;

            pathDir = desiredPos - transform.position;
            //pathDir = agent.steeringTarget;
            //pathDir.y = transform.position.y;

            for (int i = 0; i < numRays; i++)
            {
                float d = Vector3.Dot((Quaternion.Euler(0, transform.eulerAngles.y, 0) * allDirections[i]).normalized, pathDir.normalized);
                intrest[i] = Mathf.Max(0, d);

            }

        }

        private void DangerPopulate()
        {
            Vector3 pathDir = Vector3.zero;

            foreach (GameObject gO in fleeList)
            {
                pathDir += gO.transform.position - transform.position;

            }

            pathDir.Normalize();

            for (int i = 0; i < numRays; i++)
            {
                Vector3 direction = Quaternion.Euler(0, transform.eulerAngles.y, 0) * allDirections[i].normalized;

                //bool result = Physics.Raycast(transform.position, direction, lookAhead);//~LayerMask.GetMask(name));
                float d = Vector3.Dot(direction, pathDir);
                danger[i] = Mathf.Max(0, d);

                //danger[i] = result ? 1f : 0f;

            }

        }

        private void ChooseDirection()
        {
            //for (int i = 0; i < numRays; i++)
            //{
            //    if(danger[i] > 0f)
            //    {
            //        intrest[i] *= 0f;
            //    }

            //}

            chosenDir = Vector3.zero;
            for (int i = 0; i < numRays; i++)
            {
                if (danger[i] == 0)
                {
                    chosenDir += allDirections[i] * intrest[i];
                }
                else
                {
                    chosenDir -= allDirections[i] * danger[i];

                }

            }

            chosenDir.Normalize();

        }

        public override void OnDrawGizmos()
        {
            if (allDirections == null ||
                intrest == null ||
                danger == null)
            {
                return;
            }

            for (int i = 0; i <= numRays - 1; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, allDirections[i] * danger[i] * 5f);
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, allDirections[i] * intrest[i] * 5f);
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, chosenDir * 5f);
                Gizmos.color = Color.gray;
                DrawCircle();


            }
        }

        public void DrawCircle()
        {
            float corners = 30; // How many corners the circle should have
            float size = 5; // How wide the circle should be
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
