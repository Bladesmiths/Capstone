using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class Sword : MonoBehaviour
    {
        void Start()
        {
            

        }

        void Update()
        {

        }

        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.GetComponent<Enemy>())
            {
                col.gameObject.GetComponent<Enemy>().Damaged();

            }

        }


    }
}