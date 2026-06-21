using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public enum HealMode
    {
        Instant,
        OverTime
    }

    [Header("Heal Mode")]
    public HealMode healMode = HealMode.Instant;

    [Header("Instant Heal Settings")]
    public float instantHealAmount = 50f;

    [Header("Over Time Heal Settings")]
    public float totalHealAmount = 50f;
    public float healPerTick = 10f;
    public float secondsBetweenTicks = 1.5f;

    [Header("Pickup Settings")]
    public bool destroyAfterPickup = true;

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used)
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth = other.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth == null)
            return;

        used = true;

        if (healMode == HealMode.Instant)
        {
            playerHealth.Heal(instantHealAmount);
        }
        else if (healMode == HealMode.OverTime)
        {
            playerHealth.HealOverTime(totalHealAmount, healPerTick, secondsBetweenTicks);
        }

        if (destroyAfterPickup)
        {
            Destroy(gameObject);
        }
    }
}