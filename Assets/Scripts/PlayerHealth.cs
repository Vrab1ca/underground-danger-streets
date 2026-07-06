using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public GameObject deathPanel;

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Armor")]
    public PlayerArmor playerArmor;

    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;

        Time.timeScale = 1f;

        if (deathPanel != null)
            deathPanel.SetActive(false);

        if (playerArmor == null)
            playerArmor = GetComponent<PlayerArmor>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        // Armor protects first.
        if (playerArmor != null)
        {
            amount = playerArmor.ProtectFromDamage(amount);

            if (amount <= 0f)
            {
                Debug.Log("Armor blocked the damage. Player health: " + currentHealth);
                return;
            }
        }

        // If armor did not block everything, player takes damage.
        currentHealth -= amount;

        if (currentHealth < 0f)
            currentHealth = 0f;

        Debug.Log("Player health: " + currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead)
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

            if (isDead)
                yield break;

            if (currentHealth >= maxHealth)
                yield break;

            float healThisTick = Mathf.Min(healPerTick, totalHealAmount - healedAmount);

            Heal(healThisTick);

            healedAmount += healThisTick;
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        currentHealth = 0f;

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (deathPanel != null)
            deathPanel.SetActive(true);
        else
            Debug.LogWarning("Death Panel is missing in PlayerHealth.");

        Debug.Log("Player died.");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        LoadingScreenLoader.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        LoadingScreenLoader.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        Debug.Log("Quit game.");
        Application.Quit();
    }
}