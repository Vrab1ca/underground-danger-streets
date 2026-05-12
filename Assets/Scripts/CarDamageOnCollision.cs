using UnityEngine;

public class CarDamageOnCollision : MonoBehaviour
{
    public CarHealth carHealth;

    public float minDamageSpeed = 6f;
    public float damageMultiplier = 8f;
    public float maxCollisionDamage = 80f;

    void Awake()
    {
        if (carHealth == null)
            carHealth = GetComponent<CarHealth>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (carHealth == null)
            return;

        if (carHealth.IsBroken)
            return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed < minDamageSpeed)
            return;

        float damage = impactSpeed * damageMultiplier;
        damage = Mathf.Clamp(damage, 0f, maxCollisionDamage);

        carHealth.TakeDamage(damage);
    }
}