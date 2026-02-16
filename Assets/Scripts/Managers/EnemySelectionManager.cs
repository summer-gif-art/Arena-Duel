using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EnemySelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyData
    {
        public string enemyName;
        public string description;
        public Sprite portrait;
        public GameObject enemyPrefab;
    }
    
    [Header("Enemy Data - 2 Enemies")]
    public EnemyData enemy1;
    public EnemyData enemy2;
    
    [Header("UI - Enemy 1 Button")]
    public Button enemy1Button;
    public TextMeshProUGUI enemy1NameText;
    public Image enemy1Portrait;
    public TextMeshProUGUI enemy1Description;
    
    [Header("UI - Enemy 2 Button")]
    public Button enemy2Button;
    public TextMeshProUGUI enemy2NameText;
    public Image enemy2Portrait;
    public TextMeshProUGUI enemy2Description;
    
    [Header("Scene Settings")]
    public string arenaSceneName = "Map";
    
    void Start()
    {
        SetupButtons();
    }
    
    void SetupButtons()
    {
        if (enemy1Button != null)
        {
            enemy1Button.onClick.AddListener(() => SelectEnemy(0));
            if (enemy1NameText != null)
                enemy1NameText.text = enemy1.enemyName;
        }
        
        if (enemy2Button != null)
        {
            enemy2Button.onClick.AddListener(() => SelectEnemy(1));
            if (enemy2NameText != null)
                enemy2NameText.text = enemy2.enemyName;
        }
    }
    
    void SelectEnemy(int index)
    {
        PlayerPrefs.SetInt("SelectedEnemyIndex", index);
        PlayerPrefs.Save();
        
        string enemyName = (index == 0) ? enemy1.enemyName : enemy2.enemyName;
        Debug.Log("Selected: " + enemyName);
        
        SceneManager.LoadScene(arenaSceneName);
    }
}