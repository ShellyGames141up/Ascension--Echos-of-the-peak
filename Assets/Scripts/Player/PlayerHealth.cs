using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    
    private FPSPlayerController playerController;
    private bool isDead = false;
    private Vector3 respawnPosition;
    private Quaternion respawnRotation;
    
    private void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<FPSPlayerController>();
        
        // Set initial respawn point to starting position
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
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
        
        // Disable player input
        if (playerController != null)
        {
            playerController.SetInputEnabled(false);
        }
        
        // Auto-respawn after 2 seconds
        Invoke(nameof(Respawn), 2f);
    }
    
    private void Respawn()
    {
        // Reset health
        currentHealth = maxHealth;
        isDead = false;
        
        // Reset position and rotation
        transform.position = respawnPosition;
        transform.rotation = respawnRotation;
        
        // Reset all platforms in the scene
        ResetAllPlatforms();
        
        // Re-enable player input
        if (playerController != null)
        {
            playerController.SetInputEnabled(true);
        }
    }
    
    private void ResetAllPlatforms()
    {
        // Reset disappearing platforms
        DissapearingPlatforms[] disappearingPlatforms = FindObjectsOfType<DissapearingPlatforms>();
        foreach (DissapearingPlatforms platform in disappearingPlatforms)
        {
            platform.ResetPlatform();
        }
        
        // Reset rising platforms
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