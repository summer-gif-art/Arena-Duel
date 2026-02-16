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
    private float nextAttackTime = 0f;
    private bool isLunging = false;
    private float lungeTimer = 0f;
    private Vector3 lungeDirection;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (player == null || (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 0))
            return;

        // Handle lunge
        if (isLunging)
        {
            transform.position += lungeDirection * lungeSpeed * Time.deltaTime;
            lungeTimer -= Time.deltaTime;
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

        // BEHAVIOR: Aggressive melee
        if (distance > detectionRange)
        {
            return;
        }
        else if (distance > attackRange)
        {
            // Chase the player aggressively
            Vector3 moveDir = (player.position - transform.position).normalized;
            transform.position += moveDir * chaseSpeed * Time.deltaTime;
        }

        // Attack when close enough
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
        // Lunge if just outside attack range
        else if (distance <= lungeDistance && distance > attackRange && Time.time >= nextAttackTime)
        {
            StartLunge();
        }
    }

    void Attack()
    {
        Debug.Log("Witch melee attack!");
        
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    void StartLunge()
    {
        isLunging = true;
        lungeTimer = lungeDuration;
        lungeDirection = (player.position - transform.position).normalized;
        Debug.Log("Witch lunges!");
        
        // Attack at end of lunge
        Attack();
        nextAttackTime = Time.time + attackCooldown;
    }
}
