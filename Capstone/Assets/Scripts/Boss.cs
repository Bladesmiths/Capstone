using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Bladesmiths.Capstone.Enums;
using BehaviorDesigner.Runtime;

namespace Bladesmiths.Capstone
{
    public class Boss : Character
    {
        [SerializeField] private GameObject player;
        protected bool damaged = false;
        private float timer;

        [SerializeField] private GameObject activeSword;
        [SerializeField] private Quaternion originalSwordRotation;

        [SerializeField] private float inRangeDistance;

        private NavMeshAgent navAgent;

        //private Sequence rootNode;
        //
        //private RandomWeighted mainPathWeightedSelector;
        //
        //private ActionNode atPlayerCheckNode;
        //private ActionNode moveToPlayerNode;
        //private Selector approachPlayer;
        //
        //private Sequence normalAttackSequence;
        //private ActionNode normalAttack;
        //private RepeatUntilSuccess repeatAttack;
        //private float normalAttackTimer;
        //private NodeStates normalAttackState;
        //
        //private ActionNode strafeLeft;
        //private ActionNode strafeRight;
        //private RandomSelector rngStrafeDirection;
        //private float strafeAmount;
        //
        //private ActionNode wait1Second;
        //private float waitTimer;

        private Vector3 swordRotation;
        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.Find("Player");
            ObjectController.Instance.AddIdentifiedObject(Team.Enemy, this);

            GetComponent<BehaviorTree>().SetVariableValue("Player", player);

            navAgent = GetComponent<NavMeshAgent>();
            originalSwordRotation = activeSword.transform.rotation;
            swordRotation = Vector3.zero;

            //atPlayerCheckNode = new ActionNode(CheckAtPlayer);
            //moveToPlayerNode = new ActionNode(MoveToPlayer);
            //
            //normalAttack = new ActionNode(Attack);
            //repeatAttack = new RepeatUntilSuccess(normalAttack);
            //
            //wait1Second = new ActionNode(Wait1Second);
            //
            //strafeLeft = new ActionNode(StrafeLeft);
            //strafeRight = new ActionNode(StrafeRight);
            //
            //rngStrafeDirection = new RandomSelector(new List<Node>
            //{
            //    strafeLeft,
            //    strafeRight,
            //});
            //
            //approachPlayer = new Selector(new List<Node> 
            //{
            //    atPlayerCheckNode,
            //    moveToPlayerNode,
            //});
            //
            //normalAttackSequence = new Sequence(new List<Node>
            //{
            //    approachPlayer,
            //    repeatAttack,
            //});
            //
            //mainPathWeightedSelector = new RandomWeighted(new List<Node>
            //{
            //    normalAttackSequence,
            //    wait1Second,
            //    rngStrafeDirection,
            //}, new List<Vector2>
            //{
            //    new Vector2(1,50),  // 50%
            //    new Vector2(51,75), // 25%
            //    new Vector2(76,100) // 25%
            //});
            //
            //rootNode = new Sequence(new List<Node>
            //{
            //    mainPathWeightedSelector
            //});
        }

        // Update is called once per frame
        void Update()
        {
            //SeekPlayer();

            //EvaluateBehaviorTree();

            if(Health <= 0)
            {
                Die();
            }

            if (damaged)
            {
                timer += Time.deltaTime;

                if (timer >= 0.5f)
                {
                    gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                    damaged = false;
                    timer = 0f;
                }
            }
        }

        //private NodeStates CheckAtPlayer()
        //{
        //    if (Vector3.Distance(transform.position, player.transform.position) <= inRangeDistance)
        //    {
        //        // Stop seeking
        //        navAgent.destination = transform.position;
        //        return NodeStates.SUCCESS;
        //    }
        //    else
        //    {
        //        return NodeStates.FAILURE;
        //    }
        //}
        //
        //private NodeStates MoveToPlayer()
        //{
        //    SeekPlayer();
        //    return NodeStates.FAILURE;
        //}
        //
        //private NodeStates Attack()
        //{
        //    // If the sword has not swung all the way down
        //    if(swordRotation.x >= 100)
        //    {
        //        // Once the sword has reached the end of the swing, hold it there for a small amount of time
        //        if (normalAttackTimer >= 0.5f)
        //        {
        //            // Reset some variables before returning success
        //            normalAttackTimer = 0;
        //            swordRotation = Vector3.zero;
        //            activeSword.transform.forward = transform.forward;
        //            normalAttackState = NodeStates.SUCCESS;
        //            return NodeStates.SUCCESS;
        //        }
        //        else
        //        {
        //            normalAttackTimer += Time.deltaTime;
        //            normalAttackState = NodeStates.RUNNING;
        //            return NodeStates.RUNNING;
        //        }
        //    }
        //    // Rotate the sword
        //    else
        //    {
        //        // I hate Unity and Euler......
        //        // Jank rotations because I hate Euler and rapidly spinning swords in axis' that I didn't change
        //        swordRotation.x += 180 * Time.deltaTime;
        //        activeSword.transform.Rotate(180*Time.deltaTime, 0f, 0f);
        //        normalAttackState = NodeStates.RUNNING;
        //        return NodeStates.RUNNING;
        //    }
        //}
        //
        //private void SeekPlayer()
        //{
        //    navAgent.destination = player.transform.position;
        //}
        //
        //private NodeStates StrafeLeft()
        //{
        //    if(strafeAmount == 0)
        //    {
        //        strafeAmount = Random.Range(2, 10);
        //    }
        //
        //    if(strafeAmount <= 0)
        //    {
        //        strafeAmount = 0;
        //        return NodeStates.SUCCESS;
        //    }
        //
        //    transform.LookAt(player.transform);
        //    transform.Translate(-10 * Time.deltaTime, 0, 0);
        //    strafeAmount -= Time.deltaTime;
        //
        //    return NodeStates.RUNNING;
        //}
        //
        //private NodeStates StrafeRight()
        //{
        //    if (strafeAmount == 0)
        //    {
        //        strafeAmount = Random.Range(2, 10);
        //    }
        //
        //    if (strafeAmount <= 0)
        //    {
        //        strafeAmount = 0;
        //        return NodeStates.SUCCESS;
        //    }
        //
        //    transform.LookAt(player.transform);
        //    transform.Translate(10 * Time.deltaTime, 0, 0);
        //    strafeAmount -= Time.deltaTime;
        //
        //    return NodeStates.RUNNING;
        //}
        //
        //private NodeStates Wait1Second()
        //{
        //    if (waitTimer <= 0)
        //    {
        //        waitTimer = 1.0f;
        //        return NodeStates.SUCCESS;
        //    }
        //
        //    waitTimer -= Time.deltaTime;
        //    return NodeStates.RUNNING;
        //}
        //
        //private IEnumerator Execute()
        //{
        //    rootNode.Evaluate();
        //    yield return null;
        //}
        //
        //public void EvaluateBehaviorTree()
        //{
        //    // If the normal attack is not in the process of running, start from the beginning of the tree
        //    if (normalAttackState != NodeStates.RUNNING)
        //    {
        //        rootNode.Evaluate();
        //    }
        //    // Otherwise jump right to the normal attack node
        //    else
        //    {
        //        normalAttack.Evaluate();
        //    }
        //    //StartCoroutine(Execute());
        //}

        protected override void Die()
        {
            if (player != null)
            {
                player.GetComponent<Player>().AddToMaxPoints();
            }
            
            gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            gameObject.SetActive(false);
        }

        public override void Respawn()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Subtract an amount of damage from the character's health
        /// </summary>
        /// <param name="damagingID">The id of the damaging object that is damaging this character</param>
        /// <param name="damage">The amount of damage to be subtracted</param>
        /// <returns>Returns a boolean indicating whether damage was taken or not</returns>
        public override float TakeDamage(int damagingID, float damage)
        {
            // The result of Character's Take Damage
            // Was damage taken or not
            float damageResult = base.TakeDamage(damagingID, damage);

            // If damage was taken
            // Change the object to red and set damaged to true
            if (damageResult > 0)
            {
                gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.red;

                damaged = true;
            }

            // Return whether damage was taken or not
            return damageResult;
        }
    }
}