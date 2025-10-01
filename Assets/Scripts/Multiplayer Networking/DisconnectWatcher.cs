using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectWatcher : MonoBehaviour
{
    void Awake() { DontDestroyOnLoad(gameObject); }

    void OnEnable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    void OnDisable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    void OnClientDisconnected(ulong clientId)
    {
        // If we’re a CLIENT and *we* got disconnected (host quit), go back to menu
        if (!NetworkManager.Singleton.IsHost &&
            clientId == NetworkManager.Singleton.LocalClientId)
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
