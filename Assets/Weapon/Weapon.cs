using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Info")]
    public string weaponName = "Weapon";
    public GameObject pickupPrefab;

    [Header("References")]
    public Camera fpsCamera;
    public Camera carCamera;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip reloadClip;
    public Animator animator;

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
    public int reserveAmmo = 90;
    public float reloadTime = 1.5f;

    [Header("Debug")]
    public bool debugMessages = true;

    private float nextFireTime;
    private bool reloading;

    public int AmmoInMagazine => ammoInMagazine;
    public int ReserveAmmo => reserveAmmo;
    public bool IsReloading => reloading;

    private void Awake()
    {
        ammoInMagazine = Mathf.Clamp(ammoInMagazine, 0, magazineSize);
    }

    private void Update()
    {
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
            Debug.Log(weaponName + " ammo added: " + amount);
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

        if (debugMessages)
            Debug.Log(weaponName + " fired. Ammo: " + ammoInMagazine + " / " + reserveAmmo);

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (audioSource != null && shootClip != null)
            audioSource.PlayOneShot(shootClip);

        if (animator != null)
            animator.SetTrigger("Shoot");

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = shootCam.transform.forward;

            direction += shootCam.transform.right * Random.Range(-spread, spread);
            direction += shootCam.transform.up * Random.Range(-spread, spread);
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
                // Ignore player and held weapon
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
        if (ammoInMagazine >= magazineSize || reserveAmmo <= 0)
            yield break;

        reloading = true;

        if (animator != null)
            animator.SetTrigger("Reload");

        if (audioSource != null && reloadClip != null)
            audioSource.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadTime);

        int needed = magazineSize - ammoInMagazine;
        int loaded = Mathf.Min(needed, reserveAmmo);

        ammoInMagazine += loaded;
        reserveAmmo -= loaded;

        reloading = false;

        if (debugMessages)
            Debug.Log(weaponName + " reloaded. Ammo: " + ammoInMagazine + " / " + reserveAmmo);
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