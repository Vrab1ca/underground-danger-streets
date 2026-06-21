using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HelicopterBomb : MonoBehaviour
{
    [Header("Explosion")]
    public float damage = 150f;
    public float explosionRadius = 8f;
    public float explosionForce = 800f;
    public float armDelay = 0.2f;

    [Header("Auto Explode")]
    public float autoExplodeTime = 8f;

    [Header("Effects Optional")]
    public GameObject explosionEffect;

    private Rigidbody rb;
    private float spawnTime;
    private bool exploded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        spawnTime = Time.time;
        Invoke(nameof(Explode), autoExplodeTime);
    }

    public void Launch(Vector3 startVelocity)
    {
        rb.linearVelocity = startVelocity;
        rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - spawnTime < armDelay)
            return;

        Explode();
    }

    private void Explode()
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

                float finalDamage = damage * damagePercent;

                if (finalDamage < damage * 0.25f)
                    finalDamage = damage * 0.25f;

                damageable.TakeDamage(finalDamage);
                damagedObjects.Add(damageable);

                Debug.Log("Bomb damaged: " + hit.name + " Damage: " + finalDamage);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}