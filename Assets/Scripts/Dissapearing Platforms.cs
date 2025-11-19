using UnityEngine;
using System.Collections;

public class DissapearingPlatforms : MonoBehaviour
{
    [Header("Disappearance Settings")]
    public string playerTag = "Player";
    public float disappearDelay = 0f;
    public bool destroyObject = false;
    
    [Header("Visual Effects")]
    public bool playParticleEffect = false;
    public ParticleSystem disappearParticles;
    public bool playSound = false;
    public AudioClip disappearSound;
    
    private Collider objectCollider;
    private Renderer objectRenderer;
    private AudioSource audioSource;
    private bool isActive = true;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    
    void Start()
    {
        objectCollider = GetComponent<Collider>();
        objectRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
        
     
        if (playSound && audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }
        Debug.Log($"Disappearing platform initialized: {gameObject.name}");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && isActive)
        {
            Debug.Log($"Player triggered {gameObject.name}. Disappearing in {disappearDelay} seconds.");
            StartCoroutine(Disappear());
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag) && isActive)
        {
            Debug.Log($"Player collided with {gameObject.name}. Disappearing in {disappearDelay} seconds.");
            StartCoroutine(Disappear());
        }
    }
    
    private IEnumerator Disappear()
    {
        isActive = false;
        
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
            
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        
        Debug.Log($"{gameObject.name} has disappeared!");
    }
    public void ResetPlatform()
    {
        if (destroyObject && !isActive)
        {
            Debug.LogWarning($"Cannot reset {gameObject.name} - object is set to destroy and has been disabled.");
            return;
        }
        StopAllCoroutines();
        isActive = true;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;
        
        if (objectCollider != null) objectCollider.enabled = true;
        if (objectRenderer != null) objectRenderer.enabled = true;
        
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        
        Debug.Log($"{gameObject.name} has been reset!");
    }
    public void TriggerDisappearance()
    {
        if (isActive)
        {
            StartCoroutine(Disappear());
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = isActive ? Color.yellow : Color.red;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}