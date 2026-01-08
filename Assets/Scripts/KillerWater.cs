using UnityEngine;

public class WaterKillZone : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform spawnPoint;
    public int drowningDamage = 30;
    
    [Header("Visual Effects")]
    public GameObject splashEffect; // Optional: water splash particle
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePlayerFall(other);
        }
        else if (other.CompareTag("Enemy"))
        {
            // Enemies also die in water
            Destroy(other.gameObject);
        }
    }
    
    void HandlePlayerFall(Collider player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        CharacterController controller = player.GetComponent<CharacterController>();
        
        if (playerHealth != null && spawnPoint != null)
        {
            // Take drowning damage
            playerHealth.TakeDamage(drowningDamage);
            
            // Spawn splash effect if assigned
            if (splashEffect != null)
            {
                Instantiate(splashEffect, player.transform.position, Quaternion.identity);
            }
            
            // Respawn player at spawn point
            if (controller != null)
            {
                controller.enabled = false; // Disable to teleport
                player.transform.position = spawnPoint.position;
                player.transform.rotation = spawnPoint.rotation;
                controller.enabled = true; // Re-enable
            }
            
            Debug.Log("Player fell in water! Took damage and respawned.");
        }
    }
}