using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // The player to follow

    [Header("Camera Position")]
    public Vector3 offset = new Vector3(0, 5f, -7f); // Distance from player
    public float smoothSpeed = 10f; // How smoothly camera follows (higher = less lag)

    [Header("Camera Angle")]
    public bool useFixedAngle = true; // Keep camera at fixed angle
    public Vector3 fixedRotation = new Vector3(30, 0, 0); // Fixed camera angle

    void Start()
    {
        if (target == null)
        {
            // Try to find player automatically
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("CameraFollow: No target assigned and couldn't find Player!");
            }
        }

        // Set initial rotation if using fixed angle
        if (useFixedAngle)
        {
            transform.rotation = Quaternion.Euler(fixedRotation);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position (player position + offset)
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move camera to that position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Keep camera at fixed angle
        if (useFixedAngle)
        {
            transform.rotation = Quaternion.Euler(fixedRotation);
        }
        else
        {
            // Alternative: Always look at player
            transform.LookAt(target);
        }
    }
}