using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        // Load scene chơi game
        SceneManager.LoadScene("GameScene");
    }

    public void LoadGame()
    {
        Debug.Log("Load Game");
        // Sau này bạn có thể load dữ liệu save ở đây
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
