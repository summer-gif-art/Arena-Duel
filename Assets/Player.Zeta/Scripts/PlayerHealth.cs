using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    
    [Header("Power Level System")]
    public int currentPowerLevel = 1;
    public int maxPowerLevel = 3;
    public int healthThresholdForLevelUp = 50; // HP needed to level up
    
    [Header("Power Level Bonuses")]
    public float[] damageMultipliers = { 1f, 1.5f, 2f }; // Level 1, 2, 3 damage
    public float[] sizeMultipliers = { 1f, 1.2f, 1.5f };   // Level 1, 2, 3 size
    public Color[] powerColors = { Color.white, Color.yellow, Color.red }; // Level colors
    
    [Header("UI References")]
    public Image healthBarFill;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI powerLevelText;
    public Image playerPortrait;
    
    private Renderer playerRenderer;
    private Vector3 originalScale;
    private PlayerCombat playerCombat;
    
    void Start()
    {
        currentHealth = maxHealth;
        playerRenderer = GetComponentInChildren<Renderer>();
        originalScale = transform.localScale;
        playerCombat = GetComponent<PlayerCombat>();
        
        UpdateUI();
        UpdatePowerLevel();
        
        Debug.Log("Player Health: " + currentHealth);
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        
        Debug.Log("Player took " + damage + " damage! Health: " + currentHealth);
        
        UpdateUI();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int amount)
    {
        currentHealth += amount;
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        Debug.Log("Player healed " + amount + "! Health: " + currentHealth);
        
        UpdateUI();
        CheckPowerLevelUp();
    }
    
    void CheckPowerLevelUp()
    {
        // Check if player has enough HP to level up
        int newLevel = 1;
        
        if (currentHealth >= healthThresholdForLevelUp * 2)
        {
            newLevel = 3; // Max level
        }
        else if (currentHealth >= healthThresholdForLevelUp)
        {
            newLevel = 2;
        }
        
        if (newLevel > currentPowerLevel && newLevel <= maxPowerLevel)
        {
            currentPowerLevel = newLevel;
            UpdatePowerLevel();
            Debug.Log("POWER UP! Level " + currentPowerLevel);
        }
    }
    
    void UpdatePowerLevel()
    {
        int levelIndex = currentPowerLevel - 1;
        
        // Update size
        if (originalScale != Vector3.zero)
        {
            transform.localScale = originalScale * sizeMultipliers[levelIndex];
        }
        
        // Update color
        if (playerRenderer != null)
        {
            playerRenderer.material.color = powerColors[levelIndex];
        }
        
        // Update damage multiplier in combat script
        if (playerCombat != null)
        {
            playerCombat.damageMultiplier = damageMultipliers[levelIndex];
        }
        
        // Update UI
        if (powerLevelText != null)
        {
            powerLevelText.text = "LVL " + currentPowerLevel;
            powerLevelText.color = powerColors[levelIndex];
        }
        
        if (playerPortrait != null)
        {
            playerPortrait.color = powerColors[levelIndex];
        }
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
            hpText.text = "HP: " + currentHealth + " / " + maxHealth;
        }
    }
    
    void Die()
    {
        Debug.Log("Player died!");
        
        // Disable controls
        GetComponent<PlayerMovement>().enabled = false;
        
        if (playerCombat != null)
        {
            playerCombat.enabled = false;
        }
        
        // Trigger death event
        GameEvents.PlayerDeath();
    }
    
    // Public method to get current damage multiplier
    public float GetDamageMultiplier()
    {
        return damageMultipliers[currentPowerLevel - 1];
    }
}