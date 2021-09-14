using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
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
            StartCoroutine("DamageMaterialTimer");
        }

        IEnumerable DamageMaterialTimer()
        {
            Material originalMaterial = gameObject.GetComponent<MeshRenderer>().material; 
            gameObject.GetComponent<MeshRenderer>().material = damagedMaterial;

            yield return new WaitForSeconds(2);

            gameObject.GetComponent<MeshRenderer>().material = originalMaterial;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                TakeDamage(0);
            }
        }
    }
}