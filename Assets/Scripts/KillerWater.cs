using UnityEngine;

public class WaterDamage : MonoBehaviour
{
    [Header("Settings")]
    public int damageAmount = 100; // Set to 100 for instant kill
    public bool instantKill = true;

    // This function runs automatically when something enters the Trigger
    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if it's the Player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player fell in water!");
            // Call your player's damage function here
            // Example: other.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
        }
        
        // 2. Check if it's an Enemy
        else if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy fell in water!");
            // Example: other.GetComponent<EnemyHealth>().TakeDamage(damageAmount);
            
            // Optional: Destroy enemy immediately to save performance
            // Destroy(other.gameObject); 
        }
    }
}