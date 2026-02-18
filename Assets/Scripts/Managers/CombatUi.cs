using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CombatUIManager : MonoBehaviour
{
    [Header("Player UI")]
    public Image playerHealthBarFill;
    public Image playerPortrait;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI playerLevelText;

    [Header("Enemy UI")]
    public Image enemyHealthBarFill;
    public Image enemyPortrait;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyHPText;

    [Header("XP UI")]
    public Image xpBarFill;
    public TextMeshProUGUI xpText;

    [Header("Battle Banners")]
    public GameObject fightBanner;
    public GameObject victoryBanner;
    public GameObject defeatBanner;

    [Header("Animation Settings")]
    public float healthBarSmoothSpeed = 5f;
    public float bannerDisplayTime = 2f;

    [Header("References")]
    public PlayerHealth playerHealth;
    public EnemyHealth currentEnemy;

    private float targetPlayerFill = 1f;
    private float targetEnemyFill = 1f;
    private float targetXPFill = 0f;

    void Start()
    {
        if (fightBanner != null) fightBanner.SetActive(false);
        if (victoryBanner != null) victoryBanner.SetActive(false);
        if (defeatBanner != null) defeatBanner.SetActive(false);

        // Subscribe to events
        GameEvents.OnPlayerDeath += ShowDefeatBanner;
        GameEvents.OnEnemyDeath += ShowVictoryBanner;
        GameEvents.OnEnemyHealthChanged += OnEnemyHealthChanged;
        GameEvents.OnPlayerHealthChanged += OnPlayerHealthChanged;
        GameEvents.OnPlayerXPChanged += OnXPChanged;
        GameEvents.OnPlayerLevelUp += OnLevelUp;

        UpdatePlayerUI();
        UpdateEnemyUI();
        // Initialize health bars to full and green
        // Force health bars to start full and green
        targetPlayerFill = 1f;
        targetEnemyFill = 1f;

        if (playerHealthBarFill != null)
        {
            playerHealthBarFill.fillAmount = 1f;
            playerHealthBarFill.color = Color.green;
        }
        if (enemyHealthBarFill != null)
        {
            enemyHealthBarFill.fillAmount = 1f;
            enemyHealthBarFill.color = Color.green;
        }
    }

    void OnDestroy()
    {
        GameEvents.OnPlayerDeath -= ShowDefeatBanner;
        GameEvents.OnEnemyDeath -= ShowVictoryBanner;
        GameEvents.OnEnemyHealthChanged -= OnEnemyHealthChanged;
        GameEvents.OnPlayerHealthChanged -= OnPlayerHealthChanged;
        GameEvents.OnPlayerXPChanged -= OnXPChanged;
        GameEvents.OnPlayerLevelUp -= OnLevelUp;
    }

    void Update()
    {
        // Smooth player health bar
        if (playerHealthBarFill != null)
        {
            playerHealthBarFill.fillAmount = Mathf.Lerp(
                playerHealthBarFill.fillAmount,
                targetPlayerFill,
                Time.deltaTime * healthBarSmoothSpeed
            );
            UpdateHealthColor(playerHealthBarFill, playerHealthBarFill.fillAmount);
        }

        // Smooth enemy health bar
        if (enemyHealthBarFill != null)
        {
            enemyHealthBarFill.fillAmount = Mathf.Lerp(
                enemyHealthBarFill.fillAmount,
                targetEnemyFill,
                Time.deltaTime * healthBarSmoothSpeed
            );
            UpdateHealthColor(enemyHealthBarFill, enemyHealthBarFill.fillAmount);
        }

        // Smooth XP bar
        if (xpBarFill != null)
        {
            xpBarFill.fillAmount = Mathf.Lerp(
                xpBarFill.fillAmount,
                targetXPFill,
                Time.deltaTime * healthBarSmoothSpeed
            );
        }
    }

    // GetAmped2 style health gradient: green → yellow → orange → red
    void UpdateHealthColor(Image bar, float percent)
    {
        if (percent > 0.75f)
        {
            // Green
            bar.color = Color.green;
        }
        else if (percent > 0.5f)
        {
            // Green to Yellow
            float t = (percent - 0.5f) / 0.25f;
            bar.color = Color.Lerp(Color.yellow, Color.green, t);
        }
        else if (percent > 0.25f)
        {
            // Yellow to Orange
            float t = (percent - 0.25f) / 0.25f;
            bar.color = Color.Lerp(new Color(1f, 0.5f, 0f), Color.yellow, t);
        }
        else
        {
            // Orange to Red
            float t = percent / 0.25f;
            bar.color = Color.Lerp(Color.red, new Color(1f, 0.5f, 0f), t);
        }
    }

    public void UpdatePlayerUI()
    {
        if (playerHealth == null) return;

        targetPlayerFill = (float)playerHealth.GetCurrentHealth() / playerHealth.maxHealth;

        if (playerHPText != null)
            playerHPText.text = playerHealth.GetCurrentHealth() + " / " + playerHealth.maxHealth;

        if (playerNameText != null)
            playerNameText.text = "PLAYER";

        if (playerLevelText != null)
            playerLevelText.text = "LVL " + playerHealth.currentPowerLevel;
    }

    public void UpdateEnemyUI()
    {
        if (currentEnemy == null) return;

        targetEnemyFill = currentEnemy.GetHealthPercent();

        if (enemyHPText != null)
            enemyHPText.text = currentEnemy.GetCurrentHealth() + " / " + currentEnemy.GetMaxHealth();

        if (enemyNameText != null)
            enemyNameText.text = currentEnemy.GetEnemyName().ToUpper();

        if (enemyPortrait != null && currentEnemy.enemyPortrait != null)
            enemyPortrait.sprite = currentEnemy.enemyPortrait;
    }

    public void SetCurrentEnemy(EnemyHealth enemy)
    {
        currentEnemy = enemy;
        UpdateEnemyUI();
    }

    void OnPlayerHealthChanged(int newHealth)
    {
        UpdatePlayerUI();
    }

    void OnEnemyHealthChanged(int newHealth)
    {
        UpdateEnemyUI();
    }

    void OnXPChanged(int currentXP, int xpNeeded, int level)
    {
        targetXPFill = (float)currentXP / xpNeeded;

        if (xpText != null)
            xpText.text = currentXP + " / " + xpNeeded + " XP";

        if (playerLevelText != null)
            playerLevelText.text = "LVL " + level;
    }

    void OnLevelUp(int newLevel)
    {
        Debug.Log("UI: Level Up to " + newLevel + "!");
        if (playerLevelText != null)
            playerLevelText.text = "LVL " + newLevel;
    }

    // ========== BANNERS ==========

    public void ShowFightBanner()
    {
        StartCoroutine(DisplayBanner(fightBanner));
    }

    public void ShowVictoryBanner()
    {
        if (victoryBanner != null)
            victoryBanner.SetActive(true);
        Debug.Log("VICTORY!");
    }

    public void ShowDefeatBanner()
    {
        if (defeatBanner != null)
            defeatBanner.SetActive(true);
        Debug.Log("DEFEAT!");
    }

    IEnumerator DisplayBanner(GameObject banner)
    {
        if (banner != null)
        {
            banner.SetActive(true);
            yield return new WaitForSeconds(bannerDisplayTime);
            banner.SetActive(false);
        }
    }

    public void HideAllBanners()
    {
        if (fightBanner != null) fightBanner.SetActive(false);
        if (victoryBanner != null) victoryBanner.SetActive(false);
        if (defeatBanner != null) defeatBanner.SetActive(false);
    }
}