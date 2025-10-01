using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeSelector : MonoBehaviour
{
    public int gameStartScene;
    public void SelectNormalMode()
    {
        GameSettings.selectedDifficulty = GameSettings.Difficulty.Normal;
        SceneManager.LoadScene(gameStartScene);
    }

    public void SelectHardMode()
    {
        GameSettings.selectedDifficulty = GameSettings.Difficulty.Hard;
        SceneManager.LoadScene(gameStartScene);
    }
}