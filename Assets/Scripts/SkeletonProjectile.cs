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
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        // Destroy on hitting walls/floor
        if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
