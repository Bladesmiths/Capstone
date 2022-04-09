using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class PlayerShatterSword : MonoBehaviour
    {
        public Vector3 center;

        private void Start()
        {
            center = transform.position;
        }

        private void Update()
        {
            if(transform.childCount == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
