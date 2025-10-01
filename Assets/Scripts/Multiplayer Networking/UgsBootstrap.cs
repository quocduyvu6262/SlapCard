using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Authentication;
public class UgsBootstrap : MonoBehaviour
{
    async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        await InitServices();
        SceneManager.LoadScene("MainMenu");
    }
    private static async Task InitServices()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
