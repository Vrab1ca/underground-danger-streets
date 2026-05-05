using System.Collections;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public float fuseTime = 3f;
    public float radius = 6f;
    public float damage = 80f;
    public float explosionForce = 600f;
    public GameObject explosionEffect;

    void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    void Explode()
    {
        if (explosionEffect != null)
            Destroy(Instantiate(explosionEffect, transform.position, Quaternion.identity), 3f);

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(damage);

            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, radius);
        }

        Destroy(gameObject);
    }
}
