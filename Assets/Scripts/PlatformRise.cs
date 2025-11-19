using System.Collections;
using UnityEngine;

public class PlatformRise : MonoBehaviour
{
    [Header("Stand Detection Settings")]
    public string playerTag = "Player";
    public float standTimeRequired = 3f;
    
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float moveDistance = 5f;
    public bool moveOnStand = true;
    
    private float standTimer = 0f;
    private bool playerIsStanding = false;
    private bool hasActivated = false;
    private bool isMoving = false;
    private Coroutine standCoroutine;
    
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Transform playerOnPlatform;
    private bool shouldMovePlayer = true;
    
    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.up * moveDistance;
        
        // Validate collider
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError($"No Collider found on {gameObject.name}!");
        }
        else if (!collider.isTrigger)
        {
            Debug.LogWarning($"Collider on {gameObject.name} is not set as Trigger.");
        }
        
        Debug.Log($"PlatformRise initialized on {gameObject.name}");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !hasActivated)
        {
            playerOnPlatform = other.transform;
            shouldMovePlayer = true;
            
            if (standCoroutine == null)
            {
                playerIsStanding = true;
                standCoroutine = StartCoroutine(StandTimer());
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            StopMovingPlayer();
            ResetStanding();
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag) && !playerIsStanding && !hasActivated)
        {
            playerOnPlatform = other.transform;
            shouldMovePlayer = true;
            playerIsStanding = true;
            
            if (standCoroutine == null)
            {
                standCoroutine = StartCoroutine(StandTimer());
            }
        }
    }
    
    IEnumerator StandTimer()
    {
        standTimer = 0f;
        
        while (playerIsStanding && standTimer < standTimeRequired && !hasActivated)
        {
            standTimer += Time.deltaTime;
            
            if (standTimer >= standTimeRequired)
            {
                OnStandSuccess();
                yield break;
            }
            
            yield return null;
        }
        
        if (!hasActivated)
        {
            ResetStanding();
        }
    }
    
    void OnStandSuccess()
    {
        hasActivated = true;
        playerIsStanding = false;
        
        if (standCoroutine != null)
        {
            StopCoroutine(standCoroutine);
            standCoroutine = null;
        }
        
        if (moveOnStand)
        {
            StartMoving();
        }
    }
    
    void StartMoving()
    {
        if (!isMoving)
        {
            isMoving = true;
            StartCoroutine(MovePlatform());
        }
    }
    
    IEnumerator MovePlatform()
    {
        while (isMoving && Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            Vector3 previousPosition = transform.position;
            
            // Move platform
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            // Move player with platform
            if (playerOnPlatform != null && shouldMovePlayer)
            {
                Vector3 platformMovement = transform.position - previousPosition;
                playerOnPlatform.position += platformMovement;
            }
            
            yield return null;
        }
        
        transform.position = targetPosition;
        isMoving = false;
        
        // Allow player to leave naturally
        StartCoroutine(AllowPlayerToLeave());
    }
    
    IEnumerator AllowPlayerToLeave()
    {
        yield return new WaitForSeconds(0.5f);
    }
    
    void StopMovingPlayer()
    {
        shouldMovePlayer = false;
        playerOnPlatform = null;
    }
    
    void ResetStanding()
    {
        playerIsStanding = false;
        standTimer = 0f;
        StopMovingPlayer();
        
        if (standCoroutine != null && !hasActivated)
        {
            StopCoroutine(standCoroutine);
            standCoroutine = null;
        }
    }
    
    public void ResetPlatform()
    {
        StopAllCoroutines();
        
        // Reset platform position
        transform.position = startPosition;
        
        // Reset state variables
        standTimer = 0f;
        playerIsStanding = false;
        hasActivated = false;
        isMoving = false;
        shouldMovePlayer = true;
        playerOnPlatform = null;
        
        Debug.Log($"{gameObject.name} has been reset!");
    }
    
    void Update()
    {
        // Visual feedback
        if (playerIsStanding)
        {
            Debug.DrawRay(transform.position, Vector3.up * 3, Color.green);
        }
        else if (hasActivated)
        {
            Debug.DrawRay(transform.position, Vector3.up * 3, Color.blue);
        }
        
        // Draw movement path
        Debug.DrawLine(startPosition, targetPosition, Color.yellow);
    }
    
    void OnDrawGizmos()
    {
        // Visualize trigger area
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Gizmos.color = playerIsStanding ? Color.green : (hasActivated ? Color.blue : Color.yellow);
            Gizmos.matrix = transform.localToWorldMatrix;
            
            if (collider is BoxCollider boxCollider)
            {
                Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
            }
            else if (collider is SphereCollider sphereCollider)
            {
                Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
            }
        }
        
        // Draw movement target
        Gizmos.color = Color.red;
        if (Application.isPlaying)
        {
            Gizmos.DrawWireCube(targetPosition, GetComponent<Collider>().bounds.size);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position + Vector3.up * moveDistance, GetComponent<Collider>().bounds.size);
        }
    }
}