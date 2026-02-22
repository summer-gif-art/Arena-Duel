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
    public float readyDuration = 1.5f;

    [Header("Animation Settings")]
    public float healthBarSmoothSpeed = 5f;

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

    void UpdateHealthColor(Image bar, float percent)
    {
        if (percent > 0.75f)
            bar.color = Color.green;
        else if (percent > 0.5f)
        {
            float t = (percent - 0.5f) / 0.25f;
            bar.color = Color.Lerp(Color.yellow, Color.green, t);
        }
        else if (percent > 0.25f)
        {
            float t = (percent - 0.25f) / 0.25f;
            bar.color = Color.Lerp(new Color(1f, 0.5f, 0f), Color.yellow, t);
        }
        else
        {
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

    void OnPlayerHealthChanged(int newHealth) => UpdatePlayerUI();
    void OnEnemyHealthChanged(int newHealth) => UpdateEnemyUI();

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
        if (playerLevelText != null)
            playerLevelText.text = "LVL " + newLevel;
    }

    // ========== FIGHT SEQUENCE ==========

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

            if (bannerImage != null)
            {
                Color originalColor = bannerImage.color;
                RectTransform rect = fightBanner.GetComponent<RectTransform>();
                Vector3 originalScale = rect.localScale;
                rect.localScale = Vector3.zero;

                float punchTime = 0.3f;
                float t = 0f;
                while (t < punchTime)
                {
                    t += Time.unscaledDeltaTime;
                    float progress = t / punchTime;
                    float scale = progress < 0.7f
                        ? Mathf.Lerp(0f, 1.3f, progress / 0.7f)
                        : Mathf.Lerp(1.3f, 1f, (progress - 0.7f) / 0.3f);
                    rect.localScale = originalScale * scale;
                    yield return null;
                }
                rect.localScale = originalScale;

                for (int i = 0; i < 3; i++)
                {
                    bannerImage.color = Color.white;
                    yield return new WaitForSecondsRealtime(0.1f);
                    bannerImage.color = originalColor;
                    yield return new WaitForSecondsRealtime(0.1f);
                }

                yield return new WaitForSecondsRealtime(0.5f);

                float timer = 0f;
                float fadeDuration = 0.5f;
                while (timer < fadeDuration)
                {
                    timer += Time.unscaledDeltaTime;
                    float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                    bannerImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                    yield return null;
                }

                bannerImage.color = originalColor;
            }

            fightBanner.SetActive(false);
        }

        Time.timeScale = 1f;
        Debug.Log("FIGHT!");
    }

    // ========== WIN/LOSE BANNERS ==========

    public void ShowVictoryBanner()
    {
        if (victoryBanner != null)
            victoryBanner.SetActive(true);
        Time.timeScale = 0f;
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopSFX();
        StartCoroutine(AutoReturnToMenu());
        Debug.Log("VICTORY!");
    }

    public void ShowDefeatBanner()
    {
        if (defeatBanner != null)
            defeatBanner.SetActive(true);
        Time.timeScale = 0f;
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopSFX();
        StartCoroutine(AutoReturnToMenu());
        Debug.Log("DEFEAT!");
    }

    IEnumerator AutoReturnToMenu()
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    

    public void HideAllBanners()
    {
        if (fightBanner != null) fightBanner.SetActive(false);
        if (victoryBanner != null) victoryBanner.SetActive(false);
        if (defeatBanner != null) defeatBanner.SetActive(false);
        if (readyText != null) readyText.SetActive(false);
    }
}