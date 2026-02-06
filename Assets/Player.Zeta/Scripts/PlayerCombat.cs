using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attackDamage = 20;
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;
    
    [Header("Power System")]
    public float damageMultiplier = 1f; // Modified by power level from PlayerHealth
    
    [Header("Visual Feedback")]
    public GameObject hitEffectPrefab; // Optional: particle effect on hit
    
    private float lastAttackTime = 0f;
    
    void Update()
    {
        // Check for attack input (Space bar)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Check if enough time has passed since last attack (cooldown)
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
            else
            {
                Debug.Log("Attack on cooldown! Wait " + (attackCooldown - (Time.time - lastAttackTime)).ToString("F1") + "s");
            }
        }
    }
    
    void Attack()
    {
        // Trigger attack event
        GameEvents.PlayerAttack();
        
        Debug.Log("Player attacked!");
        
        // Calculate attack position (in front of player)
        Vector3 attackPosition = transform.position + transform.forward * attackRange;
        
        // Check for enemies in attack range
        Collider[] hitEnemies = Physics.OverlapSphere(attackPosition, attackRange, enemyLayer);
        
        if (hitEnemies.Length == 0)
        {
            Debug.Log("Attack missed! No enemies in range.");
        }
        
        // Damage all enemies hit
        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Calculate final damage with power level multiplier
                int finalDamage = Mathf.RoundToInt(attackDamage * damageMultiplier);
                
                enemyHealth.TakeDamage(finalDamage);
                
                Debug.Log("Hit " + enemy.name + " for " + finalDamage + " damage! (Base: " + attackDamage + " x " + damageMultiplier + ")");
                
                // Spawn hit effect if assigned
                if (hitEffectPrefab != null)
                {
                    Instantiate(hitEffectPrefab, enemy.transform.position, Quaternion.identity);
                }
            }
        }
    }
    
    // Visualize attack range in Scene view (for debugging)
    void OnDrawGizmosSelected()
    {
        // Draw attack range sphere
        Gizmos.color = Color.red;
        Vector3 attackPosition = transform.position + transform.forward * attackRange;
        Gizmos.DrawWireSphere(attackPosition, attackRange);
    }
}
