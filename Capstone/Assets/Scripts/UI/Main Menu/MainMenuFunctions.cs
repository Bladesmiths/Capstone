using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.UI;

public class MainMenuFunctions : MonoBehaviour
{
    private GameObject lastselect;

    [SerializeField] private GameObject pauseMenu;

    //[SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject skipCutSceneButton;

    [SerializeField] private GameObject blackFade;

    private bool fadeToBlack;

    private bool loadGame = false;

    public bool Paused { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        lastselect = new GameObject();
        // If the video has ended
        //videoPlayer.loopPointReached += EndReached;
    }

    // Update is called once per frame
    void Update()
    {
        // Prevents clicking on empty space from removing controller input on menu 
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastselect);
        }
        else
        {
            lastselect = EventSystem.current.currentSelectedGameObject;
        }

        // If the screen is set to fade to black
        if (fadeToBlack)
        {
            // Enable the black background
            blackFade.SetActive(true);
            // Slowly up the alpha
            blackFade.GetComponent<Image>().color = new Color(blackFade.GetComponent<Image>().color.r,
                blackFade.GetComponent<Image>().color.g, blackFade.GetComponent<Image>().color.b,
                blackFade.GetComponent<Image>().color.a + Time.deltaTime);
            // If the black background is fully opaque
            if (blackFade.GetComponent<Image>().color.a >= 1)
            {
                // Change to intro cutscene
                SceneManager.LoadScene("IntroCutscene");
            }
        }

        if (loadGame)
        {
            // Enable the black background
            blackFade.SetActive(true);
            
            // Slowly up the alpha
            blackFade.GetComponent<Image>().color = new Color(blackFade.GetComponent<Image>().color.r,
                blackFade.GetComponent<Image>().color.g, blackFade.GetComponent<Image>().color.b,
                blackFade.GetComponent<Image>().color.a + Time.deltaTime);
            // If the black background is fully opaque
            if (blackFade.GetComponent<Image>().color.a >= 1)
            {
                LoadGame();
            }
        }
    }

    /// <summary>
    /// When the video is over, load the main game scene
    /// </summary>
    /// <param name="vp"></param>
    public void EndReached()
    {
        loadGame = true;
        
        blackFade.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    /// <summary>
    /// When the pause button is pressed
    /// </summary>
    /// <param name="value"></param>
    public void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            Pause();
        }
    }

    /// <summary>
    /// Pause the cutscene
    /// </summary>
    public void Pause()
    {
        if (pauseMenu == null) return;

        // Toggle the pause menu
        pauseMenu.SetActive(!pauseMenu.activeSelf);

        //// Pause the video if it's playing
        //if (videoPlayer.isPlaying)
        //    videoPlayer.Pause();
        //else
        //    videoPlayer.Play();

        Paused = !Paused;

        EventSystem.current.SetSelectedGameObject(skipCutSceneButton);
    }

    /// <summary>
    /// When the Play button is pressed on the Main Menu, begin the fade to black
    /// </summary>
    public void Play()
    {
        fadeToBlack = true;
    }

    /// <summary>
    /// When the Exit button is pressed, Exit the game
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }

    /// <summary>
    ///  When the Continue button is pressed, unpause the cutscene
    /// </summary>
    public void Continue()
    {
        // Toggle the pause menu
        Pause();
    }

    /// <summary>
    /// When the skip cutscene button is pressed, do the same code as reaching the end of the video
    /// </summary>
    public void SkipCutscene()
    {
        EndReached();
    }

    private void LoadGame()
    {
        SceneManager.LoadScene("PlayerScene");
        SceneManager.LoadSceneAsync("Original_EnemyScene", LoadSceneMode.Additive);
        //SceneManager.LoadSceneAsync("Village_Persistent", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Village_Audio", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Village_Gameplay", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Village_Geo", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Village_Lighting", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Village_Area1", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Village_Area2", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Village_Area3", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Village_Area4", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Village_Area5", LoadSceneMode.Additive);
    }
}