using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("EnemySelection"); // Or go straight to selection UI
    }
    public void OpenGameRules()
    {
        SceneManager.LoadScene("GameRules");
    }
    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit"); // For testing in editor
    }
}
