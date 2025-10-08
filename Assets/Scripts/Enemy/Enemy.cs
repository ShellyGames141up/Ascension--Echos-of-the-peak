using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health = 100f;
    public float timeBetweenAttacks = 2f;
    public GameObject projectile;
    public float sightRange = 10f;
    public float attackRange = 5f;
    public Vector3 walkPoint;
    public float walkPointRange = 10f;
    public Transform firePoint;
    public Animator animator;
    public string moveAnimation = "Move";
    public string attackAnimation = "Attack";
    public string dieAnimation = "Die";
    public string idleAnimation = "Idle";
    public AudioSource audioSource;
    public AudioClip loopingSound;
    public AudioClip attackSound;
    public AudioClip deathSound;
    public AudioClip hurtSound;
    [Range(0f, 1f)] public float loopingVolume = 0.3f;
    [Range(0f, 1f)] public float effectVolume = 0.7f;
    
    private bool walkPointSet;
    private bool alreadyAttacked;
    private bool isDead = false;
    private bool playerInSightRange, playerInAttackRange;
    private bool isAnimatingDeath = false;
    private bool soundPlaying = false;
    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        agent = GetComponent<NavMeshAgent>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("No Animator found on Enemy!");
            }
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        audioSource.spatialBlend = 1f; // 3D sound
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 15f;
        audioSource.volume = loopingVolume;
        if (firePoint == null)
        {
            CreateFirePoint();
        }
    }

    private void Start()
    {
        if (loopingSound != null)
        {
            PlayLoopingSound();
        }
    }

    private void CreateFirePoint()
    {
        GameObject firePointObj = new GameObject("FirePoint");
        firePointObj.transform.SetParent(transform);
        firePointObj.transform.localPosition = new Vector3(0f, 1.5f, 1f);
        firePoint = firePointObj.transform;
    }

    private void Update()
    {
        if (isDead) return;
        
        if (player == null) 
        {
            SetAnimationState("Idle");
            return;
        }
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        
        if (!playerInSightRange && !playerInAttackRange) 
        {
            Patroling();
            SetAnimationState("Move");
        }
        else if (playerInSightRange && !playerInAttackRange) 
        {
            ChasePlayer();
            SetAnimationState("Move");
        }
        else if (playerInAttackRange && playerInSightRange) 
        {
            AttackPlayer();
            SetAnimationState("Attack");
        }
    }

    private void SetAnimationState(string state)
    {
        if (animator == null || isAnimatingDeath) return;
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsIdle", false);
        
        switch (state)
        {
            case "Move":
                animator.SetBool("IsMoving", true);
                break;
            case "Attack":
                animator.SetBool("IsAttacking", true);
                break;
            case "Idle":
                animator.SetBool("IsIdle", true);
                break;
        }
    }

    private void PlayLoopingSound()
    {
        if (loopingSound != null && audioSource != null && !soundPlaying)
        {
            audioSource.clip = loopingSound;
            audioSource.loop = true;
            audioSource.volume = loopingVolume;
            audioSource.Play();
            soundPlaying = true;
        }
    }

    private void StopLoopingSound()
    {
        if (audioSource != null && soundPlaying)
        {
            audioSource.Stop();
            soundPlaying = false;
        }
    }

    private void PlayOneShotSound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null && !isDead)
        {
            audioSource.PlayOneShot(clip, volume * effectVolume);
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) 
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; 
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        if (!alreadyAttacked)
        {
            if (animator != null)
            {
                animator.SetTrigger("AttackTrigger");
            }
            
            PlayOneShotSound(attackSound);
            Invoke(nameof(FireProjectile), 0.3f); 

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void FireProjectile()
    {
        if (firePoint != null && projectile != null && player != null)
        {
            GameObject bullet = Instantiate(projectile, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                Vector3 direction = (player.position - firePoint.position).normalized;
                rb.AddForce(direction * 32f, ForceMode.Impulse);
                rb.AddForce(transform.up * 4f, ForceMode.Impulse);
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        health -= damage;
        Debug.Log($"Enemy took {damage} damage. Health: {health}");
        
        PlayOneShotSound(hurtSound);
        
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (health <= 0) 
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        isAnimatingDeath = true;
        agent.isStopped = true;
        
        StopLoopingSound();
        
        PlayOneShotSound(deathSound);
        
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        Debug.Log("Enemy died!");
        
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        Invoke(nameof(DestroyEnemy), 3f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    
    public void OnDeathAnimationComplete()
    {
        DestroyEnemy();
    }
    
    public void OnAttackAnimationEvent()
    {
        PlayOneShotSound(attackSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && !isDead)
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
            }
            
            Destroy(other.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        
        if (firePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(firePoint.position, 0.1f);
        }
    }
}