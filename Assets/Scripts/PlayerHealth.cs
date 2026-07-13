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

    [Header("Debug")]
    public bool debugMessages = true;

    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;
        Time.timeScale = 1f;

        if (deathPanel != null)
            deathPanel.SetActive(false);

        FindPlayerArmor();
    }

    private void Start()
    {
        FindPlayerArmor();
    }

    private void FindPlayerArmor()
    {
        if (playerArmor != null)
            return;

        playerArmor = GetComponent<PlayerArmor>();

        if (playerArmor == null)
            playerArmor = GetComponentInParent<PlayerArmor>();

        if (playerArmor == null)
            playerArmor = GetComponentInChildren<PlayerArmor>(true);

        if (playerArmor == null)
            playerArmor = FindFirstObjectByType<PlayerArmor>();

        if (playerArmor == null)
        {
            Debug.LogWarning(
                "PlayerHealth cannot find PlayerArmor. " +
                "Assign the PlayerArmor component in the Inspector."
            );
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0f)
            return;

        FindPlayerArmor();

        // Armor always receives damage before player HP.
        if (playerArmor != null && playerArmor.IsArmorActive)
        {
            amount = playerArmor.ProtectFromDamage(amount);

            // Armor blocked the complete current hit.
            if (amount <= 0f)
            {
                if (debugMessages)
                {
                    Debug.Log(
                        "Player HP unchanged: " +
                        currentHealth + " / " + maxHealth
                    );
                }

                return;
            }
        }

        currentHealth = Mathf.Max(0f, currentHealth - amount);

        if (debugMessages)
        {
            Debug.Log(
                "PLAYER HP DAMAGE: -" + amount +
                " | HP: " + currentHealth + " / " + maxHealth
            );
        }

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead || amount <= 0f)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        if (debugMessages)
        {
            Debug.Log(
                "Player healed: +" + amount +
                " | HP: " + currentHealth + " / " + maxHealth
            );
        }
    }

    public void HealOverTime(
        float totalHealAmount,
        float healPerTick,
        float secondsBetweenTicks
    )
    {
        if (!isDead)
        {
            StartCoroutine(
                HealOverTimeRoutine(
                    totalHealAmount,
                    healPerTick,
                    secondsBetweenTicks
                )
            );
        }
    }

    private IEnumerator HealOverTimeRoutine(
        float totalHealAmount,
        float healPerTick,
        float secondsBetweenTicks
    )
    {
        float healedAmount = 0f;

        while (healedAmount < totalHealAmount)
        {
            yield return new WaitForSeconds(secondsBetweenTicks);

            if (isDead || currentHealth >= maxHealth)
                yield break;

            float healThisTick = Mathf.Min(
                healPerTick,
                totalHealAmount - healedAmount
            );

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