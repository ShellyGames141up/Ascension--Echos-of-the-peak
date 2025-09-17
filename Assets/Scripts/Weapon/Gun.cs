using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    public Transform barrelEnd;
    public GameObject bulletPrefab;
    public Camera playerCamera;
    public float fireRate = 0.15f;
    public int bulletsPerShot = 1;
    public float spreadAngle = 1f;
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 1.5f;
    public float recoilRotation = 2f;
    public float recoilKickback = 0.1f;
    public float recoilRecoverySpeed = 5f;
    
    public UnityEvent OnShoot;
    public UnityEvent OnReload;
    public UnityEvent OnAmmoChange;
    private float nextFireTime;
    private bool isReloading;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        currentAmmo = maxAmmo;
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
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
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
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
    }
    private void Shoot()
    {
        nextFireTime = Time.time + fireRate;
        currentAmmo--;
        ApplyRecoil();
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shootDirection = CalculateShootDirection();
            SpawnBullet(shootDirection);
        }
        OnShoot?.Invoke();
        OnAmmoChange?.Invoke();
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
        if (bulletPrefab == null || barrelEnd == null) return;

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
        Invoke("FinishReload", reloadTime);
        OnReload?.Invoke();
    }

    private void FinishReload()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        OnAmmoChange?.Invoke();
    }
    
    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        OnAmmoChange?.Invoke();
    }

    public bool CanShoot()
    {
        return !isReloading && currentAmmo > 0 && Time.time >= nextFireTime;
    }
}