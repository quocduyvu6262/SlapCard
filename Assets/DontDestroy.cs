using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    // This creates a static variable to hold the single instance of our SoundManager.
    public static DontDestroy instance;

    void Awake()
    {
        // Check if an instance already exists.
        if (instance == null)
        {
            // If not, set this object as the instance...
            instance = this;
            // ...and tell Unity not to destroy it when loading new scenes.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this new one immediately.
            Destroy(gameObject);
        }
    }
}