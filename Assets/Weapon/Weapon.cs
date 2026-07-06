using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Info")]
    public string weaponName = "Weapon";
    public GameObject pickupPrefab;

    [Header("Typed Ammo")]
    public WeaponAmmoType ammoType = WeaponAmmoType.Rifle;
    public PlayerAmmoInventory ammoInventory;

    [Header("References")]
    public Camera fpsCamera;
    public Camera carCamera;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip reloadClip;
    public Animator animator;
    public WeaponShootEffect shootEffect;

    [Header("Optional Scope UI")]
    public GameObject scopeOverlay;
    public GameObject normalCrosshairUI;
    public GameObject weaponVisualRoot;

    [Header("Stats")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 8f;
    public bool automatic = false;

    [Header("Shotgun / Spread")]
    public int pellets = 1;
    public float spread = 0.015f;

    [Header("Ammo")]
    public int magazineSize = 30;
    public int ammoInMagazine = 30;

    [Tooltip("Old ammo system. Keep this so your old ammo box still works.")]
    public int reserveAmmo = 90;

    public float reloadTime = 1.5f;

    [Header("Sniper Zoom")]
    public bool enableSniperZoom = false;
    public bool zoomToggleMode = true;
    public float normalFOV = 60f;
    public float[] zoomFOVs = { 35f, 20f, 10f };
    public float zoomSpeed = 12f;
    public float scopedSpread = 0f;
    public bool hideWeaponWhenZoomed = true;

    [Header("Debug")]
    public bool debugMessages = true;

    private float nextFireTime;
    private bool reloading;

    private bool isZoomed;
    private int zoomIndex;
    private Camera currentZoomCamera;

    public bool IsZoomed
    {
        get
        {
            return isZoomed;
        }
    }

    public bool IsSniperZooming()
    {
        return enableSniperZoom && isZoomed;
    }

    public int AmmoInMagazine
    {
        get
        {
            return ammoInMagazine;
        }
    }

    public int ReserveAmmo
    {
        get
        {
            return reserveAmmo;
        }
    }

    public bool IsReloading
    {
        get
        {
            return reloading;
        }
    }

    private void Awake()
    {
        ammoInMagazine = Mathf.Clamp(ammoInMagazine, 0, magazineSize);

        if (ammoInventory == null)
            ammoInventory = FindFirstObjectByType<PlayerAmmoInventory>();

        if (shootEffect == null)
            shootEffect = GetComponent<WeaponShootEffect>();

        if (shootEffect == null)
            shootEffect = GetComponentInChildren<WeaponShootEffect>();

        if (fpsCamera != null)
            normalFOV = fpsCamera.fieldOfView;

        ResetZoomInstant();
    }

    private void OnDisable()
    {
        ResetZoomInstant();
    }

    private void Update()
    {
        HandleZoomInput();
        SmoothZoom();

        if (reloading)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        bool wantsShoot;

        if (automatic)
            wantsShoot = Input.GetMouseButton(0);
        else
            wantsShoot = Input.GetMouseButtonDown(0);

        if (wantsShoot && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo += amount;

        if (debugMessages)
            Debug.Log(weaponName + " old reserve ammo added: " + amount + " | Reserve: " + reserveAmmo);
    }

    public int AddAmmoToMagazineInstant(WeaponAmmoType incomingAmmoType, int amount)
    {
        if (incomingAmmoType != ammoType)
        {
            Debug.Log("Wrong ammo type. Weapon needs " + ammoType + " but box is " + incomingAmmoType);
            return 0;
        }

        if (amount <= 0)
            return 0;

        if (ammoInMagazine >= magazineSize)
        {
            Debug.Log(weaponName + " magazine is already full.");
            return 0;
        }

        int neededAmmo = magazineSize - ammoInMagazine;
        int addedAmmo = Mathf.Min(neededAmmo, amount);

        ammoInMagazine += addedAmmo;

        Debug.Log(
            "INSTANT AMMO ADDED TO " + weaponName +
            " | Added: " + addedAmmo +
            " | Magazine: " + ammoInMagazine + " / " + magazineSize
        );

        return addedAmmo;
    }

    private void HandleZoomInput()
    {
        if (!enableSniperZoom)
            return;

        Camera shootCam = GetShootCamera();

        if (shootCam == null)
            return;

        currentZoomCamera = shootCam;

        if (zoomToggleMode)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (isZoomed)
                    StopZoom();
                else
                    StartZoom();
            }
        }
        else
        {
            if (Input.GetMouseButton(1))
                StartZoom();
            else
                StopZoom();
        }

        if (!isZoomed)
            return;

        float scroll = Input.mouseScrollDelta.y;

        if (scroll > 0f)
        {
            zoomIndex++;
            zoomIndex = Mathf.Clamp(zoomIndex, 0, zoomFOVs.Length - 1);
        }
        else if (scroll < 0f)
        {
            zoomIndex--;
            zoomIndex = Mathf.Clamp(zoomIndex, 0, zoomFOVs.Length - 1);
        }
    }

    private void StartZoom()
    {
        isZoomed = true;

        if (scopeOverlay != null)
            scopeOverlay.SetActive(true);

        if (normalCrosshairUI != null)
            normalCrosshairUI.SetActive(false);

        if (weaponVisualRoot != null && hideWeaponWhenZoomed)
            weaponVisualRoot.SetActive(false);
    }

    private void StopZoom()
    {
        isZoomed = false;
        zoomIndex = 0;

        if (scopeOverlay != null)
            scopeOverlay.SetActive(false);

        if (normalCrosshairUI != null)
            normalCrosshairUI.SetActive(true);

        if (weaponVisualRoot != null)
            weaponVisualRoot.SetActive(true);
    }

    private void SmoothZoom()
    {
        if (!enableSniperZoom)
            return;

        if (currentZoomCamera == null)
            return;

        float targetFOV = normalFOV;

        if (isZoomed && zoomFOVs.Length > 0)
        {
            zoomIndex = Mathf.Clamp(zoomIndex, 0, zoomFOVs.Length - 1);
            targetFOV = zoomFOVs[zoomIndex];
        }

        currentZoomCamera.fieldOfView = Mathf.Lerp(
            currentZoomCamera.fieldOfView,
            targetFOV,
            zoomSpeed * Time.deltaTime
        );
    }

    private void ResetZoomInstant()
    {
        isZoomed = false;
        zoomIndex = 0;

        Camera shootCam = GetShootCamera();

        if (shootCam != null)
            shootCam.fieldOfView = normalFOV;

        if (scopeOverlay != null)
            scopeOverlay.SetActive(false);

        if (normalCrosshairUI != null)
            normalCrosshairUI.SetActive(true);

        if (weaponVisualRoot != null)
            weaponVisualRoot.SetActive(true);
    }

    private void Shoot()
    {
        Camera shootCam = GetShootCamera();

        if (shootCam == null)
        {
            Debug.LogWarning("Cannot shoot: No camera found on weapon " + weaponName);
            return;
        }

        if (ammoInMagazine <= 0)
        {
            if (debugMessages)
                Debug.Log("No ammo. Reloading...");

            StartCoroutine(Reload());
            return;
        }

        ammoInMagazine--;

        if (shootEffect != null)
            shootEffect.PlayShootEffect();

        if (debugMessages)
        {
            int typedAmmoLeft = 0;

            if (ammoInventory != null)
                typedAmmoLeft = ammoInventory.GetAmmo(ammoType);

            Debug.Log(
                weaponName + " fired. Magazine: " + ammoInMagazine +
                " / " + magazineSize +
                " | Old reserve: " + reserveAmmo +
                " | " + ammoType + " ammo: " + typedAmmoLeft
            );
        }

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (audioSource != null && shootClip != null)
            audioSource.PlayOneShot(shootClip);

        if (animator != null)
            animator.SetTrigger("Shoot");

        float currentSpread = spread;

        if (enableSniperZoom && isZoomed)
            currentSpread = scopedSpread;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = shootCam.transform.forward;

            direction += shootCam.transform.right * Random.Range(-currentSpread, currentSpread);
            direction += shootCam.transform.up * Random.Range(-currentSpread, currentSpread);
            direction.Normalize();

            Debug.DrawRay(shootCam.transform.position, direction * range, Color.red, 1f);

            Ray ray = new Ray(shootCam.transform.position, direction);

            RaycastHit[] hits = Physics.RaycastAll(
                ray,
                range,
                ~0,
                QueryTriggerInteraction.Ignore
            );

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.transform.IsChildOf(transform.root))
                    continue;

                if (debugMessages)
                    Debug.Log("Weapon hit: " + hit.collider.name);

                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

                if (damageable != null)
                {
                    damageable.TakeDamage(damage);

                    if (debugMessages)
                        Debug.Log("Damaged " + hit.collider.name + " for " + damage);
                }

                if (impactEffect != null)
                {
                    GameObject impact = Instantiate(
                        impactEffect,
                        hit.point,
                        Quaternion.LookRotation(hit.normal)
                    );

                    Destroy(impact, 2f);
                }

                break;
            }
        }
    }

    private IEnumerator Reload()
    {
        if (ammoInMagazine >= magazineSize)
            yield break;

        int typedAmmoAvailable = 0;

        if (ammoInventory != null)
            typedAmmoAvailable = ammoInventory.GetAmmo(ammoType);

        if (reserveAmmo <= 0 && typedAmmoAvailable <= 0)
        {
            if (debugMessages)
                Debug.Log("No ammo to reload for " + weaponName);

            yield break;
        }

        StopZoom();

        reloading = true;

        if (animator != null)
            animator.SetTrigger("Reload");

        if (audioSource != null && reloadClip != null)
            audioSource.PlayOneShot(reloadClip);

        if (shootEffect != null)
            shootEffect.PlayReloadSpin(reloadTime);

        yield return new WaitForSeconds(reloadTime);

        int needed = magazineSize - ammoInMagazine;

        int loadedFromOldReserve = Mathf.Min(needed, reserveAmmo);
        ammoInMagazine += loadedFromOldReserve;
        reserveAmmo -= loadedFromOldReserve;

        needed = magazineSize - ammoInMagazine;

        int loadedFromTypedAmmo = 0;

        if (needed > 0 && ammoInventory != null)
        {
            loadedFromTypedAmmo = ammoInventory.TakeAmmo(ammoType, needed);
            ammoInMagazine += loadedFromTypedAmmo;
        }

        reloading = false;

        if (debugMessages)
        {
            int typedLeft = 0;

            if (ammoInventory != null)
                typedLeft = ammoInventory.GetAmmo(ammoType);

            Debug.Log(
                weaponName + " reloaded." +
                " Magazine: " + ammoInMagazine + " / " + magazineSize +
                " | Loaded old reserve: " + loadedFromOldReserve +
                " | Loaded typed ammo: " + loadedFromTypedAmmo +
                " | Old reserve left: " + reserveAmmo +
                " | " + ammoType + " ammo left: " + typedLeft
            );
        }
    }

    private Camera GetShootCamera()
    {
        if (carCamera != null && carCamera.enabled)
            return carCamera;

        if (fpsCamera != null && fpsCamera.enabled)
            return fpsCamera;

        if (Camera.main != null)
            return Camera.main;

        Camera anyCamera = FindFirstObjectByType<Camera>();

        return anyCamera;
    }
}