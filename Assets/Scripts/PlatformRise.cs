using System.Collections;
using UnityEngine;

public class PlatformRise : MonoBehaviour
{
   [Header("Stand Detection Settings")]
    public float standTimeRequired = 3f;
    public string playerTag = "Player";
    
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
    private Transform playerOnPlatform; // Track which player is on the platform
    private Vector3 lastPlatformPosition; // Track platform position for movement calculation
    private bool shouldMovePlayer = true; // Control whether to move player with platform
    
    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.up * moveDistance;
        lastPlatformPosition = transform.position;
        
        // Check if collider exists and is trigger
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError($"No Collider found on {gameObject.name}! Please add a Collider component.");
        }
        else if (!collider.isTrigger)
        {
            Debug.LogWarning($"Collider on {gameObject.name} is not set as Trigger. Please check 'Is Trigger' in the Collider component.");
        }
        
        Debug.Log($"PlatformRise initialized on {gameObject.name}. Looking for tag: {playerTag}");
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered: {other.gameObject.name} with tag: {other.tag}");
        
        if (other.CompareTag(playerTag) && !hasActivated)
        {
            Debug.Log($"Player detected! Starting stand timer on {gameObject.name}");
            playerOnPlatform = other.transform; // Store the player's transform
            shouldMovePlayer = true; // Allow player movement
            
            if (standCoroutine == null)
            {
                playerIsStanding = true;
                standCoroutine = StartCoroutine(StandTimer());
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        Debug.Log($"Trigger exited: {other.gameObject.name} with tag: {other.tag}");
        
        if (other.CompareTag(playerTag))
        {
            Debug.Log($"Player left {gameObject.name}. Resetting timer.");
            StopMovingPlayer(); // Stop moving the player when they leave
            ResetStanding();
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag) && !playerIsStanding && !hasActivated)
        {
            Debug.Log($"Player is staying on {gameObject.name}");
            playerOnPlatform = other.transform; // Store the player's transform
            shouldMovePlayer = true; // Allow player movement
            playerIsStanding = true;
            
            if (standCoroutine == null)
            {
                standCoroutine = StartCoroutine(StandTimer());
            }
        }
    }
    
    IEnumerator StandTimer()
    {
        Debug.Log("Stand timer coroutine started!");
        standTimer = 0f;
        
        while (playerIsStanding && standTimer < standTimeRequired && !hasActivated)
        {
            standTimer += Time.deltaTime;
            
            // Update every second to reduce log spam
            if (Mathf.FloorToInt(standTimer) > Mathf.FloorToInt(standTimer - Time.deltaTime))
            {
                Debug.Log($"Standing progress: {standTimer:F1}s / {standTimeRequired}s");
            }
            
            if (standTimer >= standTimeRequired)
            {
                Debug.Log($"SUCCESS! Player stood on {gameObject.name} for {standTimeRequired} seconds!");
                OnStandSuccess();
                yield break; // Exit coroutine
            }
            
            yield return null;
        }
        
        // If we get here, standing was interrupted
        if (!hasActivated)
        {
            Debug.Log($"Standing interrupted at {standTimer:F1}s");
            ResetStanding();
        }
    }
    
    void OnStandSuccess()
    {
        Debug.Log("OnStandSuccess called!");
        hasActivated = true;
        playerIsStanding = false;
        
        if (standCoroutine != null)
        {
            StopCoroutine(standCoroutine);
            standCoroutine = null;
        }
        
        // Start moving the platform
        if (moveOnStand)
        {
            StartMoving();
        }
    }
    
    void StartMoving()
    {
        if (!isMoving)
        {
            Debug.Log("Platform starting to move upwards!");
            isMoving = true;
            StartCoroutine(MovePlatform());
        }
    }
    
    IEnumerator MovePlatform()
    {
        Debug.Log("MovePlatform coroutine started!");
        
        while (isMoving && Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            // Store position before moving
            Vector3 previousPosition = transform.position;
            
            // Move platform upwards smoothly
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            // Move the player with the platform if they're still on it and movement is allowed
            if (playerOnPlatform != null && shouldMovePlayer)
            {
                MovePlayerWithPlatform(previousPosition);
            }
            
            // Update last position for next frame
            lastPlatformPosition = transform.position;
            
            // Log movement progress occasionally
            if (Time.frameCount % 60 == 0) // Every ~second at 60fps
            {
                float progress = Vector3.Distance(startPosition, transform.position) / moveDistance;
                Debug.Log($"Moving upward: {progress:P0} complete");
            }
            
            yield return null;
        }
        
        // Ensure we reach exactly the target position
        transform.position = targetPosition;
        
        // Final player position update
        if (playerOnPlatform != null && shouldMovePlayer)
        {
            MovePlayerWithPlatform(lastPlatformPosition);
        }
        
        Debug.Log("Platform reached target position!");
        isMoving = false;
        
        // Allow player to leave naturally after platform stops
        StartCoroutine(AllowPlayerToLeave());
    }
    
    IEnumerator AllowPlayerToLeave()
    {
        Debug.Log("Platform stopped. Player can now leave naturally.");
        
        // Wait a moment to ensure platform is fully stopped
        yield return new WaitForSeconds(0.5f);
        
        // The player will now naturally leave via OnTriggerExit when they move off
        // No need to force anything - physics will handle it
    }
    
    void MovePlayerWithPlatform(Vector3 oldPosition)
    {
        if (playerOnPlatform == null || !shouldMovePlayer) return;
        
        // Calculate how much the platform moved
        Vector3 platformMovement = transform.position - oldPosition;
        
        // Move the player by the same amount
        playerOnPlatform.position += platformMovement;
        
        Debug.Log($"Moving player with platform. Movement: {platformMovement}");
    }
    
    void FixedUpdate()
    {
        // Alternative method for moving player - works better with physics
        if (isMoving && playerOnPlatform != null && shouldMovePlayer)
        {
            // This ensures smooth physics-based movement
            Vector3 platformMovement = transform.position - lastPlatformPosition;
            playerOnPlatform.position += platformMovement;
            lastPlatformPosition = transform.position;
        }
    }
    
    void StopMovingPlayer()
    {
        Debug.Log("Stopping player movement with platform");
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
    
    void Update()
    {
        // Visual feedback in Scene view
        if (playerIsStanding)
        {
            Debug.DrawRay(transform.position, Vector3.up * 3, Color.green);
        }
        else if (hasActivated)
        {
            Debug.DrawRay(transform.position, Vector3.up * 3, Color.blue);
        }
        
        // Draw movement path in editor
        Debug.DrawLine(startPosition, targetPosition, Color.yellow);
        
        // Debug: Show if player is currently being moved
        if (playerOnPlatform != null && shouldMovePlayer)
        {
            Debug.DrawLine(transform.position, playerOnPlatform.position, Color.magenta);
        }
    }
    
    // TEST FUNCTION: Call this from another script or button to test movement
    public void TestMovement()
    {
        if (!hasActivated)
        {
            Debug.Log("TEST: Manually triggering platform movement");
            OnStandSuccess();
        }
    }
    
    void OnDrawGizmos()
    {
        // Visualize the trigger area and movement path
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
        
        // Draw movement target position
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(targetPosition, GetComponent<Collider>().bounds.size);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + Vector3.up * moveDistance, GetComponent<Collider>().bounds.size);
        }
    }
}