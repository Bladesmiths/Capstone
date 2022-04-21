using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using Bladesmiths.Capstone.Enums;
using Sirenix.OdinInspector;
using Bladesmiths.Capstone.Testing;


namespace Bladesmiths.Capstone
{
    public class Enemy_Ranged : Enemy
    {
        public LayerMask PlayerLayer;
        public Transform shootLoc;
        public GameObject projectilePrefab;
        

        public Vector3 projectileVelocity;
        
        /// <summary>
        /// Adds the picked up sword to the Player's list of currently obtained swords
        /// </summary>
        /// <param name="sword"></param>
        [Button("Fire Projectile")]
        public void FireProjectile()
        {
            GameObject newProjectile = MonoBehaviour.Instantiate(projectilePrefab, shootLoc.position, Quaternion.identity);
            newProjectile.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            TestingProjectile projectileComponent = newProjectile.GetComponent<TestingProjectile>();
            projectileComponent.Velocity = Quaternion.Euler(0, transform.eulerAngles.y, 0) * projectileVelocity;
            projectileComponent.canMove = true;

            ObjectController.AddIdentifiedObject(Enums.Team.Enemy, projectileComponent);
        }

        public override void Awake()
        {
            base.Awake();

        }

        public override void Start()
        {
            base.Start();
            
        }

        public override void Update()
        {
            base.Update();

        }

        void OnCollisionEnter(Collision collision)
        {

        }       

        public override void Respawn()
        {

        }
    }
}
