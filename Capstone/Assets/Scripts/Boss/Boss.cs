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

        public List<GameObject> activeProjectiles;

        public static Boss instance;

        public Vector3 lastPosition;
        public int hasntMovedCounter;
        public bool againstWallAgain;

        public Dictionary<string,float> actionCounter;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.Find("Player");
            ObjectController.Instance.AddIdentifiedObject(Team.Enemy, this);

            GetComponent<BehaviorTree>().SetVariableValue("Player", player);

            actionCounter = new Dictionary<string, float>();
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

            //if (damaged)
            //{
            //    timer += Time.deltaTime;
            //
            //    if (timer >= 0.5f)
            //    {
            //        gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            //        damaged = false;
            //        timer = 0f;
            //    }
            //}
        }

        protected override void Die()
        {
            base.Die();

            GetComponent<BehaviorTree>().DisableBehavior();

            //GetComponent<BehaviorTree>().EnableBehavior();

            if (player != null)
            {
                if (timer == 0)
                {
                    GetComponent<BehaviorTree>().DisableBehavior();
                    GetComponent<Animator>().SetBool("Dead", true);
                }

                timer += Time.deltaTime;
                if (timer >= 2)
                    player.GetComponent<Player>().AddToMaxPoints();
            }
            
            //gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            //gameObject.SetActive(false);
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