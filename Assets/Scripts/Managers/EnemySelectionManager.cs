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

    [Header("Enemy Data")]
    public EnemyData enemy1;
    public EnemyData enemy2;

    [Header("UI - Enemy 1")]
    public Button enemy1Button;
    public TextMeshProUGUI enemy1NameText;
    public Image enemy1Portrait;
    public TextMeshProUGUI enemy1Description;
    public Outline enemy1Outline;

    [Header("UI - Enemy 2")]
    public Button enemy2Button;
    public TextMeshProUGUI enemy2NameText;
    public Image enemy2Portrait;
    public TextMeshProUGUI enemy2Description;
    public Outline enemy2Outline;

    [Header("Fight Button")]
    public Button fightButton;

    [Header("Highlight Settings")]
    public Color selectedColor = new Color(1f, 0.8f, 0f, 1f);
    public Color deselectedColor = new Color(0f, 0f, 0f, 0f);

    [Header("Scene Settings")]
    public string arenaSceneName = "Map";

    private int selectedIndex = -1;

    void Start()
    {
        if (fightButton != null)
            fightButton.gameObject.SetActive(false);

        SetHighlight(enemy1Outline, false);
        SetHighlight(enemy2Outline, false);

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

        if (fightButton != null)
            fightButton.onClick.AddListener(StartFight);
    }

    void SelectEnemy(int index)
    {
        selectedIndex = index;
        SetHighlight(enemy1Outline, index == 0);
        SetHighlight(enemy2Outline, index == 1);

        if (fightButton != null)
            fightButton.gameObject.SetActive(true);

        Debug.Log("Selected: " + (index == 0 ? enemy1.enemyName : enemy2.enemyName));
    }

    void SetHighlight(Outline outline, bool active)
    {
        if (outline == null) return;
        outline.effectColor = active ? selectedColor : deselectedColor;
        outline.effectDistance = new Vector2(4, -4);
    }

    void StartFight()
    {
        if (selectedIndex == -1) return;
        PlayerPrefs.SetInt("SelectedEnemyIndex", selectedIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(arenaSceneName);
    }
}