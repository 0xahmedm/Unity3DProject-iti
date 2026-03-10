using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

/// <summary>
/// Handles shooting, fire rate, reload, and muzzle flash
/// for a single weapon. Each weapon prefab has this script.
/// Requires AmmoManager on the Player (found at runtime).
/// </summary>
public class Weapon : MonoBehaviour
{
    [Header("Weapon Configuration")]
    [SerializeField] private WeaponData weaponData;

    [Header("References")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private ParticleSystem muzzleFlash;

    [Header("Impact")]
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private float impactLifetime = 2f;

    [Header("Bullet Tracer")]
    [SerializeField] private LineRenderer tracerPrefab;
    [SerializeField] private float tracerDuration = 0.05f;

    private AmmoManager ammoManager;
    private float nextFireTime;
    private bool isReloading;

    // Per-weapon ammo state (persisted across weapon switches)
    private int savedMagazineAmmo;
    private int savedReserveAmmo;
    private bool isInitialized;

    public WeaponData Data => weaponData;
    public bool IsReloading => isReloading;
    public int SavedMagazineAmmo => savedMagazineAmmo;
    public int SavedReserveAmmo => savedReserveAmmo;

    private void Awake()
    {
        ammoManager = GetComponentInParent<AmmoManager>();
    }

    private void OnEnable()
    {
        isReloading = false;

        if (!isInitialized)
        {
            savedMagazineAmmo = weaponData.magazineSize;
            savedReserveAmmo = weaponData.maxReserveAmmo;
            isInitialized = true;
        }

        ammoManager?.SetWeapon(weaponData, savedMagazineAmmo, savedReserveAmmo);
    }

    private void OnDisable()
    {
        SaveAmmoState();
        StopAllCoroutines();
    }

    private void Update()
    {
        HandleShootingInput();
        HandleReloadInput();
    }

    private void HandleShootingInput()
    {
        if (isReloading) return;

        bool firePressed = weaponData.isAutomatic
            ? Input.GetButton("Fire1")
            : Input.GetButtonDown("Fire1");

        if (firePressed && Time.time >= nextFireTime)
        {
            Debug.Log("bullet fired yaaaaaay");
            nextFireTime = Time.time + 1f / weaponData.fireRate;
            Shoot();
        }
    }

    private void HandleReloadInput()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            TryReload();
        }
    }

    private void Shoot()
    {
        if (!ammoManager.ConsumeAmmo())
        {
            TryReload();
            return;
        }

        PlayMuzzleFlash();
        PerformRaycast();
        SaveAmmoState();
    }

    private void PerformRaycast()
    {
        Camera cam = GetActiveCamera();
        if (cam == null)
        {
            Debug.LogWarning("[Weapon] No camera found!");
            return;
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Ignore the Player layer so the raycast doesn't hit ourselves
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << playerLayer);

        Vector3 muzzlePos = muzzlePoint != null ? muzzlePoint.position : ray.origin;

        if (Physics.Raycast(ray, out RaycastHit hit, weaponData.range, layerMask))
        {
            SpawnTracer(muzzlePos, hit.point);
            SpawnImpactEffect(hit);

            IDamageable target = hit.transform.GetComponentInParent<IDamageable>();
            target?.TakeDamage(weaponData.damage, weaponData.weaponName, hit.transform.name);
        }
        else
        {
            // Tracer into the distance if nothing was hit
            SpawnTracer(muzzlePos, ray.GetPoint(weaponData.range));
        }
    }

    /// <summary>
    /// Finds the active camera. Tries Camera.main first,
    /// then falls back to CinemachineBrain's output camera.
    /// </summary>
    private Camera GetActiveCamera()
    {
        if (Camera.main != null) return Camera.main;

        CinemachineBrain brain = Object.FindAnyObjectByType<CinemachineBrain>();
        return brain != null ? brain.OutputCamera : null;
    }

    private void SpawnTracer(Vector3 start, Vector3 end)
    {
        if (tracerPrefab == null) return;

        LineRenderer tracer = Instantiate(tracerPrefab);
        tracer.SetPosition(0, start);
        tracer.SetPosition(1, end);
        Destroy(tracer.gameObject, tracerDuration);
    }

    private void SpawnImpactEffect(RaycastHit hit)
    {
        if (impactEffectPrefab == null) return;

        GameObject impact = Instantiate(
            impactEffectPrefab,
            hit.point,
            Quaternion.LookRotation(hit.normal)
        );
        Destroy(impact, impactLifetime);
    }

    private void PlayMuzzleFlash()
    {
        if (muzzleFlash == null) return;
        muzzleFlash.Play();
    }

    private void TryReload()
    {
        if (!ammoManager.CanReload()) return;
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log($"[{weaponData.weaponName}] Reloading...");

        yield return new WaitForSeconds(weaponData.reloadTime);

        ammoManager.PerformReload();
        isReloading = false;
        SaveAmmoState();

        Debug.Log($"[{weaponData.weaponName}] Reload complete.");
    }

    private void SaveAmmoState()
    {
        if (ammoManager == null) return;
        savedMagazineAmmo = ammoManager.CurrentMagazineAmmo;
        savedReserveAmmo = ammoManager.ReserveAmmo;
    }
}
