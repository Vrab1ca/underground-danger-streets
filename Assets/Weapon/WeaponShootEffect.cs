using System.Collections;
using UnityEngine;

public class WeaponShootEffect : MonoBehaviour
{
    [Header("Weapon")]
    public Transform weaponTransform;

    [Header("Shoot Recoil")]
    public Vector3 recoilPosition = new Vector3(0f, 0f, -0.08f);
    public Vector3 recoilRotation = new Vector3(-6f, 2f, 0f);

    public float recoilBackSpeed = 25f;
    public float recoilReturnSpeed = 12f;

    [Header("Reload 360 Spin")]
    public bool useReloadSpin = true;

    // Z = roll spin, good for cool rifle spin
    public Vector3 reloadSpinAxis = new Vector3(0f, 0f, 1f);

    public float reloadSpinDegrees = 360f;

    // little movement during reload
    public Vector3 reloadMovePosition = new Vector3(0f, -0.08f, -0.12f);

    [Header("Effects Optional")]
    public ParticleSystem muzzleFlash;
    public AudioSource shootAudio;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Coroutine recoilCoroutine;
    private Coroutine reloadCoroutine;

    private void Awake()
    {
        if (weaponTransform == null)
            weaponTransform = transform;

        originalPosition = weaponTransform.localPosition;
        originalRotation = weaponTransform.localRotation;
    }

    public void PlayShootEffect()
    {
        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (shootAudio != null)
            shootAudio.Play();

        if (reloadCoroutine != null)
            return;

        if (recoilCoroutine != null)
            StopCoroutine(recoilCoroutine);

        recoilCoroutine = StartCoroutine(RecoilRoutine());
    }

    public void PlayReloadSpin(float reloadTime)
    {
        if (!useReloadSpin)
            return;

        if (reloadCoroutine != null)
            StopCoroutine(reloadCoroutine);

        if (recoilCoroutine != null)
            StopCoroutine(recoilCoroutine);

        reloadCoroutine = StartCoroutine(ReloadSpinRoutine(reloadTime));
    }

    private IEnumerator RecoilRoutine()
    {
        Vector3 targetPosition = originalPosition + recoilPosition;
        Quaternion targetRotation = originalRotation * Quaternion.Euler(recoilRotation);

        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * recoilBackSpeed;

            weaponTransform.localPosition = Vector3.Lerp(originalPosition, targetPosition, timer);
            weaponTransform.localRotation = Quaternion.Slerp(originalRotation, targetRotation, timer);

            yield return null;
        }

        timer = 0f;

        Vector3 currentPosition = weaponTransform.localPosition;
        Quaternion currentRotation = weaponTransform.localRotation;

        while (timer < 1f)
        {
            timer += Time.deltaTime * recoilReturnSpeed;

            weaponTransform.localPosition = Vector3.Lerp(currentPosition, originalPosition, timer);
            weaponTransform.localRotation = Quaternion.Slerp(currentRotation, originalRotation, timer);

            yield return null;
        }

        weaponTransform.localPosition = originalPosition;
        weaponTransform.localRotation = originalRotation;

        recoilCoroutine = null;
    }

    private IEnumerator ReloadSpinRoutine(float reloadTime)
    {
        if (reloadTime <= 0.1f)
            reloadTime = 1f;

        float timer = 0f;

        Vector3 startPosition = weaponTransform.localPosition;
        Vector3 reloadPosition = originalPosition + reloadMovePosition;

        while (timer < reloadTime)
        {
            timer += Time.deltaTime;

            float percent = timer / reloadTime;
            percent = Mathf.Clamp01(percent);

            float angle = percent * reloadSpinDegrees;

            weaponTransform.localRotation = originalRotation * Quaternion.Euler(reloadSpinAxis * angle);

            if (percent < 0.5f)
            {
                weaponTransform.localPosition = Vector3.Lerp(startPosition, reloadPosition, percent * 2f);
            }
            else
            {
                weaponTransform.localPosition = Vector3.Lerp(reloadPosition, originalPosition, (percent - 0.5f) * 2f);
            }

            yield return null;
        }

        weaponTransform.localPosition = originalPosition;
        weaponTransform.localRotation = originalRotation;

        reloadCoroutine = null;
    }
}