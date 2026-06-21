using System.Collections.Generic;
using UnityEngine;

public class FireZone : MonoBehaviour
{
    [Header("Fire Damage")]
    public float damagePerSecond = 15f;
    public float radius = 5f;
    public float tickRate = 0.5f;

    [Header("Visual Optional")]
    public GameObject fireEffect;
    public GameObject smokeEffect;

    private float nextTickTime;

    private void Start()
    {
        if (fireEffect != null)
            Instantiate(fireEffect, transform.position, Quaternion.identity, transform);

        if (smokeEffect != null)
            Instantiate(smokeEffect, transform.position, Quaternion.identity, transform);
    }

    private void Update()
    {
        if (Time.time < nextTickTime)
            return;

        nextTickTime = Time.time + tickRate;

        DamageInsideFire();
    }

    private void DamageInsideFire()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            radius,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        HashSet<IDamageable> damagedObjects = new HashSet<IDamageable>();

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable == null)
                continue;

            if (damagedObjects.Contains(damageable))
                continue;

            float damage = damagePerSecond * tickRate;

            damageable.TakeDamage(damage);
            damagedObjects.Add(damageable);

            Debug.Log("Fire damaged: " + hit.name + " Damage: " + damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}