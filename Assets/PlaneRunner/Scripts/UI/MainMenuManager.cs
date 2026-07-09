using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        // Load the main game scene
        SceneManager.LoadScene(sceneName);
    }
    public void BtnExitGame()
    {
        // Exit the application
        Application.Quit();
    }
}