using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public int attackDamage = 20;
    public float attackCooldown = 0.5f;

    [Header("Block Settings")]
    public float blockDamageReduction = 0.75f;

    [Header("Dodge Settings")]
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.3f;
    public float dodgeCooldown = 1f;

    [Header("Input Keys")]
    [SerializeField] private KeyCode blockKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode attackKey = KeyCode.Space;
    [SerializeField] private KeyCode dodgeKey = KeyCode.Q;
    [SerializeField] private bool attackOnLeftClick = true; // Also attack on mouse left click

    [Header("References")]
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public GameObject attackVFXPrefab;

    private float nextAttackTime = 0f;
    private float nextDodgeTime = 0f;
    private bool isBlocking = false;
    private bool isDodging = false;
    private float dodgeTimer = 0f;
    private Vector3 dodgeDirection;
    private CharacterController characterController;
    private PlayerXP playerXP;

    public bool IsBlocking => isBlocking;
    public bool IsDodging => isDodging;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerXP = GetComponent<PlayerXP>();
    }

    void Update()
    {
        // Don't process input when game is paused (fight banner etc)
        if (Time.timeScale == 0f) return;

        // Handle dodge movement
        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;
            if (characterController != null)
                characterController.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);

            if (dodgeTimer <= 0f)
            {
                isDodging = false;
                Debug.Log("Dodge ended");
            }
            return;
        }

        // BLOCK: Hold configured key — play sound only when block STARTS
        bool wasBlocking = isBlocking;
        isBlocking = Input.GetKey(blockKey);
        if (isBlocking && !wasBlocking)
        {
            AudioManager.Instance?.PlayBlock();
            Debug.Log("Blocking!");
        }

        // ATTACK: Configured key or left click
        bool attackInput = Input.GetKeyDown(attackKey) || (attackOnLeftClick && Input.GetMouseButtonDown(0));
        if (!isBlocking && attackInput && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }

        // DODGE: Configured key
        if (Input.GetKeyDown(dodgeKey) && Time.time >= nextDodgeTime)
        {
            Dodge();
            nextDodgeTime = Time.time + dodgeCooldown;
        }
    }

    void Attack()
    {
        if (attackVFXPrefab != null && attackPoint != null)
            Instantiate(attackVFXPrefab, attackPoint.position, Quaternion.identity);
        Debug.Log("Player attacked!");
        GameEvents.PlayerAttack();

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                if (playerXP != null) playerXP.OnHitEnemy();
                Debug.Log("Hit " + enemy.name + " for " + attackDamage + " damage!");
            }
        }
    }

    void Dodge()
    {
        isDodging = true;
        dodgeTimer = dodgeDuration;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        AudioManager.Instance?.PlayDodge();

        if (h != 0 || v != 0)
            dodgeDirection = new Vector3(h, 0, v).normalized;
        else
            dodgeDirection = -transform.forward;

        Debug.Log("Dodge! Direction: " + dodgeDirection);
    }

    public int ModifyDamage(int incomingDamage)
    {
        if (isDodging)
        {
            if (playerXP != null) playerXP.OnSuccessfulDodge();
            return 0;
        }
        if (isBlocking)
        {
            if (playerXP != null) playerXP.OnSuccessfulBlock();
            return Mathf.RoundToInt(incomingDamage * (1f - blockDamageReduction));
        }
        return incomingDamage;
    }
}