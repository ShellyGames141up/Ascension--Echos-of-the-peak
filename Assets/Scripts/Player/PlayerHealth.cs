using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
       [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public int bulletDamage = 10; 
    
    private FPSPlayerController playerController;
    private bool isDead = false;
    private Vector3 respawnPosition;
    private Quaternion respawnRotation;

    private void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<FPSPlayerController>();
        
        
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            TakeDamage(bulletDamage);
            Destroy(collision.gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            TakeDamage(bulletDamage);
            Destroy(other.gameObject); 
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player has died!"); 

       
        if (playerController != null)
        {
            playerController.SetInputEnabled(false);
        }

      
        Invoke("Respawn", 2f); 
    }
    
    private void Respawn()
    {
       
        currentHealth = maxHealth;
        isDead = false;
        
        
        transform.position = respawnPosition;
        transform.rotation = respawnRotation;
        
      
        ResetAllPlatforms();
       
        if (playerController != null)
        {
            playerController.SetInputEnabled(true);
        }
        
        Debug.Log("Player respawned at start position");
    }

    private void ResetAllPlatforms()
    {
        
        DissapearingPlatforms[] disappearingPlatforms = FindObjectsOfType<DissapearingPlatforms>();
        foreach (DissapearingPlatforms platform in disappearingPlatforms)
        {
            platform.ResetPlatform();
        }

        PlatformRise[] risingPlatforms = FindObjectsOfType<PlatformRise>();
        foreach (PlatformRise platform in risingPlatforms)
        {
            platform.ResetPlatform();
        }
    }

    public void SetRespawnPoint(Vector3 position, Quaternion rotation)
    {
        respawnPosition = position;
        respawnRotation = rotation;
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
    }

    public bool IsDead()
    {
        return isDead;
    }
}
