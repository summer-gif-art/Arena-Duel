using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    
    [Header("Level System")]
    public int currentPowerLevel = 1;
    
    [Header("Visual Feedback")]
    public GameObject deathEffectPrefab;
    public float hitFlashDuration = 0.1f;
    public Color hitFlashColor = Color.red;
    
    [Header("Respawn")]
    public Transform respawnPoint;
    public bool autoRespawn = false;
    public float respawnDelay = 3f;
    
    private Renderer playerRenderer;
    private Color originalColor;
    private bool isDead = false;
    private CharacterController controller;
    
    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<CharacterController>();
        playerRenderer = GetComponentInChildren<Renderer>();
        
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
        
        Debug.Log("Player spawned with " + currentHealth + " HP");
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        
        Debug.Log("Player took " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);
        
        // Visual feedback - flash red
        StartCoroutine(HitFlash());
        
        // Trigger damage event
        GameEvents.PlayerDamaged(damage);
        GameEvents.PlayerHealthChanged(currentHealth);
        
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
        
        Debug.Log("Player healed " + amount + "! Health: " + currentHealth + "/" + maxHealth);
        
        GameEvents.PlayerHealthChanged(currentHealth);
    }
    
    System.Collections.IEnumerator HitFlash()
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color = hitFlashColor;
            yield return new WaitForSeconds(hitFlashDuration);
            playerRenderer.material.color = originalColor;
        }
    }
    
    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        Debug.Log("Player has died!");
        
        // Spawn death effect if assigned
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        
        // Trigger death event
        GameEvents.PlayerDeath();
        
        // Handle respawn or game over
        if (autoRespawn && respawnPoint != null)
        {
            StartCoroutine(RespawnAfterDelay());
        }
        else
        {
            // Disable player
            gameObject.SetActive(false);
        }
    }
    
    System.Collections.IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }
    
    public void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        
        if (respawnPoint != null)
        {
            if (controller != null)
            {
                controller.enabled = false;
                transform.position = respawnPoint.position;
                transform.rotation = respawnPoint.rotation;
                controller.enabled = true;
            }
            else
            {
                transform.position = respawnPoint.position;
                transform.rotation = respawnPoint.rotation;
            }
        }
        
        gameObject.SetActive(true);
        GameEvents.PlayerHealthChanged(currentHealth);
        
        Debug.Log("Player respawned with full health!");
    }
    
    // ========== PUBLIC GETTERS (FIXES THE ERROR!) ==========
    
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
    
    // Level up system (for Day 5 punch upgrades)
    public void LevelUp()
    {
        currentPowerLevel++;
        Debug.Log("Player leveled up to Level " + currentPowerLevel + "!");
    }
    
    public void SetPowerLevel(int level)
    {
        currentPowerLevel = level;
    }
}