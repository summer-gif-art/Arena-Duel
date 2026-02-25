using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRulesManager : MonoBehaviour
{
    [Header("Pages")]
    public GameObject page1;
    public GameObject page2;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    void Start()
    {
        if (page1 != null) page1.SetActive(true);
        if (page2 != null) page2.SetActive(false);
    }

    public void NextPage()
    {
        if (page1 != null) page1.SetActive(false);
        if (page2 != null) page2.SetActive(true);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}