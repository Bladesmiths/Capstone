using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnGameLoad : MonoBehaviour
{
    [SerializeField] private GameObject blackFade;

    [SerializeField] private bool fadeIn;

    // Start is called before the first frame update
    void Start()
    {
        // Enable the fade in on start so they editor isn't blocked by a giant black rectangle
        if(fadeIn)
            blackFade.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // If the screen is set to fade to black
        if (fadeIn)
        {
            // Enable the black background
            blackFade.SetActive(true);
            // Slowly up the alpha
            blackFade.GetComponent<Image>().color = new Color(blackFade.GetComponent<Image>().color.r, blackFade.GetComponent<Image>().color.g, blackFade.GetComponent<Image>().color.b, blackFade.GetComponent<Image>().color.a - Time.deltaTime);
            // If the black background is fully opaque
            if (blackFade.GetComponent<Image>().color.a <= 0)
            {
                fadeIn = false;
            }
        }
    }
}
