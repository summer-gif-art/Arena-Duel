using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    [Header("Fight Sequence")]
    public GameObject readyText;
    [SerializeField] private float readyDuration = 1.5f;
    [HideInInspector] public bool fightSequenceDone = false;

    [Header("Fight Banner Animation")]
    [SerializeField] private float punchTime = 0.3f;
    [SerializeField] private float punchPeakRatio = 0.7f;
    [SerializeField] private float punchOvershoot = 1.3f;
    [SerializeField] private int flashCount = 3;
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private float bannerHoldTime = 0.5f;
    [SerializeField] private float bannerFadeDuration = 0.5f;

    [Header("Health Bar Settings")]
    [SerializeField] private float healthBarSmoothSpeed = 5f;
    [SerializeField] private float greenThreshold = 0.75f;
    [SerializeField] private float yellowThreshold = 0.5f;
    [SerializeField] private float orangeThreshold = 0.25f;

    [Header("End Game")]
    [SerializeField] private float returnToMenuDelay = 3f;
    [SerializeField] private string mainMenuScene = "MainMenu";

    [Header("Display Names")]
    [SerializeField] private string playerDisplayName = "PLAYER";
    [SerializeField] private string levelPrefix = "LVL ";

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
        if (readyText != null) readyText.SetActive(false);

        GameEvents.OnPlayerDeath += ShowDefeatBanner;
        GameEvents.OnEnemyDeath += ShowVictoryBanner;
        GameEvents.OnEnemyHealthChanged += OnEnemyHealthChanged;
        GameEvents.OnPlayerHealthChanged += OnPlayerHealthChanged;
        GameEvents.OnPlayerXPChanged += OnXPChanged;
        GameEvents.OnPlayerLevelUp += OnLevelUp;

        UpdatePlayerUI();
        UpdateEnemyUI();

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
        if (playerHealthBarFill != null)
        {
            playerHealthBarFill.fillAmount = Mathf.Lerp(
                playerHealthBarFill.fillAmount,
                targetPlayerFill,
                Time.deltaTime * healthBarSmoothSpeed
            );
            UpdateHealthColor(playerHealthBarFill, playerHealthBarFill.fillAmount);
        }

        if (enemyHealthBarFill != null)
        {
            enemyHealthBarFill.fillAmount = Mathf.Lerp(
                enemyHealthBarFill.fillAmount,
                targetEnemyFill,
                Time.deltaTime * healthBarSmoothSpeed
            );
            UpdateHealthColor(enemyHealthBarFill, enemyHealthBarFill.fillAmount);
        }

        if (xpBarFill != null)
        {
            xpBarFill.fillAmount = Mathf.Lerp(
                xpBarFill.fillAmount,
                targetXPFill,
                Time.deltaTime * healthBarSmoothSpeed
            );
        }
    }

    // Health bar color gradient using configurable thresholds
    void UpdateHealthColor(Image bar, float percent)
    {
        if (percent > greenThreshold)
        {
            bar.color = Color.green;
        }
        else if (percent > yellowThreshold)
        {
            float t = (percent - yellowThreshold) / (greenThreshold - yellowThreshold);
            bar.color = Color.Lerp(Color.yellow, Color.green, t);
        }
        else if (percent > orangeThreshold)
        {
            float t = (percent - orangeThreshold) / (yellowThreshold - orangeThreshold);
            bar.color = Color.Lerp(new Color(1f, 0.5f, 0f), Color.yellow, t);
        }
        else
        {
            float t = percent / orangeThreshold;
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
            playerNameText.text = playerDisplayName;

        if (playerLevelText != null)
            playerLevelText.text = levelPrefix + playerHealth.currentPowerLevel;
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

    void OnPlayerHealthChanged(int newHealth) => UpdatePlayerUI();
    void OnEnemyHealthChanged(int newHealth) => UpdateEnemyUI();

    void OnXPChanged(int currentXP, int xpNeeded, int level)
    {
        targetXPFill = (float)currentXP / xpNeeded;

        if (xpText != null)
            xpText.text = currentXP + " / " + xpNeeded + " XP";

        if (playerLevelText != null)
            playerLevelText.text = levelPrefix + level;
    }

    void OnLevelUp(int newLevel)
    {
        if (playerLevelText != null)
            playerLevelText.text = levelPrefix + newLevel;
    }

    // ---- FIGHT SEQUENCE ----

    public void ShowFightBanner()
    {
        StartCoroutine(FightSequence());
    }

    IEnumerator FightSequence()
    {
        Time.timeScale = 0f;

        if (readyText != null)
        {
            readyText.SetActive(true);
            yield return new WaitForSecondsRealtime(readyDuration);
            readyText.SetActive(false);
        }

        if (fightBanner != null)
        {
            fightBanner.SetActive(true);
            Image bannerImage = fightBanner.GetComponent<Image>();
            AudioManager.Instance?.PlayFightBanner();

            if (bannerImage != null)
            {
                Color originalColor = bannerImage.color;
                RectTransform rect = fightBanner.GetComponent<RectTransform>();
                Vector3 originalScale = rect.localScale;
                rect.localScale = Vector3.zero;

                // Punch scale animation
                float t = 0f;
                while (t < punchTime)
                {
                    t += Time.unscaledDeltaTime;
                    float progress = t / punchTime;
                    float scale = progress < punchPeakRatio
                        ? Mathf.Lerp(0f, punchOvershoot, progress / punchPeakRatio)
                        : Mathf.Lerp(punchOvershoot, 1f, (progress - punchPeakRatio) / (1f - punchPeakRatio));
                    rect.localScale = originalScale * scale;
                    yield return null;
                }
                rect.localScale = originalScale;

                // Flash effect
                for (int i = 0; i < flashCount; i++)
                {
                    bannerImage.color = Color.white;
                    yield return new WaitForSecondsRealtime(flashInterval);
                    bannerImage.color = originalColor;
                    yield return new WaitForSecondsRealtime(flashInterval);
                }

                // Hold then fade out
                yield return new WaitForSecondsRealtime(bannerHoldTime);

                float timer = 0f;
                while (timer < bannerFadeDuration)
                {
                    timer += Time.unscaledDeltaTime;
                    float alpha = Mathf.Lerp(1f, 0f, timer / bannerFadeDuration);
                    bannerImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                    yield return null;
                }

                bannerImage.color = originalColor;
            }

            fightBanner.SetActive(false);
        }

        Time.timeScale = 1f;
        fightSequenceDone = true;
        Debug.Log("FIGHT!");
    }

    // ---- WIN / LOSE BANNERS ----

    public void ShowVictoryBanner()
    {
        if (enemyHealthBarFill != null)
        {
            enemyHealthBarFill.fillAmount = 0f;
            targetEnemyFill = 0f;
        }
        if (victoryBanner != null)
            victoryBanner.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.Instance?.StopSFX();
        StartCoroutine(AutoReturnToMenu());
        Debug.Log("VICTORY!");
    }

    public void ShowDefeatBanner()
    {
        if (playerHealthBarFill != null)
        {
            playerHealthBarFill.fillAmount = 0f;
            targetPlayerFill = 0f;
        }
        if (defeatBanner != null)
            defeatBanner.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.Instance?.StopSFX();
        StartCoroutine(AutoReturnToMenu());
        Debug.Log("DEFEAT!");
    }

    IEnumerator AutoReturnToMenu()
    {
        yield return new WaitForSecondsRealtime(returnToMenuDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void HideAllBanners()
    {
        if (fightBanner != null) fightBanner.SetActive(false);
        if (victoryBanner != null) victoryBanner.SetActive(false);
        if (defeatBanner != null) defeatBanner.SetActive(false);
        if (readyText != null) readyText.SetActive(false);
    }
}