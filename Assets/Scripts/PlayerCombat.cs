using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public int attackDamage = 20;
    public float attackCooldown = 0.5f;

    [Header("Block Settings")]
    public float blockDamageReduction = 0.75f; // blocks 75% of damage
    
    [Header("Dodge Settings")]
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.3f;
    public float dodgeCooldown = 1f;

    [Header("References")]
    public Transform attackPoint;
    public LayerMask enemyLayer;

    // Private variables
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
        // Don't allow actions during dodge
        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;
            if (characterController != null)
            {
                characterController.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(dodgeDirection * dodgeSpeed * Time.deltaTime, Space.World);
            }

            if (dodgeTimer <= 0f)
            {
                isDodging = false;
                Debug.Log("Dodge ended");
            }
            return; // skip other inputs while dodging
        }

        // BLOCK: Hold Left Shift
        AudioManager.Instance?.PlayBlock();
        isBlocking = Input.GetKey(KeyCode.LeftShift);
        if (isBlocking)
        {
            Debug.Log("Blocking!");
        }

        // ATTACK: Space or Left Click (can't attack while blocking)
        if (!isBlocking && 
            (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && 
            Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }

        // DODGE: Press Q
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= nextDodgeTime)
        {
            Dodge();
            nextDodgeTime = Time.time + dodgeCooldown;
        }
    }

    void Attack()
    {
        Debug.Log("Player attacked!");

        Collider[] hitEnemies = Physics.OverlapSphere(
            attackPoint.position, 
            attackRange, 
            enemyLayer
        );

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

        // Dodge in the direction the player is moving, or backward if standing still
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        AudioManager.Instance?.PlayDodge();

        if (h != 0 || v != 0)
        {
            dodgeDirection = new Vector3(h, 0, v).normalized;
        }
        else
        {
            dodgeDirection = -transform.forward; // dodge backward by default
        }

        Debug.Log("Dodge! Direction: " + dodgeDirection);
    }

    // Call this from your player health script when taking damage
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