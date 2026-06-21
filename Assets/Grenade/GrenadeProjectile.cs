using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrenadeProjectile : MonoBehaviour
{
    [Header("Type")]
    public GrenadeType grenadeType = GrenadeType.Normal;

    [Header("Normal Grenade")]
    public float fuseTime = 2.5f;
    public float explosionDamage = 120f;
    public float explosionRadius = 6f;
    public float explosionForce = 700f;

    [Header("Molotov")]
    public float armDelay = 0.4f;
    public GameObject fireZonePrefab;
    public float molotovFireLifeTime = 8f;

    [Header("Raycast Collision Fix")]
    public bool useRaycastCollisionFix = true;
    public LayerMask collisionMask = ~0;

    [Header("Effects Optional")]
    public GameObject explosionEffect;
    public GameObject molotovBreakEffect;

    private Rigidbody rb;
    private bool exploded;
    private float spawnTime;
    private Vector3 lastPosition;
    private Transform ownerRoot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        spawnTime = Time.time;
        lastPosition = transform.position;

        if (grenadeType == GrenadeType.Normal)
        {
            Invoke(nameof(ExplodeNormalGrenade), fuseTime);
        }
    }

    public void SetOwner(Transform owner)
    {
        if (owner == null)
            return;

        ownerRoot = owner.root;

        Collider[] ownerColliders = ownerRoot.GetComponentsInChildren<Collider>();
        Collider[] grenadeColliders = GetComponentsInChildren<Collider>();

        foreach (Collider ownerCollider in ownerColliders)
        {
            foreach (Collider grenadeCollider in grenadeColliders)
            {
                Physics.IgnoreCollision(ownerCollider, grenadeCollider);
            }
        }
    }

    private void Update()
    {
        if (exploded)
            return;

        if (grenadeType == GrenadeType.Molotov && useRaycastCollisionFix)
        {
            CheckMolotovRaycastHit();
        }

        lastPosition = transform.position;
    }

    private void CheckMolotovRaycastHit()
    {
        if (Time.time - spawnTime < armDelay)
            return;

        Vector3 movement = transform.position - lastPosition;
        float distance = movement.magnitude;

        if (distance <= 0.01f)
            return;

        Vector3 direction = movement.normalized;

        RaycastHit[] hits = Physics.RaycastAll(
            lastPosition,
            direction,
            distance + 0.2f,
            collisionMask,
            QueryTriggerInteraction.Ignore
        );

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform))
                continue;

            if (ownerRoot != null && hit.collider.transform.IsChildOf(ownerRoot))
                continue;

            Debug.Log("Molotov raycast hit real object: " + hit.collider.name);

            BreakMolotov(hit.point);
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (exploded)
            return;

        if (Time.time - spawnTime < armDelay)
            return;

        if (ownerRoot != null && collision.collider.transform.IsChildOf(ownerRoot))
            return;

        if (grenadeType == GrenadeType.Molotov)
        {
            Vector3 hitPoint = collision.contacts.Length > 0
                ? collision.contacts[0].point
                : transform.position;

            Debug.Log("Molotov collision hit real object: " + collision.collider.name);

            BreakMolotov(hitPoint);
        }
    }

    private void ExplodeNormalGrenade()
    {
        if (exploded)
            return;

        exploded = true;

        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 3f);
        }

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        HashSet<IDamageable> damagedObjects = new HashSet<IDamageable>();

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable != null && !damagedObjects.Contains(damageable))
            {
                float distance = Vector3.Distance(transform.position, hit.ClosestPoint(transform.position));
                float damagePercent = Mathf.Clamp01(1f - distance / explosionRadius);

                float finalDamage = explosionDamage * damagePercent;

                if (finalDamage < explosionDamage * 0.25f)
                    finalDamage = explosionDamage * 0.25f;

                damageable.TakeDamage(finalDamage);
                damagedObjects.Add(damageable);

                Debug.Log("Grenade damaged: " + hit.name + " Damage: " + finalDamage);
            }

            Rigidbody hitRb = hit.attachedRigidbody;

            if (hitRb != null)
            {
                hitRb.AddExplosionForce(
                    explosionForce,
                    transform.position,
                    explosionRadius
                );
            }
        }

        Destroy(gameObject);
    }

    private void BreakMolotov(Vector3 breakPosition)
    {
        if (exploded)
            return;

        exploded = true;

        Debug.Log("Molotov broke and created fire.");

        if (molotovBreakEffect != null)
        {
            GameObject effect = Instantiate(molotovBreakEffect, breakPosition, Quaternion.identity);
            Destroy(effect, 3f);
        }

        if (fireZonePrefab != null)
        {
            GameObject fire = Instantiate(
                fireZonePrefab,
                breakPosition,
                Quaternion.identity
            );

            Destroy(fire, molotovFireLifeTime);
        }
        else
        {
            Debug.LogWarning("Molotov Fire Zone Prefab is missing.");
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (grenadeType == GrenadeType.Normal)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}