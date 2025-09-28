using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class instructionButton : MonoBehaviour
{
    // Build number of scene to start when start button is pressed
    public int instructionScene;

    public void StartInstruction() {
        SceneManager.LoadScene(instructionScene);
    }
}
