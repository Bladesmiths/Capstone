using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthChunk : MonoBehaviour
{
    Rigidbody2D chunkRigidbody;
    bool shattered = false;

    // Start is called before the first frame update
    void Start()
    {
        chunkRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Shatter()
    {
        if (!shattered)
        {
            chunkRigidbody.gravityScale = 1f;
            chunkRigidbody.AddForce(new Vector2(50, 50), ForceMode2D.Impulse);
            shattered = true;
        }
    }
}
