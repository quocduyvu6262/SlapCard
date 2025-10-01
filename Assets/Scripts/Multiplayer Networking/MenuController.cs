using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    public TMP_InputField joinCodeField;
    public TMP_Text joinCodeOut;
    public async void OnCreateClicked()
    {
        string code = await SessionManager.I.CreateAndHostAsync("Purrfect Lobby", 4);
        if (!string.IsNullOrEmpty(code) && joinCodeOut)
            joinCodeOut.text = $"Join Code: {code}";
    }

    public async void OnJoinClicked()
    {
        var code = joinCodeField.text.Trim();
        if (!string.IsNullOrEmpty(code))
            await SessionManager.I.JoinAsClientAsync(code);
    }
}