using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public class TestingProjectile : MonoBehaviour
    {
        #region Fields
        private Vector3 velocity = Vector3.zero;
        private Vector3 startingPosition;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            startingPosition = transform.position;
            StartCoroutine(Util.DestroyTimer(5, gameObject));
        }

        // Update is called once per frame
        void Update()
        {
            if (velocity != Vector3.zero)
            {
                transform.position = transform.position + velocity;
            }
        }

        public void SetValues(Vector3 givenVelocity)
        {
            velocity = givenVelocity;
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.GetComponent<Player>())
            {
                col.gameObject.GetComponent<Player>().TakeDamage(0);
                col.gameObject.GetComponent<Player>().StartCoroutine(Util.DamageMaterialTimer(col.gameObject.GetComponentInChildren<SkinnedMeshRenderer>()));
            }

            Destroy(gameObject); 
        }
    }
}