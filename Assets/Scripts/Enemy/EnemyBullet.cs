using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 25;
    public float lifetime = 5f;
    
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    
    private void OnTriggerEnter(Collider other)
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
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}