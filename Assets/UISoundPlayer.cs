using UnityEngine;
using UnityEngine.SceneManagement; // <-- ADD THIS LINE
using System.Collections;         // <-- ADD THIS LINE

public class UISoundPlayer : MonoBehaviour
{
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        if (buttonClickSound != null)
        {
            var go = new GameObject("TmpClickSFX");
            Object.DontDestroyOnLoad(go);
            var src = go.AddComponent<AudioSource>();
            src.spatialBlend = 0f;
            src.PlayOneShot(buttonClickSound);
            Object.Destroy(go, buttonClickSound.length);
        }
    }

    public void PlayHoverSound()
    {
        if (buttonHoverSound != null)
        {
            audioSource.PlayOneShot(buttonHoverSound);
        }
    }

    // --- ADD THIS NEW FUNCTION AND COROUTINE ---
    public void PlayClickAndLoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAfterSound(sceneName));
    }

    private IEnumerator LoadSceneAfterSound(string sceneName)
    {
        // 1. Play the click sound
        if (buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        // 2. Wait for the sound to finish playing
        yield return new WaitForSeconds(buttonClickSound.length);

        // 3. Load the new scene
        SceneManager.LoadScene(sceneName);
    }
}