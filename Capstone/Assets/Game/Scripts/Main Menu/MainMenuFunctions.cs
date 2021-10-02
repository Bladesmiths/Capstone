using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuFunctions : MonoBehaviour
{
    private GameObject lastselect;

    // Start is called before the first frame update
    void Start()
    {
        lastselect = new GameObject();
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
    }

    public void Play()
    {
        SceneManager.LoadScene("PlayTestScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
