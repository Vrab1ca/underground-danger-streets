using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth;
    public GameObject deathPanel;

    void Awake()
    {
        currentHealth = maxHealth;

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (currentHealth <= 0f)
            return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Player healed. Current health: " + currentHealth);
    }

    public void HealOverTime(float totalHealAmount, float healPerTick, float secondsBetweenTicks)
    {
        StartCoroutine(HealOverTimeRoutine(totalHealAmount, healPerTick, secondsBetweenTicks));
    }

    private IEnumerator HealOverTimeRoutine(float totalHealAmount, float healPerTick, float secondsBetweenTicks)
    {
        float healedAmount = 0f;

        while (healedAmount < totalHealAmount)
        {
            yield return new WaitForSeconds(secondsBetweenTicks);

            if (currentHealth <= 0f)
                yield break;

            if (currentHealth >= maxHealth)
                yield break;

            float healThisTick = Mathf.Min(healPerTick, totalHealAmount - healedAmount);

            Heal(healThisTick);

            healedAmount += healThisTick;
        }
    }

    void Die()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (deathPanel != null)
            deathPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}