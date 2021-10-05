using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCylinder : MonoBehaviour
{
    [SerializeField] private GameObject well;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(well.transform.position,well.transform.up, 180*Time.deltaTime);
    }
}
