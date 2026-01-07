using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;
    
    private CharacterController controller;
    private Vector3 velocity;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (controller == null)
        {
            Debug.LogError("Character Controller not found on Player!");
        }
    }
    
    void Update()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down
        
        // Calculate movement direction
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        
        // Move the player
        if (moveDirection.magnitude >= 0.1f)
        {
            // Move
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
            
            // Rotate player to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small value to keep grounded
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}