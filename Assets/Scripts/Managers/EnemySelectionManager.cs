using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
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
    public Image enemy1Portrait;
    public TextMeshProUGUI enemy1Description;
    public Outline enemy1Outline;

    [Header("UI - Enemy 2")]
    public Button enemy2Button;
    public Image enemy2Portrait;
    public TextMeshProUGUI enemy2Description;
    public Outline enemy2Outline;

    [Header("Highlight Settings")]
    public Color hoverColor = new Color(1f, 0.8f, 0f, 1f);
    public Color normalColor = new Color(0f, 0f, 0f, 0f);
    [SerializeField] private Vector2 outlineDistance = new Vector2(5, -5);

    [Header("Scene Settings")]
    public string arenaSceneName = "Map";

    void Start()
    {
        SetHighlight(enemy1Outline, false);
        SetHighlight(enemy2Outline, false);

        // Click listeners — clicking immediately loads map
        if (enemy1Button != null)
            enemy1Button.onClick.AddListener(() => SelectEnemy(0));

        if (enemy2Button != null)
            enemy2Button.onClick.AddListener(() => SelectEnemy(1));

        // Hover listeners
        AddHoverEvents(enemy1Button, enemy1Outline);
        AddHoverEvents(enemy2Button, enemy2Outline);
    }

    void AddHoverEvents(Button btn, Outline outline)
    {
        if (btn == null) return;

        EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = btn.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => SetHighlight(outline, true));
        trigger.triggers.Add(enterEntry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => SetHighlight(outline, false));
        trigger.triggers.Add(exitEntry);
    }

    void SelectEnemy(int index)
    {
        PlayerPrefs.SetInt("SelectedEnemyIndex", index);
        PlayerPrefs.Save();
        Debug.Log("Selected: " + (index == 0 ? enemy1.enemyName : enemy2.enemyName));
        SceneManager.LoadScene(arenaSceneName);
    }

    void SetHighlight(Outline outline, bool active)
    {
        if (outline == null) return;
        outline.effectColor = active ? hoverColor : normalColor;
        outline.effectDistance = outlineDistance;
    }
}