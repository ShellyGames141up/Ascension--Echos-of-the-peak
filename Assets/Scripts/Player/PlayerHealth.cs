using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    
    public UnityEvent OnPlayerDeath;
    public UnityEvent OnPlayerDamaged;
    
    private FPSPlayerController playerController;
    private bool isDead = false;
    
    private void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<FPSPlayerController>();
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        OnPlayerDamaged?.Invoke();
        
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        isDead = true;
        Debug.Log("Player died!");
        
        if (playerController != null)
        {
            playerController.SetInputEnabled(false);
        }
        
        OnPlayerDeath?.Invoke();
    }
    
    private void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
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