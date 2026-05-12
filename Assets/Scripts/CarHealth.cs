using System.Collections;
using UnityEngine;

public class CarHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 300f;
    public float currentHealth = 300f;

    public bool destroyWhenBroken = true;
    public float destroyDelay = 4f;

    public GameObject brokenEffect;
    public GameObject explosionEffect;

    private bool broken = false;

    public bool IsBroken
    {
        get { return broken; }
    }

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (broken)
            return;

        currentHealth -= amount;

        Debug.Log("Car took damage: " + amount + " | HP: " + currentHealth);

        if (currentHealth <= 0f)
            BreakCar();
    }

    public void BreakCar()
    {
        if (broken)
            return;

        broken = true;
        currentHealth = 0f;

        BetterCarController carController = GetComponent<BetterCarController>();

        if (carController != null)
            carController.BreakCar();

        if (brokenEffect != null)
            Instantiate(brokenEffect, transform.position, transform.rotation);

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
            rb.AddForce(Vector3.up * 4f, ForceMode.VelocityChange);

        if (destroyWhenBroken)
            StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}