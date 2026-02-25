using UnityEngine;

// Witch enemy AI — aggressive melee chaser that lunges at the player.
// Uses detection range to start chasing, attack range for melee hits,
// and a lunge system to close the gap when just outside attack range.
// All values are configurable from the Inspector for easy tuning.
public class WitchAI : MonoBehaviour
{
    [Header("Melee Settings")]
    public float attackRange = 2.5f;       // Distance to trigger melee attack
    public float detectionRange = 12f;     // Distance to start chasing the player
    public float attackCooldown = 1.2f;    // Seconds between attacks
    public int attackDamage = 18;          // Damage per hit

    [Header("Movement")]
    public float chaseSpeed = 5f;                              // Speed when chasing the player
    public float circleSpeed = 3f;                             // Speed when circling (reserved for future use)
    [SerializeField] private float rotationSmoothing = 8f;     // How smoothly the witch turns to face the player

    [Header("Aggression")]
    public float lungeDistance = 3f;    // Distance threshold to trigger a lunge attack
    public float lungeSpeed = 10f;      // Speed of the lunge dash
    public float lungeDuration = 0.2f;  // How long the lunge lasts in seconds

    [Header("VFX")]
    public GameObject attackVFXPrefab;                                 // Particle effect spawned on hit
    [SerializeField] private Vector3 attackVFXOffset = Vector3.up;     // Offset from player position for VFX spawn

    [Header("Animation")]
    [SerializeField] private float emotionResetDelay = 0.5f;  // Seconds before attack animation resets to idle

    // Private references — cached in Start for performance
    private Transform player;
    private EnemyHealth enemyHealth;
    private Animator animator;

    // Attack state tracking
    private float nextAttackTime = 0f;    // When the next attack is allowed
    private bool isLunging = false;       // Whether the witch is mid-lunge
    private float lungeTimer = 0f;        // Time remaining in current lunge
    private Vector3 lungeDirection;       // Direction of the current lunge
    private float lastVFXTime = 0f;       // Prevents double VFX when lunge + attack fire together

    // Find the player and cache components on spawn
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Stop all behavior if player is missing or witch is dead
        if (player == null || (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 0))
        {
            animator?.SetBool("isWalk", false);
            return;
        }

        // Handle lunge movement — witch dashes forward for lungeDuration seconds
        if (isLunging)
        {
            transform.position += lungeDirection * lungeSpeed * Time.deltaTime;
            lungeTimer -= Time.deltaTime;
            animator?.SetBool("isWalk", true);

            // End lunge when timer runs out
            if (lungeTimer <= 0f)
                isLunging = false;
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // Always rotate to face the player smoothly
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0; // Keep rotation on horizontal plane only
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * rotationSmoothing);

        // Zone 1: Outside detection range — stand idle
        if (distance > detectionRange)
        {
            animator?.SetBool("isWalk", false);
            return;
        }
        // Zone 2: Inside detection but outside attack range — chase the player
        else if (distance > attackRange)
        {
            Vector3 moveDir = (player.position - transform.position).normalized;
            transform.position += moveDir * chaseSpeed * Time.deltaTime;
            animator?.SetBool("isWalk", true);
        }
        // Zone 3: Within attack range — stop moving
        else
        {
            animator?.SetBool("isWalk", false);
        }

        // Attack when close enough and cooldown has passed
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
        // Lunge when just outside attack range but within lunge distance
        else if (distance <= lungeDistance && distance > attackRange && Time.time >= nextAttackTime)
        {
            StartLunge();
        }
    }

    // Performs a melee attack: spawns VFX, plays animation and sound, deals damage
    void Attack()
    {
        // Spawn VFX only if cooldown has passed — prevents double VFX when lunge calls Attack()
        if (attackVFXPrefab != null && player != null && Time.time - lastVFXTime >= attackCooldown)
        {
            Instantiate(attackVFXPrefab, player.position + attackVFXOffset, Quaternion.identity);
            lastVFXTime = Time.time;
        }

        // Trigger attack animation via emotions parameter
        animator?.SetInteger("emotions", 1);

        // Play witch attack sound through the AudioManager singleton
        AudioManager.Instance?.PlayWitchAttack();

        // Deal damage to the player through PlayerHealth component
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);

        // Reset animation back to idle after configured delay
        Invoke(nameof(ResetEmotion), emotionResetDelay);
    }

    // Resets the emotions parameter so the animator returns to idle state
    void ResetEmotion()
    {
        animator?.SetInteger("emotions", 0);
    }

    // Initiates a lunge: dashes toward the player and attacks simultaneously
    void StartLunge()
    {
        isLunging = true;
        lungeTimer = lungeDuration;
        lungeDirection = (player.position - transform.position).normalized;
        Debug.Log("Witch lunges!");

        // Attack fires during lunge so damage happens at the start of the dash
        Attack();
        nextAttackTime = Time.time + attackCooldown;
    }
}