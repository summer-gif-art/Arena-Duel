using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUIManager : MonoBehaviour
{
    [Header("Player UI - Left Side")]
    public Image playerHealthBarFill;
    public Image playerHealthBarBackground;
    public Image playerPortrait;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI playerLevelText;
    
    [Header("Enemy UI - Right Side")]
    public Image enemyHealthBarFill;
    public Image enemyHealthBarBackground;
    public Image enemyPortrait;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyHPText;
    
    [Header("Battle Banners")]
    public GameObject fightBanner;
    public GameObject victoryBanner;
    public GameObject defeatBanner;
    
    [Header("Health Bar Colors")]
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.yellow;
    public Color criticalColor = Color.red;
    public float damagedThreshold = 0.5f;
    public float criticalThreshold = 0.25f;
    
    [Header("Animation Settings")]
    public float bannerDisplayTime = 2f;
    public float healthBarSmoothSpeed = 5f;
    
    [Header("References")]
    public PlayerHealth playerHealth;
    public EnemyHealth currentEnemy;
    
    private float targetPlayerFill = 1f;
    private float targetEnemyFill = 1f;
    
    void Start()
    {
        // Hide all banners at start
        if (fightBanner != null) fightBanner.SetActive(false);
        if (victoryBanner != null) victoryBanner.SetActive(false);
        if (defeatBanner != null) defeatBanner.SetActive(false);
        
        // Subscribe to events
        GameEvents.OnPlayerDeath += ShowDefeatBanner;
        GameEvents.OnEnemyDeath += ShowVictoryBanner;
        GameEvents.OnEnemyHealthChanged += OnEnemyHealthChanged;
        GameEvents.OnPlayerHealthChanged += OnPlayerHealthChanged;
        
        // Initial UI setup
        UpdatePlayerUI();
        UpdateEnemyUI();
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        GameEvents.OnPlayerDeath -= ShowDefeatBanner;
        GameEvents.OnEnemyDeath -= ShowVictoryBanner;
        GameEvents.OnEnemyHealthChanged -= OnEnemyHealthChanged;
        GameEvents.OnPlayerHealthChanged -= OnPlayerHealthChanged;
    }
    
    void Update()
    {
        // Smooth health bar animation
        if (playerHealthBarFill != null)
        {
            playerHealthBarFill.fillAmount = Mathf.Lerp(
                playerHealthBarFill.fillAmount, 
                targetPlayerFill, 
                Time.deltaTime * healthBarSmoothSpeed
            );
            UpdateHealthBarColor(playerHealthBarFill, playerHealthBarFill.fillAmount);
        }
        
        if (enemyHealthBarFill != null)
        {
            enemyHealthBarFill.fillAmount = Mathf.Lerp(
                enemyHealthBarFill.fillAmount, 
                targetEnemyFill, 
                Time.deltaTime * healthBarSmoothSpeed
            );
            UpdateHealthBarColor(enemyHealthBarFill, enemyHealthBarFill.fillAmount);
        }
    }
    
    void UpdateHealthBarColor(Image healthBar, float fillPercent)
    {
        if (fillPercent <= criticalThreshold)
        {
            healthBar.color = criticalColor;
        }
        else if (fillPercent <= damagedThreshold)
        {
            healthBar.color = damagedColor;
        }
        else
        {
            healthBar.color = healthyColor;
        }
    }
    
    public void UpdatePlayerUI()
    {
        if (playerHealth == null) return;
        
        // Update health bar target (will animate smoothly)
        targetPlayerFill = (float)playerHealth.GetCurrentHealth() / playerHealth.maxHealth;
        
        // Update HP text
        if (playerHPText != null)
        {
            playerHPText.text = playerHealth.GetCurrentHealth() + " / " + playerHealth.maxHealth;
        }
        
        // Update name
        if (playerNameText != null)
        {
            playerNameText.text = "PLAYER";
        }
        
        // Update level text
        if (playerLevelText != null)
        {
            playerLevelText.text = "LVL " + playerHealth.currentPowerLevel;
        }
    }
    
    public void UpdateEnemyUI()
    {
        if (currentEnemy == null) return;
        
        // Update health bar target (will animate smoothly)
        targetEnemyFill = currentEnemy.GetHealthPercent();
        
        // Update HP text
        if (enemyHPText != null)
        {
            enemyHPText.text = currentEnemy.GetCurrentHealth() + " / " + currentEnemy.GetMaxHealth();
        }
        
        // Update name
        if (enemyNameText != null)
        {
            enemyNameText.text = currentEnemy.GetEnemyName().ToUpper();
        }
        
        // Update portrait
        if (enemyPortrait != null && currentEnemy.enemyPortrait != null)
        {
            enemyPortrait.sprite = currentEnemy.enemyPortrait;
        }
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
    
    // ========== BANNER SYSTEM ==========
    
    public void ShowFightBanner()
    {
        StartCoroutine(DisplayBanner(fightBanner));
    }
    
    public void ShowVictoryBanner()
    {
        if (victoryBanner != null)
        {
            victoryBanner.SetActive(true);
        }
        Debug.Log("VICTORY!");
    }
    
    public void ShowDefeatBanner()
    {
        if (defeatBanner != null)
        {
            defeatBanner.SetActive(true);
        }
        Debug.Log("DEFEAT!");
    }
    
    System.Collections.IEnumerator DisplayBanner(GameObject banner)
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