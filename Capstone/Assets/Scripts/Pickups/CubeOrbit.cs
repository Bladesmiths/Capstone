using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class CubeOrbit : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Transform rootObject = null;
        private float orbitSpeed = 30;
        private Vector3 orbitAxis = Vector3.up;
        private OrbitManager orbitManager;
        #endregion

        // Update is called once per frame
        void Update()
        {
            // If there is a root object and the orbit manager hasn't been turned off
            // Rotate the cube around the given point and axis
            if (rootObject != null && orbitManager.Active)
            {
                transform.RotateAround(transform.position, orbitAxis, orbitSpeed * Time.deltaTime);
                Debug.DrawLine(transform.position, rootObject.transform.position, Color.red);
            }
        }

        /// <summary>
        /// Initializes all important values for the Cube Orbit because it needs paramaters 
        /// </summary>
        /// <param name="root">The root object it should orbit around</param>
        /// <param name="axis">The axis it should rotate around</param>
        /// <param name="speed">The speed it should orbit at</param>
        /// <param name="manager">The Orbit Manager of its parent object</param>
        public void Init(Transform root, Vector3 axis, float speed, OrbitManager manager)
        {
            rootObject = root;
            orbitAxis = axis;
            orbitSpeed = speed;
            orbitManager = manager;
        }
    }
}