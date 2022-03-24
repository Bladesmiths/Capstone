using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public class OrbitManager : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private GameObject orbitCubePrefab;
        [SerializeField]
        private GameObject orbitCubeParent;
        [SerializeField]
        private float diagOrbitRadius;
        [SerializeField]
        private float orthOrbitRadius;
        [SerializeField]
        private int numCubes;
        [SerializeField]
        private float orbitSpeed;
        #endregion

        #region Properties
        [field : SerializeField]
        public bool Active { get; private set; }
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            // Starts the orbit manager as active
            Active = true;

            // Creates the 4 orbits
            for (int i = 0; i < numCubes; i++) {
                float x = orbitCubeParent.transform.position.x + (diagOrbitRadius * Mathf.Cos((2 * i * Mathf.PI) / numCubes));
                float y = orbitCubeParent.transform.position.y;
                float z = orbitCubeParent.transform.position.z + (diagOrbitRadius * Mathf.Sin((2 * i * Mathf.PI) / numCubes));

                GameObject orbitCube1 = Instantiate(orbitCubePrefab, 
                    new Vector3(x, orbitCubeParent.transform.position.y, z), 
                    Quaternion.identity, orbitCubeParent.transform);
                GameObject orbitCube2 = Instantiate(orbitCubePrefab, 

                    new Vector3(x, orbitCubeParent.transform.position.y, z), 
                    Quaternion.identity, orbitCubeParent.transform);

                x = orbitCubeParent.transform.position.x + (orthOrbitRadius * Mathf.Cos((2 * i * Mathf.PI) / numCubes));
                z = orbitCubeParent.transform.position.z + (orthOrbitRadius * Mathf.Sin((2 * i * Mathf.PI) / numCubes));

                GameObject orbitCube3 = Instantiate(orbitCubePrefab, 
                    new Vector3(x, orbitCubeParent.transform.position.y, z), 
                    Quaternion.identity, orbitCubeParent.transform);
                GameObject orbitCube4 = Instantiate(orbitCubePrefab, 

                    new Vector3(x, orbitCubeParent.transform.position.y, z), 
                    Quaternion.identity, orbitCubeParent.transform);
                
                orbitCube1.transform.RotateAround(transform.position, Vector3.forward, 45);
                orbitCube1.GetComponent<CubeOrbit>().Init(transform, new Vector3(-1, 1, 0).normalized, orbitSpeed, this);
                orbitCube1.GetComponent<CubeSpin>().OrbitManager = this;

                orbitCube2.transform.RotateAround(transform.position, Vector3.forward, -45);
                orbitCube2.GetComponent<CubeOrbit>().Init(transform, new Vector3(1, 1, 0).normalized, orbitSpeed, this);
                orbitCube2.GetComponent<CubeSpin>().OrbitManager = this;

                orbitCube3.transform.RotateAround(transform.position, Vector3.right, 90);
                orbitCube3.GetComponent<CubeOrbit>().Init(transform, Vector3.forward, orbitSpeed, this);
                orbitCube3.GetComponent<CubeSpin>().OrbitManager = this;

                orbitCube4.GetComponent<CubeOrbit>().Init(transform, Vector3.up, orbitSpeed, this);
                orbitCube4.GetComponent<CubeSpin>().OrbitManager = this;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // If the pickup is hit by the sword
            // Deactive the orbit manager so things stop moving
            if (collision.collider.gameObject.GetComponent<Sword>())
            {
                Active = false;
            }
        }
    }
}
