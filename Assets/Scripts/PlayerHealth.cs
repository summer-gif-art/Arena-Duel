using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public int currentPowerLevel = 1;

    private PlayerCombat playerCombat;

    void Start()
    {
        currentHealth = maxHealth;
        playerCombat = GetComponent<PlayerCombat>();
    }

    public void TakeDamage(int damage)
    {
        // Let block/dodge modify the damage
        if (playerCombat != null)
        {
            damage = playerCombat.ModifyDamage(damage);
        }

        if (damage <= 0)
        {
            Debug.Log("Damage avoided!");
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log("Player took " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);

        // Update UI
        GameEvents.PlayerHealthChanged(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        GameEvents.PlayerDeath();
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}