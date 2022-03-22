using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpin : MonoBehaviour
{
    [SerializeField] 
    private float rotationDegrees;
    private int axisChoice;

    // Start is called before the first frame update
    void Start()
    {
        axisChoice = Random.Range(0, 3);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotation = new Vector3(); 

        switch(axisChoice)
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

        transform.Rotate(rotation * Time.deltaTime);
    }
}
