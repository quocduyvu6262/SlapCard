using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class backToMainMenu : MonoBehaviour
{
    // Build number of scene to start when start button is pressed
    public int mainMenuScene;

    public void StartMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
