using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Camera fpsCamera;
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
    public float spread = 0.015f;

    [Header("Ammo")]
    public int magazineSize = 30;
    public int ammoInMagazine = 30;
    public int reserveAmmo = 90;
    public float reloadTime = 1.5f;

    private float nextFireTime;
    private bool reloading;

    public int AmmoInMagazine => ammoInMagazine;
    public int ReserveAmmo => reserveAmmo;
    public bool IsReloading => reloading;

    void Awake()
    {
        if (fpsCamera == null)
            fpsCamera = Camera.main;

        ammoInMagazine = Mathf.Clamp(ammoInMagazine, 0, magazineSize);
    }

    void Update()
    {
        if (reloading) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        bool wantsShoot = automatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");
        if (wantsShoot && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo += amount;
    }

    void Shoot()
    {
        if (ammoInMagazine <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        ammoInMagazine--;

        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource != null && shootClip != null) audioSource.PlayOneShot(shootClip);
        if (animator != null) animator.SetTrigger("Shoot");

        Vector3 direction = fpsCamera.transform.forward;
        direction += fpsCamera.transform.right * Random.Range(-spread, spread);
        direction += fpsCamera.transform.up * Random.Range(-spread, spread);

        if (Physics.Raycast(fpsCamera.transform.position, direction, out RaycastHit hit, range))
        {
            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(damage);

            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }
    }

    IEnumerator Reload()
    {
        if (ammoInMagazine >= magazineSize || reserveAmmo <= 0)
            yield break;

        reloading = true;
        if (animator != null) animator.SetTrigger("Reload");
        if (audioSource != null && reloadClip != null) audioSource.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadTime);

        int needed = magazineSize - ammoInMagazine;
        int loaded = Mathf.Min(needed, reserveAmmo);

        ammoInMagazine += loaded;
        reserveAmmo -= loaded;
        reloading = false;
    }
}
