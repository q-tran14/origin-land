using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        // Load scene chơi game
        SceneManager.LoadScene("CharacterSelect");
    }

    public void LoadGame()
    {
        Debug.Log("Load Game");
    }

    public void OpenSettings()
    {
        Debug.Log("Open Settings");
        // Bật panel setting
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}
