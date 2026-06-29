using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;

    [Header("Run")]
    public float runStaminaDrainPerSecond = 18f;
    public float minStaminaToRun = 5f;

    [Header("Jump")]
    public float jumpStaminaCost = 20f;

    [Header("Restore")]
    public float staminaRegenPerSecond = 15f;
    public float regenDelay = 1f;
    public float exhaustedRecoverStamina = 25f;

    [Header("State")]
    public bool exhausted;

    private float lastUseTime;

    public float StaminaPercent
    {
        get
        {
            return currentStamina / maxStamina;
        }
    }

    private void Awake()
    {
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    private void Update()
    {
        RegenerateStamina();
    }

    public bool CanRun()
    {
        if (exhausted)
            return false;

        return currentStamina > minStaminaToRun;
    }

    public void UseRunStamina()
    {
        if (currentStamina <= 0f)
            return;

        currentStamina -= runStaminaDrainPerSecond * Time.deltaTime;
        lastUseTime = Time.time;

        if (currentStamina <= 0f)
        {
            currentStamina = 0f;
            exhausted = true;
        }
    }

    public bool TryUseJumpStamina()
    {
        if (currentStamina < jumpStaminaCost)
        {
            Debug.Log("Not enough stamina to jump.");
            return false;
        }

        currentStamina -= jumpStaminaCost;
        lastUseTime = Time.time;

        if (currentStamina <= 0f)
        {
            currentStamina = 0f;
            exhausted = true;
        }

        return true;
    }

    private void RegenerateStamina()
    {
        if (Time.time < lastUseTime + regenDelay)
            return;

        if (currentStamina >= maxStamina)
            return;

        currentStamina += staminaRegenPerSecond * Time.deltaTime;

        if (currentStamina > maxStamina)
            currentStamina = maxStamina;

        if (exhausted && currentStamina >= exhaustedRecoverStamina)
            exhausted = false;
    }
}