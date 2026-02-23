using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRulesManager : MonoBehaviour
{
    public GameObject page1;
    public GameObject page2;

    void Start()
    {
        page1.SetActive(true);
        page2.SetActive(false);
    }

    public void NextPage()
    {
        page1.SetActive(false);
        page2.SetActive(true);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}