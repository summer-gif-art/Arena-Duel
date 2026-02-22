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

    [Header("Aggression")]
    public float lungeDistance = 3f;
    public float lungeSpeed = 10f;
    public float lungeDuration = 0.2f;

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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 8f);

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
        Debug.Log("Witch melee attack!");
        animator?.SetInteger("emotions", 1);
        AudioManager.Instance?.PlayWitchAttack();

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);

        // Reset emotions back to 0 after a short delay
        Invoke(nameof(ResetEmotion), 0.5f);
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