using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthChunk : MonoBehaviour
{
    Rigidbody2D chunkRigidbody;
    Image image;
    public bool shattered = false;
    public bool chipped = false;
    [SerializeField] float shatteredTimer = 0f;
    float shatteredFadeStart = 1f;
    Vector3 originalPosition;
    float originalScale;
    [SerializeField] bool faded = false;
    [SerializeField] bool growing = false;
    float growthSpeed = 0.5f;
    float fadeSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        chunkRigidbody = GetComponent<Rigidbody2D>();
        image = GetComponent<Image>();
        originalPosition = transform.position;
        originalScale = transform.localScale.x;
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

        if (growing)
        {
            Grow();
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
            image.color = new Color(0.6f, 0.6f, 0.6f, image.color.a); //Maintain current visibility
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
            if (ChangeOpacity(-Time.deltaTime * fadeSpeed) <= 0)
            {
                InvisibleReset();
            }
        }
    }

    /// <summary>
    /// Start the process of returning the chunk to normal size and color
    /// </summary>
    public void UnChip()
    {
        if (chipped)
        {
            growing = true;
            image.color = new Color(1.0f, 1.0f, 1.0f, image.color.a); //Maintain current visibility
            chipped = false;
        }
    }

    /// <summary>
    /// Gradually return the chunk to its original size
    /// </summary>
    public void Grow()
    {
        if (ChangeSize(Time.deltaTime * growthSpeed) >= originalScale)
        {
            growing = false;
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
        transform.localScale = new Vector3(originalScale, originalScale, originalScale);
        SetOpacity(0f);
    }

    /// <summary>
    /// Reset the chunk to its original position and make it visible / unshattered again
    /// </summary>
    public void FullReset()
    {
        InvisibleReset();
        SetOpacity(1f);
        faded = false;
        shattered = false;
        growing = false;
    }

    /// <summary>
    /// Add to the opacity of the chunk's UIImage by a specified amount
    /// </summary>
    /// <param name="changeAmount"></param>
    /// <returns></returns>
    private float ChangeOpacity(float opacityChange)
    {
        Color tempColor = GetComponent<Image>().color;
        tempColor.a += opacityChange;
        GetComponent<Image>().color = tempColor;

        return tempColor.a;
    }

    /// <summary>
    /// Set the opacity of the chunk's UIImage to a specified value
    /// </summary>
    private void SetOpacity(float opacityValue)
    {
        Color tempColor = GetComponent<Image>().color;
        tempColor.a = opacityValue;
        GetComponent<Image>().color = tempColor;

    }

    /// <summary>
    /// Add to all axes of chunk's localScale by a specified amount
    /// </summary>
    /// <param name="sizeChange"></param>
    /// <returns></returns>
    private float ChangeSize(float sizeChange)
    {
        transform.localScale += new Vector3(sizeChange, sizeChange, sizeChange);
        if (transform.localScale.x > originalScale)
        {
            transform.localScale = new Vector3(originalScale, originalScale, originalScale);
        }

        return transform.localScale.x;
    }
}
