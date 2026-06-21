using System.Collections;
using UnityEngine;

public class HelicopterGun : MonoBehaviour
{
    [Header("References")]
    public HelicopterController helicopterController;
    public Camera shootCamera;
    public Transform shootPoint;

    [Header("Bullet Visual")]
    public GameObject bulletVisualPrefab;
    public float bulletVisualSpeed = 150f;
    public float bulletVisualLifeTime = 2f;

    [Header("Shooting")]
    public float damage = 25f;
    public float range = 200f;
    public float fireRate = 10f;
    public bool automatic = true;
    public float spread = 0.01f;

    [Header("Ammo")]
    public int magazineSize = 100;
    public int ammoInMagazine = 100;
    public int reserveAmmo = 300;
    public float reloadTime = 2f;

    [Header("Effects Optional")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private float nextFireTime;
    private bool reloading;

    private void Awake()
    {
        ammoInMagazine = Mathf.Clamp(ammoInMagazine, 0, magazineSize);
    }

    private void Update()
    {
        if (helicopterController == null)
            return;

        if (!helicopterController.canFly)
            return;

        if (reloading)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        bool wantsShoot = automatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (wantsShoot && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (ammoInMagazine <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (shootCamera == null)
        {
            Debug.LogWarning("HelicopterGun: Shoot Camera is missing.");
            return;
        }

        ammoInMagazine--;

        Debug.Log("Helicopter fired. Ammo: " + ammoInMagazine + " / " + reserveAmmo);

        if (muzzleFlash != null)
            muzzleFlash.Play();

        Vector3 origin;

        if (shootPoint != null)
            origin = shootPoint.position;
        else
            origin = shootCamera.transform.position + shootCamera.transform.forward * 1f;

        Vector3 direction = shootCamera.transform.forward;

        direction += shootCamera.transform.right * Random.Range(-spread, spread);
        direction += shootCamera.transform.up * Random.Range(-spread, spread);
        direction.Normalize();

        Vector3 targetPosition = origin + direction * range;

        Debug.DrawRay(origin, direction * range, Color.red, 1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, ~0, QueryTriggerInteraction.Ignore))
        {
            targetPosition = hit.point;

            Debug.Log("Helicopter hit: " + hit.collider.name);

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                Debug.Log("Damaged: " + hit.collider.name);
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
        }

        SpawnBulletVisual(origin, targetPosition);
    }

    private void SpawnBulletVisual(Vector3 startPosition, Vector3 targetPosition)
    {
        if (bulletVisualPrefab == null)
            return;

        GameObject bullet = Instantiate(
            bulletVisualPrefab,
            startPosition,
            Quaternion.identity
        );

        Vector3 direction = targetPosition - startPosition;

        if (direction != Vector3.zero)
            bullet.transform.rotation = Quaternion.LookRotation(direction);

        HelicopterBulletVisual visual = bullet.GetComponent<HelicopterBulletVisual>();

        if (visual != null)
        {
            visual.Init(
                targetPosition,
                bulletVisualSpeed,
                bulletVisualLifeTime
            );
        }
        else
        {
            Destroy(bullet, bulletVisualLifeTime);
        }
    }

    private IEnumerator Reload()
    {
        if (ammoInMagazine >= magazineSize || reserveAmmo <= 0)
            yield break;

        reloading = true;

        Debug.Log("Helicopter reloading...");

        yield return new WaitForSeconds(reloadTime);

        int needed = magazineSize - ammoInMagazine;
        int loaded = Mathf.Min(needed, reserveAmmo);

        ammoInMagazine += loaded;
        reserveAmmo -= loaded;

        reloading = false;

        Debug.Log("Helicopter reloaded.");
    }
}