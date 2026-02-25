using UnityEngine;

// Controls the Skeleton enemy AI - a ranged fighter that maintains distance.
// Behavior: retreats if player is too close, advances if too far,
// fires projectiles from preferred range. Always faces the player.
// Completely stops during game pause (timeScale == 0).
public class SkeletonAI : MonoBehaviour
{
    [Header("Range Settings")]
    public float preferredDistance = 8f;   // Ideal shooting distance
    public float tooCloseDistance = 4f;    // Distance at which skeleton retreats
    public float detectionRange = 15f;     // Distance at which skeleton notices player

    [Header("Attack Settings")]
    public float attackCooldown = 2f;      // Seconds between shots
    public int attackDamage = 10;          // Damage per projectile
    public GameObject projectilePrefab;    // Projectile to fire
    public Transform firePoint;            // Where projectiles spawn (optional)

    [Header("Movement")]
    public float moveSpeed = 3f;           // Speed when advancing
    public float retreatSpeed = 4f;        // Speed when retreating (faster to escape)
    public float rotationSpeed = 5f;       // How fast skeleton turns to face player

    [Header("Projectile Settings")]
    public float projectileHeightOffset = 2f; // How high above ground projectile spawns

    [Header("Detection")]
    public string playerTag = "Player";    // Tag used to find player - not hardcoded

    private Transform player;
    private float nextAttackTime = 0f;
    private EnemyHealth enemyHealth;
    private float fixedY;              // Locks Y position to keep skeleton grounded
    private Animator animator;
    private bool isMoving = false;

    void Start()
    {
        // Store starting Y to keep skeleton grounded throughout the fight
        fixedY = transform.position.y;

        // Find player by tag - tag is a public variable not hardcoded string
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("SkeletonAI: Could not find player with tag: " + playerTag);

        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Completely freeze during game pause (fight banner, victory, defeat)
        if (Time.timeScale == 0f) return;

        // Stop all behavior if dead or player not found
        if (player == null || (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 0))
        {
            animator?.SetBool("isWalking", false);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // Always rotate to face the player smoothly
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0; // Ignore height difference to prevent tilting
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(lookDir),
                Time.deltaTime * rotationSpeed
            );

        isMoving = false;

        // Out of detection range - stay idle
        if (distance > detectionRange)
        {
            animator?.SetBool("isWalking", false);
            return;
        }
        else if (distance < tooCloseDistance)
        {
            // Player too close - retreat away faster than advancing
            Vector3 retreatDir = (transform.position - player.position).normalized;
            transform.position += retreatDir * retreatSpeed * Time.deltaTime;
            isMoving = true;
        }
        else if (distance > preferredDistance)
        {
            // Too far from preferred shooting range - advance toward player
            Vector3 moveDir = (player.position - transform.position).normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            isMoving = true;
        }
        // else: at preferred distance - stand still and shoot

        animator?.SetBool("isWalking", isMoving);

        // Fire projectile when in range and cooldown has expired
        if (distance <= detectionRange && distance >= tooCloseDistance && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }

        // Lock Y position to keep skeleton grounded on the arena floor
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
    }

    // Spawns and aims a projectile toward the player's current position
    void Attack()
    {
        if (Time.timeScale == 0f) return;

        AudioManager.Instance?.PlaySkeletonAttack();
        animator?.SetTrigger("Attack");

        if (projectilePrefab == null) return;

        // Spawn above skeleton
        Vector3 spawnPos = transform.position + Vector3.up * projectileHeightOffset;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        SkeletonProjectile projScript = proj.GetComponent<SkeletonProjectile>();
        if (projScript != null)
        {
            projScript.SetDamage(attackDamage);
            Vector3 direction = (player.position - spawnPos).normalized;
            projScript.SetDirection(direction);
        }
    }
}