using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExitGame : MonoBehaviour
{
    public void OnExit(InputValue value)
    {
        if (value.isPressed)
            QuitGame();
    }
    
    private void QuitGame()
    {
        Debug.Log("Exit Application");
        Application.Quit();
    }
}
