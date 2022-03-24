using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class CubeSpin : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private float rotationDegrees;
        private int axisChoice;
        #endregion

        #region Properties
        [field : SerializeField]
        public OrbitManager OrbitManager { get; set; }
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            axisChoice = Random.Range(0, 3);
        }

        // Update is called once per frame
        void Update()
        {
            // Don't keep spinning if the orbit manager isn't active
            // Stops it from spinning on the ground
            if (OrbitManager.Active)
            {
                // Randomizes the direction of the spin
                Vector3 rotation = new Vector3();

                switch (axisChoice)
                {
                    case 0:
                        rotation.x = rotationDegrees;
                        break;
                    case 1:
                        rotation.y = rotationDegrees;
                        break;
                    case 2:
                        rotation.z = rotationDegrees;
                        break;
                }

                // Rotates the cube
                transform.Rotate(rotation * Time.deltaTime);
            }
        }
    }
}