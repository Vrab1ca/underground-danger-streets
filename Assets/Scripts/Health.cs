using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public bool destroyOnDeath = true;
    public UnityEvent onDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0f) return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        onDeath?.Invoke();

        if (destroyOnDeath)
            Destroy(gameObject);
    }
}
