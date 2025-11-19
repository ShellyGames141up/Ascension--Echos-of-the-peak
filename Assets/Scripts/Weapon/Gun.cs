using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    [Header("References")]
    public Transform barrelEnd;
    public GameObject bulletPrefab;
    public Camera playerCamera;
    
    [Header("Shooting Settings")]
    public float fireRate = 0.15f;
    public int bulletsPerShot = 1;
    public float spreadAngle = 1f;
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 1.5f;
    
    [Header("Recoil Settings")]
    public float recoilRotation = 2f;
    public float recoilKickback = 0.1f;
    public float recoilRecoverySpeed = 5f;
    
    [Header("Sound Effects")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;
    
    [Header("Events")]
    public UnityEvent OnShoot;
    public UnityEvent OnReload;
    public UnityEvent OnAmmoChange;
    
    private float nextFireTime;
    private bool isReloading;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private AudioSource audioSource;

    void Start()
    {
        currentAmmo = maxAmmo;
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f; // 2D sound
        }
        
        Debug.Log("Gun initialized with " + currentAmmo + " ammo");
    }
    
    void Update()
    {
        HandleRecoilRecovery();
        HandleInput();
    }
    private void HandleInput()
    {
        if (isReloading) return;
        
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            TryShoot();
        }
        
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartReload();
        }
    }
    
    private void TryShoot()
    {
        if (Time.time < nextFireTime) return;
        if (isReloading) return;

        if (currentAmmo > 0)
        {
            Shoot();
        }
        else
        {
            PlayEmptySound();
        }
    }
    
    private void Shoot()
    {
        nextFireTime = Time.time + fireRate;
        currentAmmo--;
        
        ApplyRecoil();
        
        
        PlayShootSound();
        
        
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shootDirection = CalculateShootDirection();
            SpawnBullet(shootDirection);
        }
        
        // Trigger events
        OnShoot?.Invoke();
        OnAmmoChange?.Invoke();
        
        Debug.Log($"Shot fired! Ammo: {currentAmmo}/{maxAmmo}");
    }
    
    private void PlayShootSound()
    {
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
    
    private void PlayEmptySound()
    {
        if (emptySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(emptySound);
        }
    }
    
    private void PlayReloadSound()
    {
        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
    }
    
    private Vector3 CalculateShootDirection()
    {
        Vector3 direction = playerCamera.transform.forward;
        
        if (spreadAngle > 0)
        {
            float spreadX = Random.Range(-spreadAngle, spreadAngle);
            float spreadY = Random.Range(-spreadAngle, spreadAngle);
            direction = Quaternion.Euler(spreadY, spreadX, 0) * direction;
        }
        
        return direction;
    }
    private void SpawnBullet(Vector3 direction)
    {
        if (bulletPrefab == null || barrelEnd == null) 
        {
            Debug.LogWarning("Bullet prefab or barrel end not assigned!");
            return;
        }
        GameObject bullet = Instantiate(bulletPrefab, barrelEnd.position, Quaternion.LookRotation(direction));
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        
        if (bulletScript != null)
        {
            bulletScript.Initialize(direction);
        }
    }
    private void ApplyRecoil()
    {
       
        Vector3 recoilRot = new Vector3(-recoilRotation, Random.Range(-recoilRotation/2, recoilRotation/2), 0);
        transform.localRotation = Quaternion.Euler(recoilRot) * originalRotation;
        transform.localPosition = originalPosition + (-transform.forward * recoilKickback);
    }

    private void HandleRecoilRecovery()
    {
        
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, recoilRecoverySpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, recoilRecoverySpeed * Time.deltaTime);
    }

    private void StartReload()
    {
        isReloading = true;
        PlayReloadSound();
        Invoke(nameof(FinishReload), reloadTime);
        OnReload?.Invoke();
        
        Debug.Log("Reloading...");
    }

    private void FinishReload()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        OnAmmoChange?.Invoke();
        
        Debug.Log("Reload complete!");
    }
    
    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        OnAmmoChange?.Invoke();
        
        Debug.Log($"Added {amount} ammo. Total: {currentAmmo}");
    }

    public bool CanShoot()
    {
        return !isReloading && currentAmmo > 0 && Time.time >= nextFireTime;
    }
    
    public bool IsReloading()
    {
        return isReloading;
    }
}