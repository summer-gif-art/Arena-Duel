using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    [Header("XP Settings")]
    public int currentXP = 0;
    public int currentLevel = 1;
    public int xpToNextLevel = 100;
    public float xpMultiplier = 1.5f; // each level needs more XP

    [Header("XP Rewards")]
    public int xpPerHit = 15;
    public int xpPerBlock = 10;
    public int xpPerDodge = 20;
    public int xpPerKill = 50;
    public int xpPerSurvivalTick = 5;
    public float survivalTickInterval = 3f; // earn XP every 3 seconds alive

    [Header("Level Up Bonuses")]
    public int bonusDamagePerLevel = 5;
    public int bonusHealthPerLevel = 10;

    private float survivalTimer = 0f;
    private bool isAlive = true;
    private PlayerCombat playerCombat;
    private PlayerHealth playerHealth;

    void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
        playerHealth = GetComponent<PlayerHealth>();
        GameEvents.OnEnemyDeath += OnEnemyKilled;
        GameEvents.OnPlayerDeath += OnPlayerDied;
    }

    void OnDestroy()
    {
        GameEvents.OnEnemyDeath -= OnEnemyKilled;
        GameEvents.OnPlayerDeath -= OnPlayerDied;
    }

    void Update()
    {
        if (!isAlive) return;

        // Survival XP
        survivalTimer += Time.deltaTime;
        if (survivalTimer >= survivalTickInterval)
        {
            AddXP(xpPerSurvivalTick, "Survival");
            survivalTimer = 0f;
        }
    }

    public void AddXP(int amount, string source)
    {
        if (!isAlive) return;

        currentXP += amount;
        Debug.Log("+" + amount + " XP from " + source + "! Total: " + currentXP + "/" + xpToNextLevel);

        // Check for level up
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        // Notify UI
        GameEvents.PlayerXPChanged(currentXP, xpToNextLevel, currentLevel);
    }

    void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);

        // Apply bonuses
        if (playerCombat != null)
        {
            playerCombat.attackDamage += bonusDamagePerLevel;
        }
        if (playerHealth != null)
        {
            playerHealth.maxHealth += bonusHealthPerLevel;
        }

        Debug.Log("LEVEL UP! Now level " + currentLevel + "! Next level at " + xpToNextLevel + " XP");
        GameEvents.PlayerLevelUp(currentLevel);
    }

    // Called from PlayerCombat when hitting an enemy
    public void OnHitEnemy()
    {
        AddXP(xpPerHit, "Hit");
    }

    // Called from PlayerCombat when blocking
    public void OnSuccessfulBlock()
    {
        AddXP(xpPerBlock, "Block");
    }

    // Called from PlayerCombat when dodging
    public void OnSuccessfulDodge()
    {
        AddXP(xpPerDodge, "Dodge");
    }

    void OnEnemyKilled()
    {
        AddXP(xpPerKill, "Kill");
    }

    void OnPlayerDied()
    {
        isAlive = false;
    }

    public float GetXPPercent()
    {
        return (float)currentXP / xpToNextLevel;
    }
}
