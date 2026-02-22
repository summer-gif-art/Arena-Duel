using UnityEngine;

public class SkeletonProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime = 5f;

    private int damage = 10;
    private Vector3 direction;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Projectile hit: " + other.gameObject.name + " tag: " + other.tag);
    
        if (other.CompareTag("Enemy")) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Projectile hit player for " + damage + " damage!");
            }
        }

        Destroy(gameObject);
    }
}
