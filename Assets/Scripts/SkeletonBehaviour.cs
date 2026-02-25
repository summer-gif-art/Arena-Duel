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
    [SerializeField] private float rotationSmoothing = 5f;

    [Header("Projectile")]
    [SerializeField] private float projectileSpawnHeight = 2f;

    private Transform player;
    private float nextAttackTime = 0f;
    private EnemyHealth enemyHealth;
    private float fixedY;
    private Animator animator;
    private bool isMoving = false;

    void Start()
    {
        fixedY = transform.position.y;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (player == null || (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 0))
        {
            animator?.SetBool("isWalking", false);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // Always face the player
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * rotationSmoothing);

        isMoving = false;

        if (distance > detectionRange)
        {
            animator?.SetBool("isWalking", false);
            return;
        }
        else if (distance < tooCloseDistance)
        {
            // Retreating
            Vector3 retreatDir = (transform.position - player.position).normalized;
            transform.position += retreatDir * retreatSpeed * Time.deltaTime;
            isMoving = true;
        }
        else if (distance > preferredDistance)
        {
            // Moving closer
            Vector3 moveDir = (player.position - transform.position).normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            isMoving = true;
        }

        animator?.SetBool("isWalking", isMoving);

        // Attack when in range
        if (distance <= detectionRange && distance >= tooCloseDistance && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }

        // Keep skeleton grounded at fixed Y
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
    }

    void Attack()
    {
        AudioManager.Instance?.PlaySkeletonAttack();
        animator?.SetTrigger("Attack");

        if (projectilePrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * projectileSpawnHeight;
            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            SkeletonProjectile projScript = proj.GetComponent<SkeletonProjectile>();
            if (projScript != null)
            {
                projScript.SetDamage(attackDamage);
                Vector3 direction = (player.position - spawnPos).normalized;
                projScript.SetDirection(direction);
            }
            Debug.Log("Skeleton fires projectile!");
        }
    }
}