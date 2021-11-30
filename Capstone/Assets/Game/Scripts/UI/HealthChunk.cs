using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthChunk : MonoBehaviour
{
    Rigidbody2D chunkRigidbody;
    Image image;
    public bool shattered = false;
    bool chipped = false;
    float shatteredTimer = 0f;
    float shatteredFadeStart = 1f;
    Vector3 originalPosition;
    bool faded = false;

    // Start is called before the first frame update
    void Start()
    {
        chunkRigidbody = GetComponent<Rigidbody2D>();
        image = GetComponent<Image>();
        originalPosition = transform.position;
    }


    /// <summary>
    /// Continue fading the chunk if it should be fading
    /// </summary>
    void Update()
    {
        if (shattered && !faded)
        {
            Fade();
        }
    }

    /// <summary>
    /// Detach from the health bar and fly in a random direction
    /// </summary>
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

    /// <summary>
    /// Change chunk size and tint when chip damage is taken
    /// </summary>
    public void Chip()
    {
        if (!chipped)
        {
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            image.color = new Color(0.6f, 0.6f, 0.6f, 1.0f);
            chipped = true;
        }
    }

    /// <summary>
    /// Fade the chunk's opacity as it flies away after shattering
    /// </summary>
    public void Fade()
    {
        shatteredTimer += Time.deltaTime;

        if (shatteredTimer >= shatteredFadeStart)
        {
            //Reduce opacity by an amount, then check if opacity is 0
            if (ChangeOpacity(-Time.deltaTime) <= 0)
            {
                InvisibleReset();
                faded = true;
            }
        }
    }

    /// <summary>
    /// Return the chunk to normal size and color
    /// </summary>
    public void UnChip()
    {
        if (chipped)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            image.color = new Color(1.0f, 1.0f, 1.0f, image.color.a);
        }
    }

    /// <summary>
    /// Reset a hidden / faded chunk to its original position while keeping shattered status
    /// </summary>
    public void InvisibleReset()
    {
        transform.position = originalPosition;
        chunkRigidbody.gravityScale = 0.0f;
        chunkRigidbody.velocity = Vector3.zero;
        chunkRigidbody.angularVelocity = 0.0f;
        chunkRigidbody.rotation = 0.0f;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        UnChip();
    }

    /// <summary>
    /// Reset the chunk to its original position and make it visible / unshattered again
    /// </summary>
    public void FullReset()
    {
        InvisibleReset();
        ChangeOpacity(1f);
        faded = false;
        shattered = false;
    }

    /// <summary>
    /// Add to the opacity of the chunk's UIImage by a specified amount
    /// </summary>
    /// <param name="changeAmount"></param>
    /// <returns></returns>
    public float ChangeOpacity(float changeAmount)
    {
        Color tempColor = GetComponent<Image>().color;
        tempColor.a += changeAmount;
        GetComponent<Image>().color = tempColor;

        return tempColor.a;
    }
}
