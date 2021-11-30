using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthChunk : MonoBehaviour
{
    Rigidbody2D chunkRigidbody;
    bool shattered = false;
    float shatteredTimer = 0f;
    float shatteredFadeStart = 1f;
    Vector3 originalPosition;
    bool faded = false;

    // Start is called before the first frame update
    void Start()
    {
        chunkRigidbody = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (shattered && !faded)
        {
            Fade();
        }
    }

    //Detach from the health bar and fly in a random direction
    public void Shatter()
    {
        if (!shattered)
        {
            chunkRigidbody.gravityScale = 1f;
            chunkRigidbody.drag = 0.5f;
            chunkRigidbody.angularDrag = 0.5f;
            Vector2 direction = Random.insideUnitCircle;
            float force = Random.Range(300, 500);

            //Apply force
            chunkRigidbody.AddForce(direction * force, ForceMode2D.Impulse);

            //Apply torque
            chunkRigidbody.AddTorque(Random.Range(-300, 300));

            shattered = true;
        }
    }

    //Fade the chunk's opacity as it flies away after shattering
    public void Fade()
    {
        shatteredTimer += Time.deltaTime;

        if (shatteredTimer >= shatteredFadeStart)
        {
            //Reduce opacity by an amount, then check if opacity is 0
            if (ChangeOpacity(-Time.deltaTime) <= 0)
            {
                InvisibleReset();
            }
        }
    }

    //Reset the hidden chunk to its original position while keeping shattered status
    public void InvisibleReset()
    {
        faded = true;
        transform.position = originalPosition; 
    }

    //Reset the chunk to its original position and make it visible / unshattered again
    public void FullReset()
    {
        transform.position = originalPosition;
        ChangeOpacity(1f);
        faded = false;
        shattered = false;
        chunkRigidbody.gravityScale = 0.0f;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    //Add to the opacity of the chunk's UIImage by a specified amount
    public float ChangeOpacity(float changeAmount)
    {
        Color tempColor = GetComponent<Image>().color;
        tempColor.a += changeAmount;
        GetComponent<Image>().color = tempColor;

        return tempColor.a;
    }
}
