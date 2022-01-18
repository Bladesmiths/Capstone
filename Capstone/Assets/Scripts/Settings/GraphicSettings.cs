using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicSettings : MonoBehaviour
{
    [SerializeField] private GameObject brightnessSlider;
    [SerializeField] private GameObject brightnessValue;

    [SerializeField] private GameObject resolutionDropdown;

    Resolution[] resolutions;

    public void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.GetComponent<TMP_Dropdown>().ClearOptions();

        List<string> options = new List<string>();
        int currentResIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.GetComponent<TMP_Dropdown>().AddOptions(options);
        resolutionDropdown.GetComponent<TMP_Dropdown>().value = currentResIndex;
        resolutionDropdown.GetComponent<TMP_Dropdown>().RefreshShownValue();
    }

    public void UpdateBrightness()
    {
        brightnessValue.GetComponent<Text>().text = "" + brightnessSlider.GetComponent<Slider>().value;
        Screen.brightness = brightnessSlider.GetComponent<Slider>().value;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetResolution(int index)
    {
        Resolution activeResolution = resolutions[index];
        Screen.SetResolution(activeResolution.width, activeResolution.height, Screen.fullScreen);
        //string resolution = resolutionDropdown.GetComponent<TMP_Dropdown>().options[index].text;
        //Screen.SetResolution(int.Parse(resolution.Split('x')[0]), int.Parse(resolution.Split('x')[1]), false);
        //Debug.Log(index);

    }
}
