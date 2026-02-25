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

    [Header("Visual Feedback")]
    public GameObject deathEffectPrefab;
    [SerializeField] private float hitFlashDuration = 0.1f;
    [SerializeField] private Color hitFlashColor = Color.red;

    private Renderer enemyRenderer;
    private Color originalColor;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponentInChildren<Renderer>();

        if (enemyRenderer != null)
            originalColor = enemyRenderer.material.color;

        Debug.Log(enemyName + " spawned with " + currentHealth + " HP");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log(enemyName + " took " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);

        // Visual feedback — flash red
        StartCoroutine(HitFlash());

        // Trigger damage event for UI and audio
        GameEvents.EnemyDamaged(damage);
        GameEvents.EnemyHealthChanged(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log(enemyName + " healed " + amount + "! Health: " + currentHealth + "/" + maxHealth);
        GameEvents.EnemyHealthChanged(currentHealth);
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

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        GameEvents.EnemyDeath();
        gameObject.SetActive(false);
    }

    // Public getters used by CombatUIManager
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public float GetHealthPercent() => (float)currentHealth / maxHealth;
    public bool IsDead() => isDead;
    public string GetEnemyName() => enemyName;
}