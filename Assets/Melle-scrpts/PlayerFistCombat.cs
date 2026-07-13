using System.Collections.Generic;
using UnityEngine;

public class PlayerFistCombat : MonoBehaviour
{
    [Header("References")]
    public WeaponSwitcher weaponSwitcher;
    public Camera playerCamera;
    public SimpleMeleeAnimation handsAnimation;

    [Header("Punch Damage")]
    public float damage = 8f;
    public float range = 1.2f;
    public float hitRadius = 0.55f;
    public float attackAngle = 80f;
    public float attacksPerSecond = 1.7f;
    public LayerMask damageLayers = ~0;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip punchSound;

    [Header("Debug")]
    public bool debugMessages = true;

    private float nextAttackTime;

    private void Awake()
    {
        if (weaponSwitcher == null)
        {
            weaponSwitcher =
                GetComponent<WeaponSwitcher>();
        }

        if (weaponSwitcher == null)
        {
            weaponSwitcher =
                GetComponentInParent<WeaponSwitcher>();
        }

        if (weaponSwitcher == null)
        {
            weaponSwitcher =
                FindFirstObjectByType<WeaponSwitcher>();
        }

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (weaponSwitcher == null)
            return;

        // Fists only work while normal hands are selected.
        if (!weaponSwitcher.HandsActive)
            return;

        if (Input.GetMouseButtonDown(0))
            TryPunch();
    }

    private void TryPunch()
    {
        if (Time.time < nextAttackTime)
            return;

        float safeAttackSpeed =
            Mathf.Max(0.01f, attacksPerSecond);

        nextAttackTime =
            Time.time + 1f / safeAttackSpeed;

        if (handsAnimation != null)
            handsAnimation.PlayAttack();

        if (audioSource != null &&
            punchSound != null)
        {
            audioSource.PlayOneShot(punchSound);
        }

        DamageClosestTarget();
    }

    private void DamageClosestTarget()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning(
                "PlayerFistCombat: Player Camera is missing."
            );

            return;
        }

        Vector3 cameraPosition =
            playerCamera.transform.position;

        Vector3 cameraForward =
            playerCamera.transform.forward;

        Vector3 hitCenter =
            cameraPosition +
            cameraForward * range;

        Collider[] hits =
            Physics.OverlapSphere(
                hitCenter,
                hitRadius,
                damageLayers,
                QueryTriggerInteraction.Ignore
            );

        IDamageable closestTarget = null;
        Collider closestCollider = null;

        float closestDistance =
            Mathf.Infinity;

        HashSet<IDamageable> checkedTargets =
            new HashSet<IDamageable>();

        foreach (Collider hit in hits)
        {
            if (hit == null)
                continue;

            // Do not hit the player or player's objects.
            if (hit.transform.IsChildOf(transform.root))
                continue;

            IDamageable damageable =
                hit.GetComponentInParent<IDamageable>();

            if (damageable == null)
                continue;

            if (checkedTargets.Contains(damageable))
                continue;

            checkedTargets.Add(damageable);

            MonoBehaviour targetBehaviour =
                damageable as MonoBehaviour;

            if (targetBehaviour == null)
                continue;

            Vector3 directionToTarget =
                targetBehaviour.transform.position -
                cameraPosition;

            float distance =
                directionToTarget.magnitude;

            if (distance > range + hitRadius)
                continue;

            if (directionToTarget.sqrMagnitude <= 0.001f)
                continue;

            float angle =
                Vector3.Angle(
                    cameraForward,
                    directionToTarget.normalized
                );

            if (angle > attackAngle)
                continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = damageable;
                closestCollider = hit;
            }
        }

        if (closestTarget != null)
        {
            closestTarget.TakeDamage(damage);

            if (debugMessages)
            {
                Debug.Log(
                    "Hands hit " +
                    closestCollider.name +
                    " for " +
                    damage +
                    " damage."
                );
            }
        }
        else
        {
            if (debugMessages)
                Debug.Log("Hands missed.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Camera cam = playerCamera;

        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            return;

        Gizmos.color = Color.yellow;

        Vector3 center =
            cam.transform.position +
            cam.transform.forward * range;

        Gizmos.DrawWireSphere(
            center,
            hitRadius
        );
    }
}