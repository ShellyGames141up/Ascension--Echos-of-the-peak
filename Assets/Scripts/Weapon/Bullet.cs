using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float maxLifetime = 3f;
    public int damage = 25;
    public LayerMask collisionLayers;
    public GameObject impactEffect;

    private Vector3 direction;
    private float lifetime;
    private bool hasHit;

    void Update()
    {
        if (hasHit) return;

        transform.position += direction * (speed * Time.deltaTime);

        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 shootDirection)
    {
        direction = shootDirection.normalized;
        transform.forward = direction;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        
        if (((1 << other.gameObject.layer) & collisionLayers) != 0)
        {
            HandleImpact();
        }
    }

    void HandleImpact()
    {
        hasHit = true;

        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.LookRotation(transform.forward));
        }

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.1f);
    }
}