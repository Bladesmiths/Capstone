using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public class TestingEnemy : Enemy
    {
        [SerializeField]
        private Material damagedMaterial;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        public override void Update()
        {

        }
        
        public override void TakeDamage(float damage)
        {
            StartCoroutine(Util.DamageMaterialTimer(gameObject));
        }
    }
}