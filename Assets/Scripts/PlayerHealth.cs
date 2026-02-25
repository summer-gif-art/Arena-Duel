using UnityEngine;
using Muryotaisu;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public int currentPowerLevel = 1;

    [Header("Death Animation")]
    [SerializeField] private float fallTime = 1f;
    [SerializeField] private Vector3 deathFallRotation = new Vector3(90f, 0f, 0f);
    [SerializeField] private float deathPauseTime = 0.5f;
    [SerializeField] private float sinkTime = 1f;
    [SerializeField] private float sinkDepth = 2f;

    private PlayerCombat playerCombat;

    void Awake()
    {
        currentHealth = maxHealth;
        playerCombat = GetComponent<PlayerCombat>();
    }

    public void TakeDamage(int damage)
    {
        // Let block/dodge modify the damage
        if (playerCombat != null)
            damage = playerCombat.ModifyDamage(damage);

        if (damage <= 0)
        {
            Debug.Log("Damage avoided!");
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log("Player took " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);

        GameEvents.PlayerHealthChanged(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Player died!");
        GameEvents.PlayerDeath();
        StartCoroutine(DeathSequence());
    }

    System.Collections.IEnumerator DeathSequence()
    {
        // Disable controls on death
        MuryotaisuController movement = GetComponent<MuryotaisuController>();
        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (movement != null) movement.enabled = false;
        if (combat != null) combat.enabled = false;

        // Fall over animation
        float timer = 0f;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = transform.rotation * Quaternion.Euler(deathFallRotation);

        while (timer < fallTime)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRot, endRot, timer / fallTime);
            yield return null;
        }

        yield return new WaitForSeconds(deathPauseTime);

        // Sink into ground
        timer = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + Vector3.down * sinkDepth;

        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        while (timer < sinkTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, timer / sinkTime);
            yield return null;
        }

        gameObject.SetActive(false);
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