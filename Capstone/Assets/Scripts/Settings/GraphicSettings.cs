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
    [SerializeField] private GameObject qualityDropdown;

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

        LoadGraphicPrefs();

        resolutionDropdown.GetComponent<TMP_Dropdown>().AddOptions(options);
        resolutionDropdown.GetComponent<TMP_Dropdown>().value = currentResIndex;
        resolutionDropdown.GetComponent<TMP_Dropdown>().RefreshShownValue();

        qualityDropdown.GetComponent<TMP_Dropdown>().value = QualitySettings.GetQualityLevel();
    }

    public void UpdateBrightness()
    {
        brightnessValue.GetComponent<Text>().text = "" + brightnessSlider.GetComponent<Slider>().value;
        Screen.brightness = brightnessSlider.GetComponent<Slider>().value;

        //PlayerPrefs.SetFloat("Brightness", value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetString("Fullscreen", isFullscreen + "");
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        PlayerPrefs.SetInt("Quality", qualityIndex);
    }

    public void SetResolution(int index)
    {
        Resolution activeResolution = resolutions[index];
        Screen.SetResolution(activeResolution.width, activeResolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("Resolution", index);
    }

    private void LoadGraphicPrefs()
    {
        // Load Fullscreen
        Screen.fullScreen = PlayerPrefs.GetString("Fullscreen") == "true" ? true : false;
        // Load Quality
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality"));
        // Load Resolution
        Resolution activeResolution = resolutions[PlayerPrefs.GetInt("Resolution")];
        Screen.SetResolution(activeResolution.width, activeResolution.height, Screen.fullScreen);
    }
}
