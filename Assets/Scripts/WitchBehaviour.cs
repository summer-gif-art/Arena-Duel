using UnityEngine;

public class WitchAI : MonoBehaviour
{
    [Header("Melee Settings")]
    public float attackRange = 2.5f;
    public float detectionRange = 12f;
    public float attackCooldown = 1.2f;
    public int attackDamage = 18;

    [Header("Movement")]
    public float chaseSpeed = 5f;
    public float circleSpeed = 3f;
    [SerializeField] private float rotationSmoothing = 8f;

    [Header("Aggression")]
    public float lungeDistance = 3f;
    public float lungeSpeed = 10f;
    public float lungeDuration = 0.2f;

    [Header("VFX")]
    public GameObject attackVFXPrefab;
    [SerializeField] private Vector3 attackVFXOffset = Vector3.up;

    [Header("Animation")]
    [SerializeField] private float emotionResetDelay = 0.5f;

    private Transform player;
    private EnemyHealth enemyHealth;
    private Animator animator;
    private float nextAttackTime = 0f;
    private bool isLunging = false;
    private float lungeTimer = 0f;
    private Vector3 lungeDirection;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (player == null || (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 0))
        {
            animator?.SetBool("isWalk", false);
            return;
        }

        // Handle lunge
        if (isLunging)
        {
            transform.position += lungeDirection * lungeSpeed * Time.deltaTime;
            lungeTimer -= Time.deltaTime;
            animator?.SetBool("isWalk", true);
            if (lungeTimer <= 0f)
                isLunging = false;
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // Always face the player
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * rotationSmoothing);

        if (distance > detectionRange)
        {
            animator?.SetBool("isWalk", false);
            return;
        }
        else if (distance > attackRange)
        {
            // Chase the player
            Vector3 moveDir = (player.position - transform.position).normalized;
            transform.position += moveDir * chaseSpeed * Time.deltaTime;
            animator?.SetBool("isWalk", true);
        }
        else
        {
            animator?.SetBool("isWalk", false);
        }

        // Attack when close enough
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
        else if (distance <= lungeDistance && distance > attackRange && Time.time >= nextAttackTime)
        {
            StartLunge();
        }
    }

    void Attack()
    {
        if (attackVFXPrefab != null && player != null)
            Instantiate(attackVFXPrefab, player.position + attackVFXOffset, Quaternion.identity);

        animator?.SetInteger("emotions", 1);
        AudioManager.Instance?.PlayWitchAttack();

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);

        // Reset emotions back to 0 after configured delay
        Invoke(nameof(ResetEmotion), emotionResetDelay);
    }

    void ResetEmotion()
    {
        animator?.SetInteger("emotions", 0);
    }

    void StartLunge()
    {
        isLunging = true;
        lungeTimer = lungeDuration;
        lungeDirection = (player.position - transform.position).normalized;
        Debug.Log("Witch lunges!");
        Attack();
        nextAttackTime = Time.time + attackCooldown;
    }
}