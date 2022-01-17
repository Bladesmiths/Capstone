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
        string resolution = resolutionDropdown.GetComponent<TMP_Dropdown>().options[index].text;
        Screen.SetResolution(int.Parse(resolution.Split('x')[0]), int.Parse(resolution.Split('x')[1]), false);
        Debug.Log(index);

    }
}
