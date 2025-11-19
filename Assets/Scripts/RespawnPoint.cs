using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [Header("Respawn Settings")]
    public string playerTag = "Player";
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.SetRespawnPoint(transform.position, transform.rotation);
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 2, 1));
        Gizmos.DrawIcon(transform.position + Vector3.up * 2, "RespawnPoint", true);
    }
}