using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        // Find all objects with the same tag as this one.
        GameObject[] musicObjects = GameObject.FindGameObjectsWithTag(this.gameObject.tag);

        // If there's more than one, it means a SoundManager already exists,
        // so we destroy this new one to avoid duplicate music.
        if (musicObjects.Length > 1)
        {
            Destroy(this.gameObject);
        }

        // This tells Unity not to destroy this object when a new scene is loaded.
        DontDestroyOnLoad(this.gameObject);
    }
}