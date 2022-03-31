using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthChunk : MonoBehaviour
{
    Rigidbody2D chunkRigidbody;
    Image image;
    Vector3 originalPosition;
    Quaternion originalRotation;
    float originalRigidbodyRotation;
    float originalScale;

    public bool shattered = false;
    public bool chipped = false;
    [SerializeField] float shatteredTimer = 0f;
    [SerializeField] bool faded = false;
    [SerializeField] bool growing = false;
    float shatteredFadeStart = 1f;  
    float growthSpeedChip = 0.6f;
    float growthSpeedFull = 3.0f;
    float fadeSpeed = 1.0f;

    /// <summary>
    /// Get references to components and save the chunk's original status so it can be reset later
    /// </summary>
    void Start()
    {
        chunkRigidbody = GetComponent<Rigidbody2D>();
        image = GetComponent<Image>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale.x;
        originalRigidbodyRotation = chunkRigidbody.rotation;
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;

        //Copy spriteRenderer sprite to Image component so the health bar shows in the UI
        //Used for one-time conversion from PSD importer default settings that create a spriteRenderer
        //image.sprite = GetComponent<SpriteRenderer>().sprite;
    }


    /// <summary>
    /// Continue fading / growing the chunk over time
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
                faded = true;
                shatteredTimer = 0;
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
            image.color = new Color(1.0f, 1.0f, 1.0f, image.color.a); //Maintain current visibility
            growing = true;
        }
    }

    /// <summary>
    /// Start the process of healing a chunk, transitioning it from invisible to fully visible
    /// </summary>
    public void Restore()
    {
        if (!chipped || shattered)
        {           
            InvisibleReset();
            faded = false;
            shattered = false;
            SetOpacity(1f);
            transform.localScale = new Vector3();

            growing = true;           
        }       
    }

    /// <summary>
    /// Gradually return the chunk to its original size
    /// </summary>
    public void Grow()
    {
        float growthSpeed;

        //Chunks grow more quickly if fully regrowing
        //This results in chip / full regrowth taking roughly equal amounts of time
        if (chipped)
        {
            growthSpeed = growthSpeedChip;
        }
        else
        {
            growthSpeed = growthSpeedFull;
        }

        //Once a chunk returns to its original size, stop growing
        if (ChangeSize(Time.deltaTime * growthSpeed) >= originalScale)
        {
            growing = false;
            chipped = false;
        }
    }

    /// <summary>
    /// Reset a hidden / faded chunk to its original position while keeping it invisible
    /// </summary>
    public void InvisibleReset()
    {
        //Reset rigidbody
        chunkRigidbody.velocity = Vector2.zero;
        chunkRigidbody.angularVelocity = 0.0f;
        chunkRigidbody.rotation = originalRigidbodyRotation;
        chunkRigidbody.gravityScale = 0.0f;

        //Reset chunk location, size, color, etc.
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = new Vector3(originalScale, originalScale, originalScale);
        image.color = new Color(1.0f, 1.0f, 1.0f, image.color.a); //Maintain current visibility
        chipped = false;
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
        growing = false;
        shattered = false;
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
