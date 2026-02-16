using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    [Header("Range Settings")]
    public float preferredDistance = 8f;
    public float tooCloseDistance = 4f;
    public float detectionRange = 15f;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public int attackDamage = 10;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float retreatSpeed = 4f;

    private Transform player;
    private float nextAttackTime = 0f;
    private EnemyHealth enemyHealth;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (player == null || (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 0))
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Always face the player
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);

        // BEHAVIOR: Keep distance and shoot
        if (distance > detectionRange)
        {
            // Player too far, idle
            return;
        }
        else if (distance < tooCloseDistance)
        {
            // Too close! Retreat
            Vector3 retreatDir = (transform.position - player.position).normalized;
            transform.position += retreatDir * retreatSpeed * Time.deltaTime;
        }
        else if (distance > preferredDistance)
        {
            // Move closer to preferred range
            Vector3 moveDir = (player.position - transform.position).normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        // Attack when in range
        if (distance <= detectionRange && distance >= tooCloseDistance && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            SkeletonProjectile projScript = proj.GetComponent<SkeletonProjectile>();
            if (projScript != null)
            {
                projScript.SetDamage(attackDamage);
                projScript.SetDirection((player.position - firePoint.position).normalized);
            }
            Debug.Log("Skeleton fires projectile!");
        }
    }
}
