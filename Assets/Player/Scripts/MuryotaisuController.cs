using UnityEngine;

namespace Muryotaisu
{
    // Controls all player movement: walking, jumping, rotation, and idle animations.
    // Camera-relative movement means W always moves toward the camera's forward direction.
    // Movement is locked until the fight starts via the canMove flag set by ArenaManager.
    public class MuryotaisuController : MonoBehaviour
    {
        private Animator animator;
        private CharacterController controller;
        private Vector3 moveDirection = Vector3.zero;

        [Header("Movement Settings")]
        public float speed = 5f;           // Walking speed
        public float jumpSpeed = 8f;       // Initial jump velocity
        public float gravity = 20f;        // Gravity applied while airborne
        public float rotationSpeed = 10f;  // How fast player turns to face movement direction
        public float startKocchi = 2f;     // Distance to camera that triggers wave animation

        [Header("Fight Control")]
        public bool canMove = false; // Locked until ArenaManager starts the fight

        float idleTimer = 0f; // Tracks time standing still for idle animation switch

        void Start()
        {
            // Get components from this GameObject (Animator is on the root for this character)
            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            // Wave animation when camera gets very close to player
            float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
            animator.SetBool("kocchiFlag", dist < startKocchi);

            // Block all movement input until fight officially starts
            if (!canMove)
            {
                // Still apply gravity so player doesn't float
                moveDirection.x = 0f;
                moveDirection.z = 0f;
                if (!controller.isGrounded)
                    moveDirection.y -= gravity * Time.deltaTime;
                else
                    moveDirection.y = -2f;
                controller.Move(moveDirection * Time.deltaTime);
                animator.SetBool("walkFlag", false);
                return;
            }

            if (controller.isGrounded)
            {
                moveDirection.y = -2f; // Small downward force to keep grounded

                // Read raw input (no smoothing for responsive controls)
                float horizontal = Input.GetAxisRaw("Horizontal"); // A=-1, D=1
                float vertical = Input.GetAxisRaw("Vertical");     // S=-1, W=1

                // Calculate movement relative to camera orientation
                Transform cam = Camera.main.transform;
                Vector3 camForward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
                Vector3 camRight = new Vector3(cam.right.x, 0f, cam.right.z).normalized;
                Vector3 inputDir = (camForward * vertical + camRight * horizontal).normalized;

                bool isMoving = inputDir.magnitude >= 0.1f;

                if (isMoving)
                {
                    // Apply movement in camera-relative direction
                    moveDirection.x = inputDir.x * speed;
                    moveDirection.z = inputDir.z * speed;

                    // Smoothly rotate to face movement direction
                    Quaternion targetRot = Quaternion.LookRotation(inputDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                    idleTimer = 0f;
                }
                else
                {
                    moveDirection.x = 0f;
                    moveDirection.z = 0f;

                    // After 15 seconds idle, trigger alternate idle animation
                    idleTimer += Time.deltaTime;
                    if (idleTimer >= 15f)
                    {
                        animator.SetTrigger("idleBFlag");
                        idleTimer = 0f;
                    }
                }

                // Jump on Space or Enter
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {
                    moveDirection.y = jumpSpeed;
                    animator.SetBool("jumpFlag", true);
                }

                // Update walk animation and reset jump when grounded
                animator.SetBool("walkFlag", isMoving);
                animator.SetBool("idleFlag", false);
                if (isMoving) animator.SetBool("jumpFlag", false);
            }
            else
            {
                // In air: apply gravity, disable walk animation
                moveDirection.y -= gravity * Time.deltaTime;
                animator.SetBool("walkFlag", false);

                // Reset jump flag once falling
                if (moveDirection.y < 0)
                    animator.SetBool("jumpFlag", false);
            }

            // Apply final movement to CharacterController
            controller.Move(moveDirection * Time.deltaTime);
        }
    }
}