using UnityEngine;

public class DissapearingPlatforms : MonoBehaviour
{
     [Header("Disappearance Settings")]
    public string playerTag = "Player";
    public float disappearDelay = 0f;
    public bool destroyObject = true; // If false, just disable it
    
    [Header("Visual Effects")]
    public bool playParticleEffect = false;
    public ParticleSystem disappearParticles;
    public bool playSound = false;
    public AudioClip disappearSound;
    
    private Collider objectCollider;
    private Renderer objectRenderer;
    private AudioSource audioSource;
    
    void Start()
    {
        // Get components
        objectCollider = GetComponent<Collider>();
        objectRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        
        // If we need audio but no AudioSource, add one
        if (playSound && audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log($"Player collided with {gameObject.name}. Disappearing in {disappearDelay} seconds.");
            StartCoroutine(Disappear());
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            Debug.Log($"Player collided with {gameObject.name}. Disappearing in {disappearDelay} seconds.");
            StartCoroutine(Disappear());
        }
    }
    
    private System.Collections.IEnumerator Disappear()
    {
       
        if (disappearDelay > 0)
        {
            yield return new WaitForSeconds(disappearDelay);
        }
        
      
        if (playParticleEffect && disappearParticles != null)
        {
            disappearParticles.Play();
            
            yield return new WaitForSeconds(0.1f);
        }
        
        if (playSound && disappearSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(disappearSound);
         
            yield return new WaitForSeconds(disappearSound.length);
        }
        
       
        if (destroyObject)
        {
            Destroy(gameObject);
        }
        else
        {
           
            if (objectCollider != null) objectCollider.enabled = false;
            if (objectRenderer != null) objectRenderer.enabled = false;
            if (audioSource != null) audioSource.enabled = false;
            
         
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        
        Debug.Log($"{gameObject.name} has disappeared!");
    }
    
    
    public void TriggerDisappearance()
    {
        StartCoroutine(Disappear());
    }
    
    void OnDrawGizmos()
    {
       
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}
