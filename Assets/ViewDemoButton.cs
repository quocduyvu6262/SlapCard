using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewDemoButton : MonoBehaviour
{
    // Build number of scene to start when start button is pressed
    public int demoScene;

    public void ViewDemo()
    {
        SceneManager.LoadScene(demoScene);
    }
}