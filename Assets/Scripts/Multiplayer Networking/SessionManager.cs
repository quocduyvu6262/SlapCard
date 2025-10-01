using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Multiplayer;
using System.Threading.Tasks;


public class SessionManager : MonoBehaviour
{
    public static SessionManager I;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this; DontDestroyOnLoad(gameObject);
    }

    // HOST: create session (will add real code next), then start host
    public async Task<string> CreateAndHostAsync(string sessionName, int maxPlayers)
    {
        try
        {
            var options = new SessionOptions { MaxPlayers = maxPlayers }.WithRelayNetwork();
            var session = await MultiplayerService.Instance.CreateSessionAsync(options);

            Debug.Log($"Session created. Join code: {session.Code}");

            // Load your gameplay scene
            SceneManager.LoadScene("SampleScene");

            // Return the code so UI can show it
            return session.Code;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Create host failed: " + ex.Message);
            return null;
        }
    }

    // CLIENT: join by code (will add real code next), then start client
    public async System.Threading.Tasks.Task JoinAsClientAsync(string joinCode)
    {
        // TODO: Join session + Relay via Multiplayer Services using the joinCode
        // TODO: Configure UnityTransport with Relay data
        var session = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);

        Debug.Log($"Joined session {session.Id}");

        // When the connection is ready, move to the game
        SceneManager.LoadScene("SampleScene");
    }

    // Host leaves or client wants to exit back to menu
    public void ShutdownToMenu()
    {
        if (NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.Shutdown();

        SceneManager.LoadScene("MainMenu");
    }
}
