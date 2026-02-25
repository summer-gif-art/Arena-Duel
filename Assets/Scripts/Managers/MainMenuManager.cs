using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string enemySelectionScene = "EnemySelection";
    [SerializeField] private string gameRulesScene = "GameRules";

    public void StartGame()
    {
        SceneManager.LoadScene(enemySelectionScene);
    }

    public void OpenGameRules()
    {
        SceneManager.LoadScene(gameRulesScene);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}