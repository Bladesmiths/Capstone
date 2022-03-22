using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeOrbit : MonoBehaviour
{
    private GameObject parentObject;
    private float orbitSpeed;
    [SerializeField]
    private Vector3 orbitAxis = Vector3.up; 

    // Start is called before the first frame update
    void Start()
    {
        parentObject = transform.parent.parent.gameObject;
        orbitSpeed = Random.Range(30, 50);

        Vector3 displaceFromParent = (parentObject.transform.position - transform.position).normalized;
        transform.position = parentObject.transform.position + displaceFromParent * 0.8f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(parentObject.transform.position, orbitAxis, orbitSpeed * Time.deltaTime);
        Debug.DrawLine(transform.position, parentObject.transform.position, Color.red);
    }
}
