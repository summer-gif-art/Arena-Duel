using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    
    [Header("Enemy Info")]
    public string enemyName = "Enemy";
    public Sprite enemyPortrait;
    
    [Header("UI References")]
    public Image healthBarFill;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI nameText;
    public Image portraitImage;
    
    [Header("Visual Feedback")]
    public GameObject deathEffectPrefab;
    public float hitFlashDuration = 0.1f;
    public Color hitFlashColor = Color.red;
    
    private Renderer enemyRenderer;
    private Color originalColor;
    private bool isDead = false;
    
    void Awake()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponentInChildren<Renderer>();
        
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
        
        UpdateUI();
        
        Debug.Log(enemyName + " spawned with " + currentHealth + " HP");
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        
        Debug.Log(enemyName + " took " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);
        
        // Visual feedback - flash red
        StartCoroutine(HitFlash());
        
        UpdateUI();
        
        // Trigger damage event (matches your GameEvents signature)
        GameEvents.EnemyDamaged(damage);
        GameEvents.EnemyHealthChanged(currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        Debug.Log(enemyName + " healed " + amount + "! Health: " + currentHealth + "/" + maxHealth);
        
        UpdateUI();
        GameEvents.EnemyHealthChanged(currentHealth);
    }
    
    void UpdateUI()
    {
        // Update health bar fill
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
        
        // Update HP text
        if (hpText != null)
        {
            hpText.text = currentHealth + " / " + maxHealth;
        }
        
        // Update name text
        if (nameText != null)
        {
            nameText.text = enemyName;
        }
        
        // Update portrait
        if (portraitImage != null && enemyPortrait != null)
        {
            portraitImage.sprite = enemyPortrait;
        }
    }
    
    System.Collections.IEnumerator HitFlash()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = hitFlashColor;
            yield return new WaitForSeconds(hitFlashDuration);
            enemyRenderer.material.color = originalColor;
        }
    }
    
    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        Debug.Log(enemyName + " has been defeated!");
        
        // Spawn death effect if assigned
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        
        // Trigger death event (matches your GameEvents signature)
        GameEvents.EnemyDeath();
        
        // Disable enemy
        gameObject.SetActive(false);
    }
    
    // Public getters
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public string GetEnemyName()
    {
        return enemyName;
    }
}